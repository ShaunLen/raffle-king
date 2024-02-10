using Microsoft.AspNetCore.Components;
using RaffleKing.Data.Models;

namespace RaffleKing.Components.Pages;

public partial class DrawDetails
{
    [Parameter]
    public int DrawId { get; set; }

    private DrawModel? _draw;
    private List<PrizeModel>? _prizes;
    private int _availableEntries;

    protected override async Task OnInitializedAsync()
    {
        _draw = await DrawService.GetDrawById(DrawId);
        _prizes = await PrizeService.GetPrizesForDraw(DrawId);
        _availableEntries = GetAvailableEntries();
    }

    /// <summary>
    /// Returns the number of available entries for this draw. Returns the difference between MaxEntriesPerUser and
    /// the current number of entries made by this user, or the number of total entries remaining - whichever is lower. 
    /// </summary>
    /// <returns>Maximum available entries.</returns>
    private int GetAvailableEntries()
    {
        if (_draw is null) return 0;
        
        return _draw.MaxEntriesPerUser;
    }
}