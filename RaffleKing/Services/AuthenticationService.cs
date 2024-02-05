using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using RaffleKing.Data;

namespace RaffleKing.Services;

public class AuthenticationService(SignInManager<ApplicationUser> signInManager, NavigationManager navigationManager)
{
    public async Task SignOutAsync()
    {
        await signInManager.SignOutAsync();
        navigationManager.NavigateTo("/");
    }
}