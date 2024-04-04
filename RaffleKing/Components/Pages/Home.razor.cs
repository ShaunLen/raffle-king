    using Microsoft.IdentityModel.Tokens;
    using RaffleKing.Data.Models;
    
    namespace RaffleKing.Components.Pages;
    
    public partial class Home
    {
        private bool _userAuthenticated;
        private string? _currentUsername;
        private List<DrawModel>? _activeDraws;
        private List<DrawModel>? _enteredDraws;
        private List<WinnerModel>? _recentWinners;
        private List<WinnerModel>? _unclaimedPrizes;
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
    
            if (_userAuthenticated)
                _currentUsername = await UserService.GetUsername();
            
            _activeDraws = await DrawManagementService.GetActiveDraws();
            _enteredDraws = await EntryManagementService.GetDrawsEnteredByCurrentUser();
    
            if (_userAuthenticated)
            {
                // Filter the active draws to remove any draw that the user has already entered.
                // Then sort the draws based on their draw date and limit to the first 5 items.
                _activeDraws = _activeDraws?
                    .Where(ad => _enteredDraws != null && _enteredDraws.All(ed => ed.Id != ad.Id))
                    .OrderBy(draw => draw.DrawDate).Take(5).ToList();
                
                // Don't show expired draws.
                _enteredDraws = _enteredDraws?
                    .Where(ed => _enteredDraws != null && !ed.IsFinished)
                    .OrderBy(draw => draw.DrawDate).Take(5).ToList();
            }
            else
            {
                // Unauthenticated (guest) users won't have draws they've entered displayed
                _activeDraws = _activeDraws?.OrderBy(draw => draw.DrawDate).Take(5).ToList();
            }
            
            _unclaimedPrizes = await PrizeManagementService.GetUnclaimedPrizesByCurrentUser();
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