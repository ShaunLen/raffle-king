﻿using RaffleKing.Data.Models;

namespace RaffleKing.Services.DAL.Interfaces;

public interface IPrizeService
{
    /* Create Operations */
    public Task AddNewPrize(PrizeModel prizeModel);
    
    /* Read Operations */
    public Task<PrizeModel?> GetPrizeById(int prizeId);
    public Task<List<PrizeModel>?> GetPrizesByDraw(int drawId);
    
    /* Delete Operations */
    public Task DeletePrize(int prizeId);
}