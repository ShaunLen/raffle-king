using RaffleKing.Data.Models;

namespace RaffleKing.Services.BLL.Interfaces;

public interface IPrizeManagementService
{
    Task AddNewPrize(PrizeModel prizeModel);
    Task<List<PrizeModel>?> GetPrizesByDraw(int drawId);
    Task DeletePrize(int prizeId);
}