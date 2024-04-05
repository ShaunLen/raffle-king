using RaffleKing.Data.Models;

namespace RaffleKing.Services.DAL.Interfaces;

public interface IEntryService
{
    /* Create Operations */
    public Task AddEntry(EntryModel entryModel);
    
    /* Read Operations */
    public Task<EntryModel?> GetEntryById(int entryId);
    public Task<List<EntryModel>?> GetAllEntries();
    public Task<List<EntryModel>?> GetEntriesByUser(string userId);
    public Task<List<EntryModel>?> GetEntriesByDraw(int drawId);
    public Task<List<EntryModel>?> GetEntriesByUserAndDraw(string userId, int drawId);
    public Task<EntryModel?> GetEntryByGuestRef(string guestRef);
    public Task<int> CountEntriesByDraw(int drawId);
    public Task<int> CountEntriesByUserAndDraw(string userId, int drawId);
    
    /* Update Operations */
    public Task UpdateEntry(EntryModel entryModel);

    /* Delete Operations */
    public Task DeleteEntry(int entryId);
    public Task DeleteEntriesByUser(string userId);
    public Task DeleteEntriesByDraw(int drawId);
    public Task DeleteEntriesByUserAndDraw(string userId, int drawId);
    public Task DeleteEntryByGuestRef(string guestRef);
}