using Microsoft.AspNetCore.Components;

namespace RaffleKing.Components.Layout;

public partial class LeftNav
{
    [Parameter]
    public bool IsDrawerClosed { get; set; }
    private bool _isRafflesExpanded;
    
    /// <summary>
    /// Closes any expanded MudNavGroup components within the MudNavMenu.
    /// </summary>
    public void CloseExpandedGroups()
    {
        _isRafflesExpanded = false;
        StateHasChanged();
    }
    
    /* Override methods */
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (IsDrawerClosed)
            CloseExpandedGroups();
    }
}