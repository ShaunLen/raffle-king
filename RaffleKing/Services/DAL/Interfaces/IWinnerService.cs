using RaffleKing.Data.Models;

namespace RaffleKing.Services.DAL.Interfaces;

public interface IWinnerService
{
    /* Create Operations */
    public Task AddWinner(WinnerModel winnerModel);
    
    /* Read Operations */
    public Task<List<WinnerModel>?> GetWinnersByDraw(int drawId);
    public Task<List<WinnerModel>?> GetRecentWinners(int numberOfWinners);
}