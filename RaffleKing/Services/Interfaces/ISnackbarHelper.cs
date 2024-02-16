using MudBlazor;

namespace RaffleKing.Services.Interfaces;

public interface ISnackbarHelper
{
    /// <summary>
    /// Add a key/value string pair to local storage to be used for displaying a Snackbar after rendering a page.
    /// </summary>
    /// <param name="key">The Snackbar identifier. Must match one of the keys defined in SnackbarHelper.</param>
    /// <param name="message">The message to be displayed in the Snackbar.</param>
    Task QueueSnackbarMessageForReload(string key, string message);
    
    /// <summary>
    /// Display any pending Snackbar entries in local storage, then delete the entry.
    /// </summary>
    Task DisplayPendingSnackbarMessages();
}