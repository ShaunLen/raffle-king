using Microsoft.AspNetCore.Components;
using RaffleKing.Data.Models;

namespace RaffleKing.Components.Pages.DrawDetails;

public partial class DrawDetails
{
    /// <summary>
    /// The Id of the draw to be displayed.
    /// </summary>
    [Parameter]
    public int DrawId { get; set; }

    private DrawModel? _draw;
    private List<PrizeModel>? _prizes;
    private bool _userIsHost;

    protected override async Task OnInitializedAsync()
    {
        _draw = await DrawManagementService.GetDrawById(DrawId);
        _prizes = await PrizeManagementService.GetPrizesByDraw(DrawId);
        _userIsHost = await UserService.IsHostOfDraw(DrawId);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await SnackbarHelper.DisplayPendingSnackbarMessages();
    }
}