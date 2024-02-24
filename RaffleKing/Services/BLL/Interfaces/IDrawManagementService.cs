﻿using RaffleKing.Common;
using RaffleKing.Data.Models;

namespace RaffleKing.Services.BLL.Interfaces;

public interface IDrawManagementService
{
    Task<OperationResult<int>> AddNewDraw(DrawModel draw);
    Task<DrawModel?> GetDrawById(int drawId);
    Task<OperationResult> ActivateDraw(int drawId);
    Task DeleteDraw(int drawId);
}