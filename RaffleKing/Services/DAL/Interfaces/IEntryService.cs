using RaffleKing.Data.Models;

namespace RaffleKing.Services.DAL.Interfaces;

public interface IEntryService
{
    /* Create Operations */
    public Task AddEntry(EntryModel entryModel);
    
    /* Read Operations */
    public Task<List<EntryModel>?> GetAllEntries();
    public Task<List<EntryModel>?> GetEntriesByUser(string userId);
    public Task<List<EntryModel>?> GetEntriesByDraw(int drawId);
    public Task<List<EntryModel>?> GetEntriesByUserAndDraw(string userId, int drawId);
    public Task<EntryModel?> GetEntryByGuestRef(string guestRef);
    
    /* Update Operations */
    public Task UpdateEntry(EntryModel entryModel);
    public Task<int> CountEntriesByDraw(int drawId);
    public Task<int> CountEntriesByUserAndDraw(string userId, int drawId);

    /* Delete Operations */
    public Task DeleteEntry(int entryId);
    public Task DeleteEntriesByUser(string userId);
    public Task DeleteEntriesByDraw(int drawId);
    public Task DeleteEntriesByUserAndDraw(string userId, int drawId);
    public Task DeleteEntryByGuestRef(string guestRef);
}