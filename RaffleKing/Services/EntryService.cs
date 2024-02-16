using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using RaffleKing.Data;
using RaffleKing.Data.Models;
using RaffleKing.Services.Interfaces;

namespace RaffleKing.Services;

public class EntryService(IDbContextFactory<ApplicationDbContext> factory, IHttpContextAccessor httpContextAccessor) : IEntryService
{
    /* Create Operations */
    public async Task AddEntry(EntryModel entryModel)
    {
        await using var context = await factory.CreateDbContextAsync();
        context.Entries.Add(entryModel);
        await context.SaveChangesAsync();
    }

    /* Read Operations */
    public async Task<List<EntryModel>?> GetEntriesForDraw(int drawId)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Entries
            .Where(entry => entry.DrawId == drawId)
            .ToListAsync();
    }

    public async Task<List<EntryModel>?> GetEntriesForDrawByUser(int drawId)
    {
        await using var context = await factory.CreateDbContextAsync();
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return null;

        return await context.Entries
            .Where(entry => entry.DrawId == drawId && entry.UserId == userId)
            .ToListAsync();
    }
    
    public async Task<List<EntryModel>?> GetEntriesForDrawByGuest(int drawId, string guestRef)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Entries
            .Where(entry => entry.DrawId == drawId && entry.GuestReferenceCode == guestRef)
            .ToListAsync();
    }

    /* Update Operations */
    public async Task SelectWinnersForDraw(int drawId)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveGuestInformation(int entryId)
    {
        throw new NotImplementedException();
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
}