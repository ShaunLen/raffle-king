using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using RaffleKing.Services.BLL.Interfaces;
using RaffleKing.Services.DAL.Interfaces;

namespace RaffleKing.Services.BLL.Implementations;

public class UserService(AuthenticationStateProvider authenticationStateProvider, IDrawService drawService) 
    : IUserService
{
    public async Task<ClaimsPrincipal> GetUser()
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        return authState.User;
    }

    public async Task<string?> GetUserId()
    {
        var user = await GetUser();
        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public async Task<string?> GetUsername()
    {
        var user = await GetUser();
        return user.Identity is { IsAuthenticated: true } ? user.Identity.Name : "";
    }
    
    public async Task<bool> IsGuest()
    {
        var user = await GetUser();
        return user.Identity is { IsAuthenticated: false };
    }

    public async Task<bool> IsAuthenticated()
    {
        var user = await GetUser();
        return user.Identity is { IsAuthenticated: true };
    }

    public async Task<bool> IsHost()
    {
        var user = await GetUser();
        return user.IsInRole("Host");
    }

    public async Task<bool> IsAdmin()
    {
        var user = await GetUser();
        return user.IsInRole("Admin");
    }

    public async Task<bool> IsHostOfDraw(int drawId)
    {
        var draw = await drawService.GetDrawById(drawId);
        var user = await GetUser();
        
        if (user.Identity is { IsAuthenticated: true })
        {
            return draw?.DrawHostId == user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        return false;
    }
}