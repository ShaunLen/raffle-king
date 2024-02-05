using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;

namespace RaffleKing.Components.Pages;

public partial class Home
{
    protected override void OnInitialized()
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("from", out var from) && from == "logout")
        {
            Snackbar.Add("Logged out successfully.", Severity.Success);
        }
    }
}