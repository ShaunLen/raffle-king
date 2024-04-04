using Microsoft.EntityFrameworkCore;
using RaffleKing.Data;
using RaffleKing.Data.Models;
using RaffleKing.Services.BLL.Interfaces;
using RaffleKing.Services.DAL.Interfaces;

namespace RaffleKing.Services.DAL.Implementations;

public class WinnerService(IDbContextFactory<ApplicationDbContext> factory, IHttpContextAccessor httpContextAccessor,
    IEntryService entryService, IEmailService emailService) : IWinnerService
{
    public async Task AddWinner(WinnerModel winnerModel)
    {
        await using var context = await factory.CreateDbContextAsync();
        context.Winners.Add(winnerModel);
        await context.SaveChangesAsync();
        
        // Send email to winner below
        if(winnerModel.EntryId == null)
            return;
        
        var entryId = (int) winnerModel.EntryId;
        var entry = await entryService.GetEntryById(entryId);

        if (entry == null)
            return;
        
        if (entry.IsGuest)
        {
            // Suppressed nullable warning as I know GuestEmail will not be null for a guest entry at this point
            emailService.SendGuestWinnerEmail(entry.GuestEmail!);
        }
        else
        {
            var user = entry.User;
            if(user == null)
                return;

            Console.WriteLine();
            
            // Suppressed nullable warning as I know that valid users will have a non-null Email
            emailService.SendUserWinnerEmail(user.Email!);
        }
    }

    public async Task<List<WinnerModel>?> GetWinnersByDraw(int drawId)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Winners
            .Where(winner => winner.Entry != null && winner.Entry.DrawId == drawId)
            .ToListAsync();
    }

    public async Task<List<WinnerModel>?> GetRecentWinners(int numberOfWinners)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Winners
            .OrderByDescending(winner => winner.Id)
            .Take(numberOfWinners)
            .ToListAsync();
    }

    public async Task<List<WinnerModel>?> GetUnclaimedPrizesByUser(string userId)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Winners
            .Where(winner => winner.Entry != null && winner.Entry.UserId == userId && !winner.IsClaimed)
            .ToListAsync();
    }

    public async Task SetClaimed(int winnerId)
    {
        await using var context = await factory.CreateDbContextAsync();
        var winner = await context.Winners.FindAsync(winnerId);
        if (winner != null)
        {
            winner.IsClaimed = true;
            await context.SaveChangesAsync();
        }
    }
}