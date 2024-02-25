using RaffleKing.Data.Models;
using RaffleKing.Services.BLL.Interfaces;
using RaffleKing.Services.DAL.Interfaces;

namespace RaffleKing.Services.BLL.Implementations;

public class DrawExecutionService(IDrawService drawService, IEntryService entryService, IPrizeService prizeService, 
    IWinnerService winnerService) : IDrawExecutionService
{
    public async Task ScheduleDrawExecution(int drawId)
    {
        throw new NotImplementedException();
    }

    public async Task ExecuteDraw(int drawId)
    {
        var draw = await drawService.GetDrawById(drawId);
        if (draw == null)
            return;
        
        var prizes = await prizeService.GetPrizesByDraw(drawId);
        if (prizes == null)
            return;

        await drawService.DeactivateDraw(drawId);

        var entries = await entryService.GetEntriesByDraw(drawId);
        if (entries == null)
            return;

        switch (draw.DrawType)
        {
            case DrawTypeEnum.Raffle:
                if(draw.IsBundle)
                    SelectWinnerForRaffleBundle(prizes, entries);
                else
                    SelectWinnersForRaffle(prizes, entries);
                return;
            case DrawTypeEnum.Lottery:
                if(draw.IsBundle)
                    DrawLuckyNumberForLotteryBundle(draw, prizes, entries);
                else
                    DrawLuckyNumbersForLottery(draw, prizes, entries);
                return;
            default:
                return;
        }
    }

    public void SelectWinnersForRaffle(List<PrizeModel> prizes, List<EntryModel> entries)
    {
        var random = new Random();
        foreach (var prize in prizes)
        {
            // If there are more prizes but no more entries, there will be no winner for those prizes
            if (entries.Count == 0)
                return;
            
            for (var i = 0; i < prize.Quantity; i++)
            {
                var index = random.Next(entries.Count);

                var winner = new WinnerModel
                {
                    EntryId = entries[index].Id,
                    PrizeId = prize.Id
                };

                winnerService.AddWinner(winner);
                // Ensure an entry cannot be drawn twice
                entries.RemoveAt(index);
            }
        }
    }

    public void SelectWinnerForRaffleBundle(List<PrizeModel> prizes, List<EntryModel> entries)
    {
        var random = new Random();
        var index = random.Next(entries.Count);

        foreach (var prize in prizes)
        {
            var winner = new WinnerModel
            {
                EntryId = entries[index].Id,
                PrizeId = prize.Id
            };

            winnerService.AddWinner(winner);
        }
    }

    public void DrawLuckyNumbersForLottery(DrawModel draw, List<PrizeModel> prizes, List<EntryModel> entries)
    {
        var random = new Random();
        EntryModel? winningEntry = null;
        var availableLuckyNumbers = Enumerable.Range(1, draw.MaxEntriesTotal).ToList();
        
        foreach (var prize in prizes)
        {
            // For this prize, draw a lucky number and check if any entry has it
            var index = random.Next(availableLuckyNumbers.Count);
            foreach (var entry in entries.Where(entry => entry.LuckyNumber == availableLuckyNumbers[index]))
                winningEntry = entry;

            if (winningEntry == null)
                return;

            var winner = new WinnerModel
            {
                EntryId = winningEntry.Id,
                PrizeId = prize.Id
            };

            winnerService.AddWinner(winner);
            // Ensure a lucky number cannot be drawn twice
            availableLuckyNumbers.RemoveAt(index);
        }
    }

    public void DrawLuckyNumberForLotteryBundle(DrawModel draw, List<PrizeModel> prizes, List<EntryModel> entries)
    {
        var random = new Random();
        var winningLuckyNumber = random.Next(draw.MaxEntriesTotal);
        EntryModel? winningEntry = null;

        // If an entry has this lucky number, it's the winning entry
        foreach (var entry in entries.Where(entry => entry.LuckyNumber == winningLuckyNumber))
            winningEntry = entry;

        if (winningEntry == null)
            return;
        
        foreach (var prize in prizes)
        {
            var winner = new WinnerModel
            {
                EntryId = winningEntry.Id,
                PrizeId = prize.Id
            };

            winnerService.AddWinner(winner);
        }
    }
}