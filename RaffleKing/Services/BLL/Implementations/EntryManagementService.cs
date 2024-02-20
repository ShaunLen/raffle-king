using RaffleKing.Common;
using RaffleKing.Data.Models;
using RaffleKing.Services.BLL.Interfaces;
using RaffleKing.Services.DAL.Interfaces;

namespace RaffleKing.Services.BLL.Implementations;

public class EntryManagementService(IUserService userService, IEntryService entryService, IDrawService drawService)
    : IEntryManagementService
{
    public async Task<OperationResult> TryEnterRaffle(int drawId, int numberOfEntries)
    {
        throw new NotImplementedException();
    }

    public async Task<OperationResult> TryEnterLottery(int drawId, IEnumerable<int> luckyNumbers)
    {
        throw new NotImplementedException();
    }

    public async Task<OperationResult> CanEnterDraw(int drawId, IEnumerable<int> luckyNumbers)
    {
        throw new NotImplementedException();
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
}