using MudBlazor;

namespace RaffleKing.Components.Layout;

public partial class MainLayout
{
    private string? _username;
    private bool _isDarkModeActive = true;
    private LeftNav _leftNav = null!;
    private bool _isDrawerOpen;
    private bool _isDrawerHeldOpen;
    private bool _isAccountPopoverOpen;
    private bool _isAccountPage;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _isAccountPage = NavigationManager.Uri.Contains("/Account");
    }
    
    protected override async Task OnInitializedAsync()
    {
        _username = await UserService.GetUsername();
    }

    /// <summary>
    /// Closes the navigation drawer if it is not pinned open.
    /// </summary>
    private void CloseDrawer()
    {
        if (_isDrawerHeldOpen) return;
        _isDrawerOpen = false;
        _leftNav.CloseExpandedGroups();
    }

    /// <summary>
    /// Toggles the open state of the navigation drawer, including its pinned state.
    /// </summary>
    private void ToggleDrawer()
    {
        _isDrawerOpen = !_isDrawerOpen;
        _isDrawerHeldOpen = !_isDrawerHeldOpen;
    }

    /// <summary>
    /// Toggles the visibility of the account popover.
    /// </summary>
    private void ToggleAccountPopover()
    {
        _isAccountPopoverOpen = !_isAccountPopoverOpen;
    }
    
    /// <summary>
    /// Defines the default theme for the application, specifying both light and dark palette options.
    /// </summary>
    public static MudTheme DefaultTheme => new()
    {
        Palette = new PaletteLight
        {
            Primary = "#0bba83",
            Tertiary = "#1b1b1b",
            Background = "#e9e9e9",
            Surface = "#ececec",
            DrawerBackground = "#ececec",
            AppbarBackground = "#f7f7f7",
            Divider = "#b5b5b5",
            Info = "#534399"
        },
        PaletteDark = new PaletteDark
        {
            White = "#837162",
            Primary = "#0bba83",
            Tertiary = "#f5f5f5",
            Background = "#151521",
            Surface = "#1b1b28",
            DrawerBackground = "#1b1b28",
            AppbarBackground = "#1e1e2d",
            TextPrimary = "#adadb1",
            Info = "#534399"
        },
        Typography = new Typography
        {
            H1 = new H1
            {
                FontFamily = ["Roboto", "Helvetica", "Arial", "sans-serif"],
                FontSize = "4rem",
                FontWeight = 500,
                LineHeight = 1.2,
                LetterSpacing = ".0075em"
            }
        }
    };
}