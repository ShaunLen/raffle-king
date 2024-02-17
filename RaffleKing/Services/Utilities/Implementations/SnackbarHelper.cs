using Blazored.LocalStorage;
using MudBlazor;
using RaffleKing.Services.Utilities.Interfaces;

namespace RaffleKing.Services.Utilities.Implementations;

public class SnackbarHelper(ISnackbar snackbar, ILocalStorageService localStorage) : ISnackbarHelper
{
    private readonly List<string> _sbSuccessKeys = [
    "Snackbar_LoggedOut",
    "Snackbar_DrawPublished",
    "Snackbar_PrizeAdded",
    "Snackbar_PrizeDeleted",
    "Snackbar_EnteredDraw",
    "Snackbar_RemovedEntries"];
    
    public async Task QueueSnackbarMessageForReload(string key, string message)
    {
        await localStorage.SetItemAsync($"Snackbar_{key}", message);
    }
    
    public async Task DisplayPendingSnackbarMessages()
    {
        foreach (var key in _sbSuccessKeys)
        {
            var message = await localStorage.GetItemAsync<string>(key);
            if (string.IsNullOrEmpty(message)) continue;
            
            snackbar.Add(message, Severity.Success);
            await localStorage.RemoveItemAsync(key);
        }
    }
}