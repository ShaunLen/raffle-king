using System.Text;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using RaffleKing.Common;
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
    private int _userEntriesCount;
    private int _availableEntries;
    private List<int>? _availableLuckyNumbers;
    private string? _detailsText;
    private bool _userIsHost;
    private bool _userIsGuest;
    private double _percentageEntriesRemaining;
    private int _selectedNumberOfEntries = 1;
    private IEnumerable<int> _selectedLuckyNumbers = new List<int>();
    private string _guestEmail = "";

    protected override async Task OnInitializedAsync()
    {
        _draw = await DrawManagementService.GetDrawById(DrawId);
        _userIsHost = await UserService.IsHostOfDraw(DrawId);
        _userIsGuest = await UserService.IsGuest();
        await FetchFreshData();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await SnackbarHelper.DisplayPendingSnackbarMessages();
    }
    
    /// <summary>
    /// Updates local fields with fresh information from the database. It is important to do this before performing
    /// any action that relies on this data, to ensure the data is not outdated by the time it is used in checks.
    /// </summary>
    private async Task FetchFreshData()
    {
        _prizes = await PrizeManagementService.GetPrizesByDraw(DrawId);
        _userEntriesCount = await EntryManagementService.CountCurrentUserEntriesByDraw(DrawId);
        _percentageEntriesRemaining = await EntryManagementService.GetPercentageEntriesRemainingByDraw(DrawId);
        _availableEntries = await EntryManagementService.CountCurrentUserEntriesRemainingByDraw(DrawId);
        _availableLuckyNumbers = await EntryManagementService.GetAvailableLuckyNumbersByDraw(DrawId);
        _detailsText = GetDetailsText();
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
        var confirmed = await DialogService.ShowMessageBox(
            "Publish Draw",
            "Once published, the draw will no longer be editable and will be open to entries. Are you sure?",
            yesText: "Publish", cancelText: "Cancel");
        
        if (confirmed == null) return;

        var result = await DrawManagementService.ActivateDraw(DrawId);

        if (!result.Success)
        {
            Snackbar.Add(result.Message, Severity.Error);
            return;
        }
            
        await SnackbarHelper.QueueSnackbarMessageForReload("DrawPublished", "Draw has been published!");
        NavigationManager.NavigateTo(NavigationManager.Uri, true);
    }
    
    /// <summary>
    /// Displays a dialog requiring confirmation before deleting the draw. Once confirmed, the draw is deleted along
    /// with any linked prizes and entries and the user is navigated to <i>/draws/my-draws</i>.
    /// </summary>
    private async Task DeleteDrawWithConfirmation()
    {
        await FetchFreshData();
        
        // Display confirmation dialog
        var confirmed = await DialogService.ShowMessageBox(
            "Delete Draw",
            "Deletion is permanent! All associated prizes and entries will also be deleted and no winners " +
            "will be drawn. Are you sure?",
            yesText: "Delete", cancelText: "Cancel");

        if (confirmed == null) return;

        await DrawManagementService.DeleteDraw(DrawId);
        NavigationManager.NavigateTo("/draws/my-draws");
        Snackbar.Add("Draw deleted successfully", Severity.Success);
    }
    
    /// <summary>
    /// Displays a dialog requiring confirmation before deleting the prize. Once confirmed, the prize is deleted.
    /// </summary>
    private async Task DeletePrizeWithConfirmation(int prizeId)
    {
        // Display confirmation dialog
        var confirmed = await DialogService.ShowMessageBox(
            "Delete Prize",
            "Deletion is permanent! Are you sure?",
            yesText: "Delete", cancelText: "Cancel");

        if (confirmed == null) 
            return;

        await PrizeManagementService.DeletePrize(prizeId);
        await SnackbarHelper.QueueSnackbarMessageForReload("PrizeDeleted", "Prize has been deleted!");
        NavigationManager.NavigateTo(NavigationManager.Uri, true);
    }

    /// <summary>
    /// Displays a dialog where details of the prize should be entered. Once confirmed, the new prize is added to the
    /// draw.
    /// </summary>
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
            if (result.Data is PrizeModel prize) await PrizeManagementService.AddNewPrize(prize);
            await SnackbarHelper.QueueSnackbarMessageForReload("PrizeAdded", "Prize has been added!");
            NavigationManager.NavigateTo(NavigationManager.Uri, true);
        }
    }
    
    /// <summary>
    /// Displays a dialog requiring confirmation before removing all entries the current user has made for this draw.
    /// </summary>
    private async Task RemoveUserEntriesWithConfirmation()
    {
        await FetchFreshData();
        
        // Display confirmation dialog
        var confirmed = await DialogService.ShowMessageBox(
            "Remove Entries",
            "All of your entries for this draw will be removed! Are you sure?",
            yesText: "Remove", cancelText: "Cancel");
        
        if (confirmed == null) 
            return;

        await EntryManagementService.RemoveCurrentUserEntriesByDraw(DrawId);
        await SnackbarHelper.QueueSnackbarMessageForReload("RemovedEntries", 
            "You have removed your entries for this draw!");
        NavigationManager.NavigateTo(NavigationManager.Uri, true);
    }

    
    private async Task EnterDraw()
    {
        await FetchFreshData();

        OperationResult result;
        switch (_draw?.DrawType)
        {
            case DrawTypeEnum.Raffle:
                result = await EntryManagementService.TryEnterRaffle(DrawId, _selectedNumberOfEntries, _guestEmail);
                break;
            case DrawTypeEnum.Lottery:
                result = await EntryManagementService.TryEnterLottery(DrawId, _selectedLuckyNumbers);
                break;
            default:
                return;
        }

        if (!result.Success)
        {
            Snackbar.Add(result.Message, Severity.Error);
            return;
        }
        
        if(_userIsGuest)
            await SnackbarHelper.QueueSnackbarMessageForReload("EnteredDraw", "Draw entered as guest!");
        else
            await SnackbarHelper.QueueSnackbarMessageForReload("EnteredDraw", "Draw entered!");
        NavigationManager.NavigateTo(NavigationManager.Uri, true);
    }
}