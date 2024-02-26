using RaffleKing.Data.Models;

namespace RaffleKing.Services.BLL.Interfaces;

public interface IPrizeManagementService
{
    Task AddNewPrize(PrizeModel prizeModel);
    Task<PrizeModel?> GetPrizeById(int prizeId);
    Task<List<PrizeModel>?> GetPrizesByDraw(int drawId);
    Task DeletePrize(int prizeId);
}