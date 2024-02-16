namespace RaffleKing.Components.Pages;

public partial class Home
{
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await SnackbarHelper.DisplayPendingSnackbarMessages();
        }
    }
}