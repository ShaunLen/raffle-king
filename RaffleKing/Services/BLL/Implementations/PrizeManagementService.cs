using RaffleKing.Data.Models;
using RaffleKing.Services.BLL.Interfaces;
using RaffleKing.Services.DAL.Interfaces;

namespace RaffleKing.Services.BLL.Implementations;

public class PrizeManagementService(IPrizeService prizeService, IWinnerService winnerService, IUserService userService)
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

    public async Task<List<WinnerModel>?> GetUnclaimedPrizesByCurrentUser()
    {
        var currentUserId = await userService.GetUserId();
        if (currentUserId == null)
            return null;

        return await winnerService.GetUnclaimedPrizesByUser(currentUserId);
    }

    public async Task ClaimPrize(int winnerId)
    {
        await winnerService.SetClaimed(winnerId);
    }

    public async Task DeletePrize(int prizeId)
    {
        await prizeService.DeletePrize(prizeId);
    }
}