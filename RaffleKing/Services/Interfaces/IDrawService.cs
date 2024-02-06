using RaffleKing.Data.Models;

namespace RaffleKing.Services.Interfaces;

public interface IDrawService
{
    /* Create Operations */
    public Task AddNewDraw(DrawModel drawModel);
    
    /* Read Operations */
    public Task<List<DrawModel>?> GetAllDraws();
    
    /* Update Operations */
    public Task UpdateDraw(DrawModel drawModel);
    public Task ActivateDraw(int drawId);
    public Task DeactivateDraw(int drawId);
    
    /* Delete Operations */
    public Task DeleteDraw(int drawId);
}