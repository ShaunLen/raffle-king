using Microsoft.EntityFrameworkCore;
using RaffleKing.Data;
using RaffleKing.Data.Models;
using RaffleKing.Services.DAL.Interfaces;

namespace RaffleKing.Services.DAL.Implementations;

public class WinnerService(IDbContextFactory<ApplicationDbContext> factory, IHttpContextAccessor httpContextAccessor) 
    : IWinnerService
{
    public async Task AddWinner(WinnerModel winnerModel)
    {
        await using var context = await factory.CreateDbContextAsync();
        context.Winners.Add(winnerModel);
        await context.SaveChangesAsync();
    }
}