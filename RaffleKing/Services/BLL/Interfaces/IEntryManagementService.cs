using RaffleKing.Common;
using RaffleKing.Data.Models;

namespace RaffleKing.Services.BLL.Interfaces;

public interface IEntryManagementService
{
    Task<OperationResult> TryEnterRaffle(int drawId, int numberOfEntries, string guestEmail = "");
    Task<OperationResult> TryEnterLottery(int drawId, IEnumerable<int> luckyNumbers);
    Task<OperationResult> CurrentUserCanEnterDraw(int drawId);
    Task<List<EntryModel>?> GetEntriesByDraw(int drawId);
    Task<List<EntryModel>?> GetCurrentUserEntries();
    Task<List<EntryModel>?> GetCurrentUserEntriesByDraw(int drawId);
    Task<EntryModel?> GetGuestEntry(string guestRef);
    Task RemoveEntriesByDraw(int drawId);
    Task RemoveCurrentUserEntriesByDraw(int drawId);
    Task<int> CountEntriesByDraw(int drawId);
    Task<int> CountEntriesRemainingByDraw(int drawId);
    Task<int> CountCurrentUserEntriesByDraw(int drawId);
    Task<int> CountCurrentUserEntriesRemainingByDraw(int drawId);
    Task<int> GetPercentageEntriesRemainingByDraw(int drawId);
    Task<List<int>> GetAvailableLuckyNumbersByDraw(int drawId);
    Task<List<DrawModel>?> GetDrawsEnteredByCurrentUser();
    Task<bool> IsCurrentUserParticipatingInDraw(int drawId);
    Task<bool> IsGuestParticipatingInDraw(int drawId, string guestEmail);
}
