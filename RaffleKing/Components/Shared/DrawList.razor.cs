using Microsoft.AspNetCore.Components;
using RaffleKing.Data.Models;

namespace RaffleKing.Components.Shared;

public enum DrawListType
{
    ActiveDraws,
    HostedDraws,
    EnteredDraws
}

public partial class DrawList
{
    /// <summary>
    /// The type of draw list (Active, Hosted or Entered).
    /// </summary>
    [Parameter]
    public DrawListType ListType { get; set; }
    
    /* Private Fields */
    private List<DrawModel>? _draws;
    private string? _headingPrefix;

    protected override async Task OnInitializedAsync()
    {
        // Change list heading depending on list type
        switch (ListType)
        {
            case DrawListType.ActiveDraws:
                _draws = await DrawManagementService.GetActiveDraws();
                _headingPrefix = "Active";
                break;
            case DrawListType.HostedDraws:
                _draws = await DrawManagementService.GetDrawsHostedByCurrentUser();
                _headingPrefix = "My";
                break;
            case DrawListType.EnteredDraws:
                _draws = await EntryManagementService.GetDrawsEnteredByCurrentUser();
                _headingPrefix = "Entered";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}