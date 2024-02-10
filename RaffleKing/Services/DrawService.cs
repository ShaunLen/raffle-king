using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using RaffleKing.Data;
using RaffleKing.Data.Models;
using RaffleKing.Services.Interfaces;

namespace RaffleKing.Services;

public class DrawService(IDbContextFactory<ApplicationDbContext> factory, IHttpContextAccessor httpContextAccessor) : IDrawService
{
    /* Create Operations */
    public async Task AddNewDraw(DrawModel drawModel)
    {
        await using var context = await factory.CreateDbContextAsync();
        context.Draws.Add(drawModel);
        await context.SaveChangesAsync();
    }

    /* Read Operations */
    public async Task<DrawModel?> GetDrawById(int drawId)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Draws.FirstOrDefaultAsync(draw => draw.Id == drawId);
    }

    public async Task<List<DrawModel>?> GetAllDraws()
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Draws.ToListAsync();
    }

    public async Task<List<DrawModel>?> GetHostedDraws()
    {
        await using var context = await factory.CreateDbContextAsync();
        var hostId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (hostId == null)
            return null;

        var draws = await context.Draws
            .Where(draw => draw.DrawHostId == hostId)
            .ToListAsync();

        return draws;
    }

    public async Task<List<DrawModel>?> GetActiveDraws()
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Draws.Where(draw => draw.IsActive).ToListAsync();
    }

    /* Update Operations */
    public async Task UpdateDraw(DrawModel drawModel)
    {
        await using var context = await factory.CreateDbContextAsync();
        context.Draws.Update(drawModel);
        await context.SaveChangesAsync();
    }

    public async Task ActivateDraw(int drawId)
    {
        await using var context = await factory.CreateDbContextAsync();
        var draw = await context.Draws.FindAsync(drawId);
        if (draw != null)
        {
            draw.IsActive = true;
            await context.SaveChangesAsync();
        }
    }

    public async Task DeactivateDraw(int drawId)
    {
        await using var context = await factory.CreateDbContextAsync();
        var draw = await context.Draws.FindAsync(drawId);
        if (draw != null)
        {
            draw.IsActive = false;
            await context.SaveChangesAsync();
        }
    }

    /* Delete Operations */
    public async Task DeleteDraw(int drawId)
    {
        await using var context = await factory.CreateDbContextAsync();
        var draw = await context.Draws.FindAsync(drawId);
        if (draw != null)
        {
            context.Draws.Remove(draw);
            await context.SaveChangesAsync();
        }
    }
}