using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using RaffleKing.Data;
using RaffleKing.Data.Models;
using RaffleKing.Services.Interfaces;

namespace RaffleKing.Services;

public class DrawService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : IDrawService
{
    /* Create Operations */
    public async Task AddNewDraw(DrawModel drawModel)
    {
        context.Draws.Add(drawModel);
        await context.SaveChangesAsync();
    }

    /* Read Operations */
    public async Task<List<DrawModel>?> GetAllDraws()
    {
        return await context.Draws.ToListAsync();
    }

    public async Task<List<DrawModel>?> GetHostedDraws()
    {
        var hostId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (hostId == null)
            return null;

        var draws = await context.Draws
            .Where(draw => draw.DrawHostId == hostId)
            .ToListAsync();

        return draws;
    }

    /* Update Operations */
    public async Task UpdateDraw(DrawModel drawModel)
    {
        context.Draws.Update(drawModel);
        await context.SaveChangesAsync();
    }

    public async Task ActivateDraw(int drawId)
    {
        var draw = await context.Draws.FindAsync(drawId);
        if (draw != null)
        {
            draw.IsActive = true;
            await context.SaveChangesAsync();
        }
    }

    public async Task DeactivateDraw(int drawId)
    {
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
        var draw = await context.Draws.FindAsync(drawId);
        if (draw != null)
        {
            context.Draws.Remove(draw);
            await context.SaveChangesAsync();
        }
    }
}