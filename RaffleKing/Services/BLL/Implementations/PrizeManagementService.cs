using RaffleKing.Data.Models;
using RaffleKing.Services.BLL.Interfaces;
using RaffleKing.Services.DAL.Interfaces;

namespace RaffleKing.Services.BLL.Implementations;

public class PrizeManagementService(IPrizeService prizeService)
    : IPrizeManagementService
{
    public async Task AddNewPrize(PrizeModel prizeModel)
    {
        await prizeService.AddNewPrize(prizeModel);
    }

    public async Task<PrizeModel?> GetPrizeById(int prizeId)
    {
        return await prizeService.GetPrizeById(prizeId);
    }

    public async Task<List<PrizeModel>?> GetPrizesByDraw(int drawId)
    {
        return await prizeService.GetPrizesByDraw(drawId);
    }

    public async Task DeletePrize(int prizeId)
    {
        await prizeService.DeletePrize(prizeId);
    }
}