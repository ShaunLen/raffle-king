using MudBlazor;

namespace RaffleKing.Components.Pages;

public partial class EnteredDraws
{
    private string _guestRef;

    private async Task LoadDrawFromGuestRef()
    {
        var entry = await EntryManagementService.GetGuestEntry(_guestRef);
        if (entry == null)
        {
            Snackbar.Add("No entry found with this guest reference code!", Severity.Error);
            return;
        }
        
        var drawId = entry.DrawId;
        NavigationManager.NavigateTo($"/draws/draw-details/{drawId}");
    }
}