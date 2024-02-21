using RaffleKing.Common;
using RaffleKing.Data.Models;
using RaffleKing.Services.BLL.Interfaces;
using RaffleKing.Services.DAL.Interfaces;

namespace RaffleKing.Services.BLL.Implementations;

public class DrawManagementService(IDrawService drawService, IPrizeManagementService prizeManagementService,
    IPrizeService prizeService, IEntryService entryService) 
    : IDrawManagementService
{
    public async Task<DrawModel?> GetDrawById(int drawId)
    {
        return await drawService.GetDrawById(drawId);
    }

    public async Task<OperationResult> ActivateDraw(int drawId)
    {
        var prizeCount = await prizeService.CountPrizesByDraw(drawId);
        if(prizeCount == 0)
            return OperationResult.Fail("Cannot publish a draw that has no prizes.");

        await drawService.ActivateDraw(drawId);
        return OperationResult.Ok();
    }

    public async Task DeleteDraw(int drawId)
    {
        await prizeService.DeletePrizesByDraw(drawId);
        await entryService.DeleteEntriesByDraw(drawId);
        await drawService.DeleteDraw(drawId);
    }
}