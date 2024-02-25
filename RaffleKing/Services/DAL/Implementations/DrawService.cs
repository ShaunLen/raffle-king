using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using RaffleKing.Data;
using RaffleKing.Data.Models;
using RaffleKing.Services.DAL.Interfaces;

namespace RaffleKing.Services.DAL.Implementations;

public class DrawService(IDbContextFactory<ApplicationDbContext> factory, IHttpContextAccessor httpContextAccessor) 
    : IDrawService
{
    /* Create Operations */
    public async Task<int> AddNewDraw(DrawModel drawModel)
    {
        await using var context = await factory.CreateDbContextAsync();
        context.Draws.Add(drawModel);
        await context.SaveChangesAsync();
        return drawModel.Id;
    }

    /* Read Operations */
    public async Task<DrawModel?> GetDrawById(int drawId)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Draws.FirstOrDefaultAsync(draw => draw.Id == drawId);
    }

    public async Task<List<DrawModel>?> GetDrawsByIds(List<int> drawIds)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Draws
            .Where(draw => drawIds.Contains(draw.Id))
            .ToListAsync();
    }

    public async Task<List<DrawModel>?> GetAllDraws()
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Draws.ToListAsync();
    }
    
    public async Task<List<DrawModel>?> GetDrawsByHostId(string hostUserId)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Draws
            .Where(draw => draw.DrawHostId == hostUserId)
            .ToListAsync();
    }

    public async Task<List<DrawModel>?> GetActiveDraws()
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Draws.Where(draw => draw.IsPublished && !draw.IsFinished).ToListAsync();
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
            draw.IsPublished = true;
            await context.SaveChangesAsync();
        }
    }

    public async Task DeactivateDraw(int drawId)
    {
        await using var context = await factory.CreateDbContextAsync();
        var draw = await context.Draws.FindAsync(drawId);
        if (draw != null)
        {
            draw.IsFinished = true;
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