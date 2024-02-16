using Microsoft.AspNetCore.Components;

namespace RaffleKing.Components.Layout;

public partial class AccountPopover
{
    [CascadingParameter(Name = "IsLoggedIn")]
    private bool isLoggedIn { get; set; }
    
    private void Logout()
    {
        SnackbarHelper.QueueSnackbarMessageForReload("LoggedOut", "Logged out successfully.");
        NavigationManager.NavigateTo("/Logout", true);
    }
}