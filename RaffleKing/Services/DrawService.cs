using RaffleKing.Data;
using RaffleKing.Data.Models;
using RaffleKing.Services.Interfaces;

namespace RaffleKing.Services;

public class DrawService(ApplicationDbContext context) : IDrawService
{
    /* Create Operations */
    public Task AddNewDraw(DrawModel drawModel)
    {
        throw new NotImplementedException();
    }

    /* Read Operations */
    public Task<List<DrawModel>?> GetAllDraws()
    {
        throw new NotImplementedException();
    }

    /* Update Operations */
    public Task UpdateDraw(DrawModel drawModel)
    {
        throw new NotImplementedException();
    }

    public Task ActivateDraw(int drawId)
    {
        throw new NotImplementedException();
    }

    public Task DeactivateDraw(int drawId)
    {
        throw new NotImplementedException();
    }

    /* Delete Operations */
    public Task DeleteDraw(int drawId)
    {
        throw new NotImplementedException();
    }
}