using Microsoft.AspNetCore.Components;
using RaffleKing.Data.Models;

namespace RaffleKing.Components.Shared;

public partial class UnclaimedPrizeCard
{
    /// <summary>
    /// The WinnerModel item.
    /// </summary>
    [Parameter] public WinnerModel Winner { get; set; }
    private string _text;
    private PrizeModel? _prize;
    private int _drawId;

    protected override async Task OnInitializedAsync()
    {
        _prize = await PrizeManagementService.GetPrizeById(Winner.PrizeId);
        _text = $"You won '{_prize?.Title}'";
        
        if (_prize != null) 
            _drawId = _prize.DrawId;
    }

    private async void ClaimPrize()
    {
        if (_prize != null) 
            await PrizeManagementService.ClaimPrize(Winner.Id);
        
        await SnackbarHelper.QueueSnackbarMessageForReload("PrizeClaimed", $"'{_prize?.Title}' has been claimed!");
        NavigationManager.NavigateTo(NavigationManager.Uri, true);
    }
}