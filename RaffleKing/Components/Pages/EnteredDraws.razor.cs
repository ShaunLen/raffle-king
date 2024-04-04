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
        var draw = await DrawManagementService.GetDrawById(drawId);
        
        if (draw == null)
        {
            Snackbar.Add("Invalid draw!", Severity.Error);
            return;
        }

        // Claim any prizes this guest entry has won
        if (draw.IsFinished)
        {
            var winners = await DrawManagementService.GetWinnersByDraw(drawId);

            if (winners != null)
            {
                var hasWon = false;
                foreach (var winner in winners.Where(winner => winner.EntryId == entry.Id))
                {
                    // Don't reclaim any already claimed prizes
                    if(winner.IsClaimed)
                        continue;
                    
                    await PrizeManagementService.ClaimPrize(winner.Id);
                    hasWon = true;
                }
                
                if(hasWon)
                    await SnackbarHelper.QueueSnackbarMessageForReload(
                        "PrizeClaimed", "You're a winner! Prize(s) claimed.");
            }
        }
        
        NavigationManager.NavigateTo($"/draws/draw-details/{drawId}");
    }
}