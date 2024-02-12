using System.Text;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using RaffleKing.Components.Shared;
using RaffleKing.Data.Models;

namespace RaffleKing.Components.Pages;

public partial class DrawDetails
{
    /// <summary>
    /// The Id of the draw to be displayed.
    /// </summary>
    [Parameter]
    public int DrawId { get; set; }

    private DrawModel? _draw;
    private List<PrizeModel>? _prizes;
    private int _availableEntries;
    private List<int>? _availableLuckyNumbers;
    private string? _detailsText;
    private bool _userIsHost;

    protected override async Task OnInitializedAsync()
    {
        _draw = await DrawService.GetDrawById(DrawId);
        _prizes = await PrizeService.GetPrizesForDraw(DrawId);
        _availableEntries = GetAvailableEntries();
        _detailsText = GetDetailsText();
        _availableLuckyNumbers = GetAvailableLuckyNumbers();
        
        // Check whether the currently logged in user is the host of this draw
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity is { IsAuthenticated: true })
        {
            _userIsHost = _draw?.DrawHostId == user.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // If draw has just been published, display snackbar to notify host
            var drawPublished = await LocalStorage.GetItemAsync<bool>("DrawPublished");
            if (drawPublished)
            {
                Snackbar.Add("Draw has been published!", Severity.Success);
                await LocalStorage.RemoveItemAsync("DrawPublished");
            }
            
            // If a prize has just been added, display snackbar to notify host
            var prizeAdded = await LocalStorage.GetItemAsync<bool>("PrizeAdded");
            if (prizeAdded)
            {
                Snackbar.Add("Prize has been added!", Severity.Success);
                await LocalStorage.RemoveItemAsync("PrizeAdded");
            }
            
            // If a prize has just been deleted, display snackbar to notify host
            var prizeDeleted = await LocalStorage.GetItemAsync<bool>("PrizeDeleted");
            if (prizeDeleted)
            {
                Snackbar.Add("Prize has been deleted!", Severity.Success);
                await LocalStorage.RemoveItemAsync("PrizeDeleted");
            }
        }
    }

    /// <summary>
    /// Returns the number of available entries for this draw. Returns the difference between MaxEntriesPerUser and
    /// the current number of entries made by this user, or the number of total entries remaining, whichever is lower. 
    /// </summary>
    /// <returns>Maximum available entries.</returns>
    private int GetAvailableEntries()
    {
        // TODO: Get actual available entries
        if (_draw is null) return 0;
        
        return _draw.MaxEntriesPerUser;
    }

    /// <summary>
    /// Returns a list of the available lucky numbers for this draw (if lottery). The list will only contain
    /// </summary>
    private List<int> GetAvailableLuckyNumbers()
    {
        var availableLuckyNumbers = new List<int>();
        if (_draw?.DrawType != DrawTypeEnum.Lottery) return availableLuckyNumbers;
        
        for (var i = 1; i < 100; i++)
        {
            // Temporarily just add max range of lucky numbers until entry logic is added
            availableLuckyNumbers.Add(i);
        }

        return availableLuckyNumbers;
    }

    /// <summary>
    /// Returns the text to be displayed underneath the draw description. Contains information about the number of
    /// prizes available, and whether or not the draw is a bundle (single winner for all prizes or individual winner
    /// for each prize).
    /// </summary>
    /// <returns>The draw details text.</returns>
    private string GetDetailsText()
    {
        var numberOfPrizes = _prizes?.Sum(prize => prize.Quantity);
        var text = new StringBuilder();

        text.Append($"There are a total of {numberOfPrizes} prizes to be won in this {_draw?.DrawType.ToString()}. ");

        if (_draw is { IsBundle: true })
        {
            if (_draw.DrawType == DrawTypeEnum.Raffle)
            {
                text.Append("This Raffle is a bundle, so a single winner will be drawn as the recipient of all " +
                            "prizes - each entry gives you 1 chance to win.");
            }
            else
            {
                text.Append("This Lottery is a bundle, so a single lucky number will be drawn as the recipient of " +
                            "all prizes - each entry gives you 1 chance to win.");
            }
        }
        else
        {
            text.Append($"A lucky number will be drawn for each prize in this {_draw?.DrawType.ToString()}, " +
                        $"so each entry gives you {numberOfPrizes} chances to win!");
        }

        return text.ToString();
    }

    /// <summary>
    /// Displays a dialog requiring confirmation before publishing the draw. Once confirmed, the draw is published
    /// and the page is reloaded.
    /// </summary>
    private async Task PublishDrawWithConfirmation()
    {
        // Display confirmation dialog
        var result = await DialogService.ShowMessageBox(
            "Publish Draw",
            "Once published, the draw will no longer be editable and will be open to entries. Are you sure?",
            yesText: "Publish", cancelText: "Cancel");
        
        if (result == null) return;

        await DrawService.ActivateDraw(DrawId);
        await LocalStorage.SetItemAsync("DrawPublished", true);
        NavigationManager.NavigateTo(NavigationManager.Uri, true);
    }
    
    /// <summary>
    /// Displays a dialog requiring confirmation before deleting the draw. Once confirmed, the draw is deleted along
    /// with any linked prizes and entries and the user is navigated to <i>/draws/my-draws</i>.
    /// </summary>
    private async Task DeleteDrawWithConfirmation()
    {
        // Display confirmation dialog
        var result = await DialogService.ShowMessageBox(
            "Delete Draw",
            "Deletion is permanent! All associated prizes and entries will also be deleted and no winners " +
            "will be drawn. Are you sure?",
            yesText: "Delete", cancelText: "Cancel");

        if (result == null) return;

        // Delete all prizes associated with this draw
        if (_prizes != null)
            foreach (var prize in _prizes)
            {
                await PrizeService.DeletePrize(prize.Id);
            }
        
        // TODO: Delete all associated entries 

        await DrawService.DeleteDraw(DrawId);
        NavigationManager.NavigateTo("/draws/my-draws");
        Snackbar.Add("Draw deleted successfully", Severity.Success);
    }
    
    /// <summary>
    /// Displays a dialog requiring confirmation before deleting the prize. Once confirmed, the prize is deleted.
    /// </summary>
    private async Task DeletePrizeWithConfirmation(int prizeId)
    {
        // Display confirmation dialog
        var result = await DialogService.ShowMessageBox(
            "Delete Prize",
            "Deletion is permanent! Are you sure?",
            yesText: "Delete", cancelText: "Cancel");

        if (result == null) return;

        await PrizeService.DeletePrize(prizeId);
        await LocalStorage.SetItemAsync("PrizeDeleted", true);
        NavigationManager.NavigateTo(NavigationManager.Uri, true);
    }

    private async Task AddPrize()
    {
        var parameters = new DialogParameters
        {
            { "DrawId", DrawId }
        };
        var dialog = await DialogService.ShowAsync<AddPrize>("Add Prize", parameters);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            if (result.Data is PrizeModel prize) await PrizeService.AddNewPrize(prize);
            await LocalStorage.SetItemAsync("PrizeAdded", true);
            NavigationManager.NavigateTo(NavigationManager.Uri, true);
        }
    }
}