using RaffleKing.Data.Models;

namespace RaffleKing.Services.DAL.Interfaces;

public interface IPrizeService
{
    /* Create Operations */
    public Task AddNewPrize(PrizeModel prizeModel);
    
    /* Read Operations */
    public Task<PrizeModel?> GetPrizeById(int prizeId);
    public Task<List<PrizeModel>?> GetPrizesByDraw(int drawId);
    public Task<int> CountPrizesByDraw(int drawId, bool includeQty = true);
    
    /* Delete Operations */
    public Task DeletePrize(int prizeId);
    public Task DeletePrizesByDraw(int drawId);
}