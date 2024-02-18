using Microsoft.EntityFrameworkCore;
using RaffleKing.Data;
using RaffleKing.Data.Models;
using RaffleKing.Services.DAL.Interfaces;

namespace RaffleKing.Services.DAL.Implementations;

public class EntryService(IDbContextFactory<ApplicationDbContext> factory, IHttpContextAccessor httpContextAccessor) 
    : IEntryService
{
    /* Create Operations */
    public async Task AddEntry(EntryModel entryModel)
    {
        await using var context = await factory.CreateDbContextAsync();
        context.Entries.Add(entryModel);
        await context.SaveChangesAsync();
    }

    /* Read Operations */
    public async Task<List<EntryModel>?> GetAllEntries()
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Entries.ToListAsync();
    }

    public async Task<List<EntryModel>?> GetEntriesByUser(string userId)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Entries
            .Where(entry => entry.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<EntryModel>?> GetEntriesByDraw(int drawId)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Entries
            .Where(entry => entry.DrawId == drawId)
            .ToListAsync();
    }

    public async Task<List<EntryModel>?> GetEntriesByUserAndDraw(string userId, int drawId)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Entries
            .Where(entry => entry.UserId == userId && entry.DrawId == drawId)
            .ToListAsync();
    }

    public async Task<EntryModel?> GetEntryByGuestRef(string guestRef)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Entries.FirstOrDefaultAsync(entry => entry.GuestReferenceCode == guestRef);
    }

    /* Update Operations */
    public async Task UpdateEntry(EntryModel entryModel)
    {
        await using var context = await factory.CreateDbContextAsync();
        context.Entries.Update(entryModel);
        await context.SaveChangesAsync();
    }

    public async Task<int> CountEntriesByDraw(int drawId)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Entries
            .CountAsync(entry => entry.DrawId == drawId);
    }

    public async Task<int> CountEntriesByUserAndDraw(string userId, int drawId)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Entries
            .CountAsync(entry => entry.UserId == userId && entry.DrawId == drawId);
    }

    /* Delete Operations */
    public async Task DeleteEntry(int entryId)
    {
        await using var context = await factory.CreateDbContextAsync();
        var entry = await context.Entries.FindAsync(entryId);
        if (entry != null)
        {
            context.Entries.Remove(entry);
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteEntriesByUser(string userId)
    {
        await using var context = await factory.CreateDbContextAsync();
        var entries = await GetEntriesByUser(userId);
        if (entries != null)
        {
            context.Entries.RemoveRange(entries);
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteEntriesByUserAndDraw(string userId, int drawId)
    {
        await using var context = await factory.CreateDbContextAsync();
        var entries = await GetEntriesByUserAndDraw(userId, drawId);
        if (entries != null)
        {
            context.Entries.RemoveRange(entries);
            await context.SaveChangesAsync();
        }
    }
}