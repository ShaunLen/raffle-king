using RaffleKing.Common;
using RaffleKing.Data.Models;

namespace RaffleKing.Services.BLL.Interfaces;

public interface IDrawExecutionService
{
    OperationResult ScheduleDrawExecution(int drawId, DateTime scheduleTime);
    Task ExecuteDraw(int drawId);
    void SelectWinnersForRaffle(List<PrizeModel> prizes, List<EntryModel> entries);
    void SelectWinnerForRaffleBundle(List<PrizeModel> prizes, List<EntryModel> entries);
    void DrawLuckyNumbersForLottery(DrawModel draw, List<PrizeModel> prizes, List<EntryModel> entries);
    void DrawLuckyNumberForLotteryBundle(DrawModel draw, List<PrizeModel> prizes, List<EntryModel> entries);
}