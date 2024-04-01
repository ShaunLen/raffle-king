using RaffleKing.Common;
using RaffleKing.Data.Models;
using RaffleKing.Services.BLL.Interfaces;
using RaffleKing.Services.DAL.Interfaces;

namespace RaffleKing.Services.BLL.Implementations;

public class DrawManagementService(IDrawService drawService, IPrizeService prizeService, IEntryService entryService, 
    IUserService userService, IWinnerService winnerService, IDrawExecutionService drawExecutionService) 
    : IDrawManagementService
{
    public async Task<OperationResult<int>> AddNewDraw(DrawModel draw)
    {
        if(draw.Title.Length > 30)
            return OperationResult<int>.Fail("Title cannot exceed 30 characters.", 0);
        
        if(draw.Description.Length > 500)
            return OperationResult<int>.Fail("Description cannot exceed 500 characters.", 0);

        if (draw.MaxEntriesPerUser > draw.MaxEntriesTotal)
            return OperationResult<int>.Fail("Max Entries Per User cannot exceed Max Entries Total.", 0);

        if (draw.DrawDate < DateTime.Now)
            return OperationResult<int>.Fail("Draw must be scheduled for some time in the future.", 0);
        
        var drawId = await drawService.AddNewDraw(draw);
        return OperationResult<int>.Ok(drawId);
    }

    public async Task<DrawModel?> GetDrawById(int drawId)
    {
        return await drawService.GetDrawById(drawId);
    }

    public async Task<List<WinnerModel>?> GetWinnersByDraw(int drawId)
    {
        return await winnerService.GetWinnersByDraw(drawId);
    }

    public async Task<List<DrawModel>?> GetActiveDraws()
    {
        return await drawService.GetActiveDraws();
    }

    public async Task<List<DrawModel>?> GetDrawsHostedByCurrentUser()
    {
        var userId = await userService.GetUserId();
        if (userId == null)
            return null;
        
        return await drawService.GetDrawsByHostId(userId);
    }

    public async Task<OperationResult> ActivateDraw(int drawId)
    {
        var prizeCount = await prizeService.CountPrizesByDraw(drawId);
        if(prizeCount == 0)
            return OperationResult.Fail("Cannot publish a draw that has no prizes.");

        var draw = await drawService.GetDrawById(drawId);
        if (draw == null)
            return OperationResult.Fail("Invalid draw.");;

        await drawService.ActivateDraw(drawId);
        drawExecutionService.ScheduleDrawExecution(drawId, draw.DrawDate);
        return OperationResult.Ok();
    }

    public async Task DeleteDraw(int drawId)
    {
        await prizeService.DeletePrizesByDraw(drawId);
        await entryService.DeleteEntriesByDraw(drawId);
        await drawService.DeleteDraw(drawId);
    }
}