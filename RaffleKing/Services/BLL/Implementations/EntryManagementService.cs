using RaffleKing.Common;
using RaffleKing.Data.Models;
using RaffleKing.Services.BLL.Interfaces;
using RaffleKing.Services.DAL.Interfaces;

namespace RaffleKing.Services.BLL.Implementations;

public class EntryManagementService(IUserService userService, IEntryService entryService, IDrawService drawService)
    : IEntryManagementService
{
    public async Task<OperationResult> TryEnterRaffle(int drawId, int numberOfEntries, string guestEmail = "")
    {
        var canEnter = await CurrentUserCanEnterDraw(drawId);
        var isGuest = await userService.IsGuest();

        if (!canEnter.Success)
            return canEnter;

        if (numberOfEntries > await CountCurrentUserEntriesRemainingByDraw(drawId))
            return OperationResult.Fail("Selected number of entries exceeds available entries!");
        
        if(isGuest && numberOfEntries > 1)
            return OperationResult.Fail("Guests can only enter each raffle once! Create an account for more entries.");
        
        if(isGuest && string.IsNullOrWhiteSpace(guestEmail))
            return OperationResult.Fail("An email must be supplied to enter as a guest.");

        if (isGuest && await IsGuestParticipatingInDraw(drawId, guestEmail))
            return OperationResult.Fail("You are already participating in this draw as a guest! Create an account " +
                                        "for more entries.");
        
        List<EntryModel> entries = [];

        if (isGuest)
        {
            entries.Add(new EntryModel
            {
                DrawId = drawId,
                IsGuest = true,
                GuestEmail = guestEmail,
                GuestReferenceCode = Guid.NewGuid().ToString("N")
            });
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
        
        return OperationResult.Ok();
    }

    public async Task<OperationResult> TryEnterLottery(int drawId, IEnumerable<int> luckyNumbers)
    {
        var canEnter = await CurrentUserCanEnterDraw(drawId);

        if (!canEnter.Success)
            return canEnter;

        luckyNumbers = luckyNumbers.ToList();
        if (luckyNumbers.Count() > await CountCurrentUserEntriesRemainingByDraw(drawId))
            return OperationResult.Fail("Selected number of entries exceeds available entries!");

        var availableLuckyNumbers = await GetAvailableLuckyNumbersByDraw(drawId);
        var unavailableNumbers = 
            luckyNumbers.Where(luckyNumber => !availableLuckyNumbers.Contains(luckyNumber)).ToList();
        
        if (unavailableNumbers.Count != 0)
            return OperationResult.Fail($"Lucky Number(s) {string.Join(", ", unavailableNumbers)} is/are " +
                                        $"not available!");
        
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
        
        return OperationResult.Ok();
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
        var availableLuckyNumbers = new List<int>();
        var draw = await drawService.GetDrawById(drawId);
        
        if (draw is not { DrawType: DrawTypeEnum.Lottery })
            return availableLuckyNumbers;
        
        // TODO: Add actual handling
        for (var i = 1; i < 100; i++)
        {
            // Temporarily just add max range of lucky numbers until lottery entry logic is added
            availableLuckyNumbers.Add(i);
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
}