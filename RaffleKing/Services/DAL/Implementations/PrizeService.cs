using Microsoft.EntityFrameworkCore;
using RaffleKing.Data;
using RaffleKing.Data.Models;
using RaffleKing.Services.DAL.Interfaces;

namespace RaffleKing.Services.DAL.Implementations;

public class PrizeService(IDbContextFactory<ApplicationDbContext> factory, IHttpContextAccessor httpContextAccessor) : IPrizeService
{
    /* Create Operations */
    public async Task AddNewPrize(PrizeModel prizeModel)
    {
        await using var context = await factory.CreateDbContextAsync();
        context.Prizes.Add(prizeModel);
        await context.SaveChangesAsync();
    }

    /* Read Operations */
    public async Task<PrizeModel?> GetPrizeById(int prizeId)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Prizes.FirstOrDefaultAsync(prize => prize.Id == prizeId);
    }
    
    public async Task<List<PrizeModel>?> GetPrizesForDraw(int drawId)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Prizes.Where(prize => prize.DrawId == drawId).ToListAsync();
    }

    /* Delete Operations */
    public async Task DeletePrize(int prizeId)
    {
        await using var context = await factory.CreateDbContextAsync();
        var prize = await context.Prizes.FindAsync(prizeId);
        if (prize != null)
        {
            context.Prizes.Remove(prize);
            await context.SaveChangesAsync();
        }
    }
}