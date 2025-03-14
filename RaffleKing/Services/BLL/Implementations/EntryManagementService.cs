﻿using System.Globalization;
using System.Text.RegularExpressions;
using RaffleKing.Common;
using RaffleKing.Data.Models;
using RaffleKing.Services.BLL.Interfaces;
using RaffleKing.Services.DAL.Interfaces;

namespace RaffleKing.Services.BLL.Implementations;

public class EntryManagementService(IUserService userService, IEntryService entryService, IDrawService drawService,
    IEmailService emailService, IWinnerService winnerService) : IEntryManagementService
{
    public async Task<OperationResult<string>> TryEnterRaffle(int drawId, int numberOfEntries, string guestEmail = "")
    {
        var canEnter = await CurrentUserCanEnterDraw(drawId);
        var isGuest = await userService.IsGuest();

        if (!canEnter.Success)
            return OperationResult<string>.Fail(canEnter.Message, "");

        if (isGuest)
        {
            if(numberOfEntries > 1)
                return OperationResult<string>.Fail("Guests can only enter each raffle once! Create an account for " +
                                                    "more entries.", "");
            
            if (await IsGuestParticipatingInDraw(drawId, guestEmail))
                return OperationResult<string>.Fail("You are already participating in this draw as a guest! Create an " +
                                                    "account for more entries.", "");
            
            var validEmail = IsValidEmailAddress(guestEmail);
            if (!validEmail.Success)
                return OperationResult<string>.Fail(validEmail.Message, "");
        }

        if (numberOfEntries > await CountCurrentUserEntriesRemainingByDraw(drawId))
            return OperationResult<string>.Fail("Selected number of entries exceeds available entries!", "");
        
        List<EntryModel> entries = [];

        var guestReferenceCode = Guid.NewGuid().ToString("N");
        if (isGuest)
        {
            
            entries.Add(new EntryModel
            {
                DrawId = drawId,
                IsGuest = true,
                GuestEmail = guestEmail,
                GuestReferenceCode = guestReferenceCode
            });

            emailService.SendGuestEntranceEmail(guestEmail, guestReferenceCode);
        }
        else
        {
            for (var i = 0; i < numberOfEntries; i++)
            {
                entries.Add(new EntryModel
                {
                    DrawId = drawId,
                    UserId = await userService.GetUserId()
                });
            }
        }

        foreach (var entry in entries)
            await entryService.AddEntry(entry);
        
        return OperationResult<string>.Ok(guestReferenceCode);
    }

    /// <summary>
    /// Use RegEx to perform some basic validation on the format of an email address. Adapted from Microsoft Learn -
    /// https://learn.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
    /// </summary>
    /// <param name="email">The email address to validate</param>
    /// <returns></returns>
    private OperationResult IsValidEmailAddress(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return OperationResult.Fail("An email address must be supplied to enter as a guest.");

        try
        {
            // Normalize the domain
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                RegexOptions.None, TimeSpan.FromMilliseconds(200));

            // Examines the domain part of the email and normalizes it.
            string DomainMapper(Match match)
            {
                // Use IdnMapping class to convert Unicode domain names.
                var idn = new IdnMapping();

                // Pull out and process domain name (throws ArgumentException on invalid)
                var domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException e)
        {
            return OperationResult.Fail("Operation timed out.");
        }
        catch (ArgumentException e)
        {
            return OperationResult.Fail("Unhandled exception.");
        }

        try
        {
            var isValid = Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            
            if(isValid)
                return OperationResult.Ok();
            else
                return OperationResult.Fail("Email address is not valid.");
        }
        catch (RegexMatchTimeoutException)
        {
            return OperationResult.Fail("Operation timed out.");
        }
    }

    public async Task<OperationResult<string>> TryEnterLottery(int drawId, IEnumerable<int> luckyNumbers)
    {
        var canEnter = await CurrentUserCanEnterDraw(drawId);

        if (!canEnter.Success)
            return OperationResult<string>.Fail(canEnter.Message, "");

        luckyNumbers = luckyNumbers.ToList();
        if (!luckyNumbers.Any())
            return OperationResult<string>.Fail("At least 1 Lucky Number must be selected.", "");
        
        if (luckyNumbers.Count() > await CountCurrentUserEntriesRemainingByDraw(drawId))
            return OperationResult<string>.Fail("Selected number of entries exceeds available entries!", "");

        var availableLuckyNumbers = await GetAvailableLuckyNumbersByDraw(drawId);
        var unavailableNumbers = 
            luckyNumbers.Where(luckyNumber => !availableLuckyNumbers.Contains(luckyNumber)).ToList();
        
        if (unavailableNumbers.Count != 0)
            return OperationResult<string>.Fail($"Lucky Number(s) {string.Join(", ", unavailableNumbers)} is/are " +
                                        $"not available!", "");
        
        List<EntryModel> entries = [];

        foreach (var luckyNumber in luckyNumbers)
        {
            entries.Add(new EntryModel
            {
                DrawId = drawId,
                UserId = await userService.GetUserId(),
                LuckyNumber = luckyNumber
            });
        }
        
        foreach (var entry in entries)
            await entryService.AddEntry(entry);
        
        return OperationResult<string>.Ok("");
    }

    public async Task<OperationResult> CurrentUserCanEnterDraw(int drawId)
    {
        var draw = await drawService.GetDrawById(drawId);

        if (await userService.IsHostOfDraw(drawId))
            return OperationResult.Fail("You cannot enter your own draw!");
        
        if(await CountEntriesRemainingByDraw(drawId) < 1)
            return OperationResult.Fail("This draw has no entries remaining!");

        if (await CountCurrentUserEntriesRemainingByDraw(drawId) < 1)
            return OperationResult.Fail("You have no remaining entries for this draw!");

        switch (draw?.DrawType)
        {
            case DrawTypeEnum.Raffle:
                break;
            case DrawTypeEnum.Lottery:
                if(await userService.IsGuest())
                    return OperationResult.Fail("Only registered users can enter lotteries!");
                break;
            default:
                return OperationResult.Fail("Invalid draw type.");
        }
        
        return OperationResult.Ok();
    }

    public async Task<EntryModel?> GetEntryById(int entryId)
    {
        return await entryService.GetEntryById(entryId);
    }

    public async Task<List<EntryModel>?> GetEntriesByDraw(int drawId)
    {
        return await entryService.GetEntriesByDraw(drawId);
    }

    public async Task<List<EntryModel>?> GetCurrentUserEntries()
    {
        var currentUserId = await userService.GetUserId();
        if (currentUserId == null)
            return null;
        
        return await entryService.GetEntriesByUser(currentUserId);
    }

    public async Task<List<EntryModel>?> GetCurrentUserEntriesByDraw(int drawId)
    {
        var currentUserId = await userService.GetUserId();
        if (currentUserId == null)
            return null;

        return await entryService.GetEntriesByUserAndDraw(currentUserId, drawId);
    }

    public async Task<EntryModel?> GetGuestEntry(string guestRef)
    {
        return await entryService.GetEntryByGuestRef(guestRef);
    }

    public async Task RemoveEntriesByDraw(int drawId)
    {
        await entryService.DeleteEntriesByDraw(drawId);
    }

    public async Task RemoveCurrentUserEntriesByDraw(int drawId)
    {
        var currentUserId = await userService.GetUserId();
        if (currentUserId != null)
            await entryService.DeleteEntriesByUserAndDraw(currentUserId, drawId);
    }

    public async Task<int> CountEntriesByDraw(int drawId)
    {
        return await entryService.CountEntriesByDraw(drawId);
    }

    public async Task<int> CountEntriesRemainingByDraw(int drawId)
    {
        var currentEntriesCount = await entryService.CountEntriesByDraw(drawId);
        var draw = await drawService.GetDrawById(drawId);
        return draw?.MaxEntriesTotal - currentEntriesCount ?? 0;
    }

    public async Task<int> CountCurrentUserEntriesByDraw(int drawId)
    {
        var currentUserId = await userService.GetUserId();
        if (currentUserId == null)
            return 0;

        return await entryService.CountEntriesByUserAndDraw(currentUserId, drawId);
    }

    public async Task<int> CountCurrentUserEntriesRemainingByDraw(int drawId)
    {
        if (await userService.IsGuest())
            return 1;
        
        var draw = await drawService.GetDrawById(drawId);
        return draw == null ? 0 
            : Math.Min(draw.MaxEntriesPerUser - await CountCurrentUserEntriesByDraw(drawId), 
                await CountEntriesRemainingByDraw(drawId));
    }

    public async Task<int> GetPercentageEntriesRemainingByDraw(int drawId)
    {
        var draw = await drawService.GetDrawById(drawId);
        return draw == null ? 0 
            : (int)Math.Round(100 - (float) await CountEntriesByDraw(drawId) / draw.MaxEntriesTotal * 100);
    }

    public async Task<List<int>> GetAvailableLuckyNumbersByDraw(int drawId)
    {
        var draw = await drawService.GetDrawById(drawId);
        if (draw == null)
            return [];
        
        var availableLuckyNumbers = Enumerable.Range(1, draw.MaxEntriesTotal).ToList();
        
        if (draw is not { DrawType: DrawTypeEnum.Lottery })
            return availableLuckyNumbers;

        var entries = await entryService.GetEntriesByDraw(drawId);
        if (entries == null)
            return availableLuckyNumbers;
        
        foreach (var luckyNumber in entries.Select(entry => entry.LuckyNumber)
                     .Where(luckyNumber => availableLuckyNumbers.Contains((int)luckyNumber!)))
        {
            availableLuckyNumbers.Remove((int) luckyNumber!);
        }

        return availableLuckyNumbers;
    }

    public async Task<List<DrawModel>?> GetDrawsEnteredByCurrentUser()
    {
        var currentUserId = await userService.GetUserId();
        if (currentUserId == null)
            return null;

        var userEntries = await entryService.GetEntriesByUser(currentUserId);
        if (userEntries == null)
            return null;
        
        var drawIds = userEntries.Select(entry => entry.DrawId).Distinct().ToList();
        return await drawService.GetDrawsByIds(drawIds);
    }

    public async Task<bool> IsCurrentUserParticipatingInDraw(int drawId)
    {
        return await CountCurrentUserEntriesByDraw(drawId) > 0;
    }

    public async Task<bool> IsGuestParticipatingInDraw(int drawId, string guestEmail)
    {
        var entries = await GetEntriesByDraw(drawId);
        return entries != null && entries.Any(entry => entry.GuestEmail == guestEmail);
    }

    public async Task<List<WinnerModel>?> GetRecentWinners(int numberOfWinners)
    {
        return await winnerService.GetRecentWinners(numberOfWinners);
    }

    public async Task DeleteEntry(int entryId)
    {
        var entry = await entryService.GetEntryById(entryId);

        if (entry is { IsGuest: true })
        {
            Console.WriteLine("\n\n\n\n\n");
            Console.WriteLine("DELETING WINNER");
            Console.WriteLine("\n\n\n\n\n");
            await winnerService.DeleteWinnerByEntry(entryId);
        }
        
        Console.WriteLine("\n\n\n\n\n");
        Console.WriteLine("DELETING ENTRY");
        Console.WriteLine("\n\n\n\n\n");
        await entryService.DeleteEntry(entryId);
    }
}