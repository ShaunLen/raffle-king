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
    private List<EntryModel>? _allEntries;
    private List<EntryModel>? _userEntries;
    private int _availableEntries;
    private List<int>? _availableLuckyNumbers;
    private string? _detailsText;
    private bool _userIsHost;
    private double _percentageEntriesRemaining;
    private int _selectedNumberOfEntries = 1;
    private IEnumerable<int> _selectedLuckyNumbers = new List<int>();

    protected override async Task OnInitializedAsync()
    {
        await FetchFreshData();
        
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
            await SnackbarHelper.DisplayPendingSnackbarMessages();
    }
    
    /// <summary>
    /// Updates local fields with fresh information from the database. It is important to do this before performing
    /// any action that relies on this data, to ensure the data is not outdated by the time it is used in checks.
    /// </summary>
    private async Task FetchFreshData()
    {
        _draw = await DrawService.GetDrawById(DrawId);
        _prizes = await PrizeService.GetPrizesForDraw(DrawId);
        _allEntries = await EntryService.GetEntriesForDraw(DrawId);
        _userEntries = await EntryService.GetEntriesForDrawByUser(DrawId);
        _availableEntries = GetAvailableEntries();
        _availableLuckyNumbers = GetAvailableLuckyNumbers();
        _detailsText = GetDetailsText();
        
        if(_allEntries != null && _draw != null)
            _percentageEntriesRemaining = Math.Round(100 - (float)_allEntries.Count / _draw.MaxEntriesTotal * 100);
    }

    /// <summary>
    /// Returns the number of available entries for this draw. Returns the difference between MaxEntriesPerUser and
    /// the current number of entries made by this user, or the number of total entries remaining, whichever is lower. 
    /// </summary>
    /// <returns>Maximum available entries.</returns>
    private int GetAvailableEntries()
    {
        if (_draw is null) return 0;

        if (_userEntries != null && _allEntries != null)
                return Math.Min(_draw.MaxEntriesPerUser - _userEntries.Count, _draw.MaxEntriesTotal - _allEntries.Count);
        
        return 0;
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
        var result = await DialogService.ShowMessageBox(
            "Delete Draw",
            "Deletion is permanent! All associated prizes and entries will also be deleted and no winners " +
            "will be drawn. Are you sure?",
            yesText: "Delete", cancelText: "Cancel");

        if (result == null) return;

        // Delete all prizes associated with this draw
        if (_prizes != null)
            foreach (var prize in _prizes)
                await PrizeService.DeletePrize(prize.Id);
        
        // Delete all entries associated with this draw
        if(_allEntries != null)
            foreach (var entry in _allEntries)
                await EntryService.DeleteEntry(entry.Id);

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
            if (result.Data is PrizeModel prize) await PrizeService.AddNewPrize(prize);
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
        var result = await DialogService.ShowMessageBox(
            "Remove Entries",
            "All of your entries for this draw will be removed! Are you sure?",
            yesText: "Remove", cancelText: "Cancel");
        
        if (_userEntries == null || result == null) return;
        
        foreach (var entry in _userEntries)
        {
            await EntryService.DeleteEntry(entry.Id);
        }
        
        await SnackbarHelper.QueueSnackbarMessageForReload("RemovedEntries", 
            "You have removed your entries for this draw!");
        NavigationManager.NavigateTo(NavigationManager.Uri, true);
    }

    
    private async Task EnterDrawAsUser()
    {
        await FetchFreshData();

        if (_availableEntries == 0)
            return;
        
        if (_selectedNumberOfEntries > _availableEntries || _selectedLuckyNumbers.Count() > _availableEntries)
        {
            Snackbar.Add("Failed to enter draw - too many selections.", Severity.Error);
            return;
        }
        
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        var userId = "";

        if (user.Identity is { IsAuthenticated: true })
        {
            userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }

        if (userId == "") return;

        List<EntryModel> entries = [];

        switch (_draw?.DrawType)
        {
            case DrawTypeEnum.Raffle:
                for (var i = 0; i < _selectedNumberOfEntries; i++)
                {
                    entries.Add(new EntryModel
                    {
                        DrawId = DrawId,
                        UserId = userId
                    });
                }
                break;
            case DrawTypeEnum.Lottery:
                for (var i = 0; i < _selectedLuckyNumbers.Count(); i++)
                {
                    entries.Add(new EntryModel
                    {
                        DrawId = DrawId,
                        UserId = userId,
                        LuckyNumber = _selectedLuckyNumbers.ToList()[i]
                    });
                }
                break;
            default:
                return;
        }

        foreach (var entry in entries)
        {
            await EntryService.AddEntry(entry);
        }
        
        await SnackbarHelper.QueueSnackbarMessageForReload("EnteredDraw", "Draw entered!");
        NavigationManager.NavigateTo(NavigationManager.Uri, true);
    }

    private async Task EnterDrawAsGuest()
    {
        await FetchFreshData();
    }
}