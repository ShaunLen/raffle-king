using RaffleKing.Data.Models;

namespace RaffleKing.Services.DAL.Interfaces;

public interface IEntryService
{
    /* Create Operations */
    public Task AddEntry(EntryModel entryModel);
    
    /* Read Operations */
    public Task<List<EntryModel>?> GetEntriesForDraw(int drawId);
    public Task<List<EntryModel>?> GetEntriesForDrawByUser(int drawId);
    public Task<List<EntryModel>?> GetEntriesForDrawByGuest(int drawId, string guestRef);
    
    /* Update Operations */
    public Task SelectWinnersForDraw(int drawId);
    public Task RemoveGuestInformation(int entryId);

    /* Delete Operations */
    public Task DeleteEntry(int entryId);
}