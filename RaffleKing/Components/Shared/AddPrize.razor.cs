using Microsoft.AspNetCore.Components;
using MudBlazor;
using RaffleKing.Data.Models;

namespace RaffleKing.Components.Shared;

public partial class AddPrize
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    [Parameter] public int DrawId { get; set; }

    private DrawModel? _draw;
    private List<PrizeModel>? _prizes;
        
    // Form fields
    private string _title = string.Empty;
    private string _description = string.Empty;
    private int _quantity = 1;

    protected override async Task OnInitializedAsync()
    {
        _draw = await DrawManagementService.GetDrawById(DrawId);
        _prizes = await PrizeManagementService.GetPrizesByDraw(DrawId);
    }

    private void Cancel() => MudDialog.Close(DialogResult.Cancel());

    private void CreatePrize()
    {
        if (!PrizeIsValid())
            return;
        
        var prize = new PrizeModel
        {
            Title = _title,
            Description = _description,
            Quantity = _quantity,
            DrawId = DrawId
        };
        
        MudDialog.Close(DialogResult.Ok(prize));
    }

    private bool PrizeIsValid()
    {
        if (_title.Length == 0 || _description.Length == 0)
            return false;
        
        var currentPrizeQuantity = 0;
        if (_prizes != null) currentPrizeQuantity += _prizes.Sum(prize => prize.Quantity);

        return _draw is not { IsBundle: false } || (currentPrizeQuantity + _quantity) <= _draw.MaxEntriesTotal;
    }
}