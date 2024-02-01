using MudBlazor;

namespace RaffleKing.Components.Layout;

public partial class MainLayout
{
    private bool _isDarkModeActive = true;
    private LeftNav _leftNav = null!;
    private bool _isDrawerOpen;
    private bool _isDrawerHeldOpen;
    private bool _isAccountPopoverOpen;

    /// <summary>
    /// Closes the MudDrawer containing LeftNav, if not being held open through use of the menu button.
    /// </summary>
    private void CloseDrawer()
    {
        if (_isDrawerHeldOpen) return;
        _isDrawerOpen = false;
        _leftNav.CloseExpandedGroups();;
    }

    /// <summary>
    /// Toggles the expansion of the MudDrawer containing LeftNav.
    /// </summary>
    private void ToggleDrawer()
    {
        _isDrawerOpen = !_isDrawerOpen;
        _isDrawerHeldOpen = !_isDrawerHeldOpen;
    }

    /// <summary>
    /// Toggles the visibility of the account popover MudPopover.
    /// </summary>
    private void ToggleAccountPopover()
    {
        _isAccountPopoverOpen = !_isAccountPopoverOpen;
    }
    
    /// <summary>
    /// Overrides default theme. Non-overridden properties are set to default theme values.
    /// </summary>
    public static MudTheme DefaultTheme => new()
    {
        Palette = new PaletteLight
        {
            Primary = "#0bba83",
            TextPrimary = "#515151",
            Background = "#e9e9e9",
            Surface = "#ececec",
            DrawerBackground = "#ececec",
            AppbarBackground = "#f7f7f7",
            Divider = "#b5b5b5",
        },
        PaletteDark = new PaletteDark
        {
            White = "#837162",
            Primary = "#0bba83",
            Background = "#151521",
            Surface = "#1b1b28",
            DrawerBackground = "#1b1b28",
            AppbarBackground = "#1e1e2d",
            TextPrimary = "#adadb1",
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