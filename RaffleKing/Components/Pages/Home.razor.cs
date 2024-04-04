using Microsoft.IdentityModel.Tokens;
using RaffleKing.Data.Models;

namespace RaffleKing.Components.Pages;

public partial class Home
{
    private bool _userAuthenticated;
    private List<DrawModel>? _activeDraws;
    private List<DrawModel>? _enteredDraws;
    private List<WinnerModel>? _recentWinners;
    private List<string> _recentWinnerNames = [];
    private List<string> _recentWinnerPrizes = [];
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await SnackbarHelper.DisplayPendingSnackbarMessages();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        _userAuthenticated = await UserService.IsAuthenticated();
        
        _activeDraws = await DrawManagementService.GetActiveDraws();
        _enteredDraws = await EntryManagementService.GetDrawsEnteredByCurrentUser();
        
        // Sort draws by draw date
        _activeDraws = _activeDraws?.OrderBy(draw => draw.DrawDate).ToList();
        _enteredDraws = _enteredDraws?.OrderBy(draw => draw.DrawDate).ToList();

        _recentWinners = await EntryManagementService.GetRecentWinners(11);

        if (!_recentWinners.IsNullOrEmpty())
        {
            
            foreach (var winner in _recentWinners!)
            {
                var entry = await EntryManagementService.GetEntryById((int) winner.EntryId!);
                var prize = await PrizeManagementService.GetPrizeById(winner.PrizeId);

                if (entry is { IsGuest: true })
                {
                    _recentWinnerNames.Add("Guest");
                }
                else if (entry?.UserId != null)
                {
                    var user = await UserManager.FindByIdAsync(entry.UserId);
                    _recentWinnerNames.Add(user?.UserName ?? "Someone");
                }
                else
                {
                    _recentWinnerNames.Add("Someone");
                }

                _recentWinnerPrizes.Add($"'{prize?.Title}'");
            }
        }
    }
}