using RaffleKing.Data.Models;

namespace RaffleKing.Services.DAL.Interfaces;

public interface IDrawService
{
    /* Create Operations */
    public Task<int> AddNewDraw(DrawModel drawModel);
    
    /* Read Operations */
    public Task<DrawModel?> GetDrawById(int drawId);
    public Task<List<DrawModel>?> GetAllDraws();
    public Task<List<DrawModel>?> GetHostedDraws();
    public Task<List<DrawModel>?> GetActiveDraws();
    
    /* Update Operations */
    public Task UpdateDraw(DrawModel drawModel);
    public Task ActivateDraw(int drawId);
    public Task DeactivateDraw(int drawId);
    
    /* Delete Operations */
    public Task DeleteDraw(int drawId);
}