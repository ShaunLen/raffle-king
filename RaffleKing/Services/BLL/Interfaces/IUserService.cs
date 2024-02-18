using System.Security.Claims;

namespace RaffleKing.Services.BLL.Interfaces;

public interface IUserService
{
    Task<ClaimsPrincipal> GetUser();
    Task<bool> IsGuest();
    Task<bool> IsAuthenticated();
    Task<bool> IsHost();
    Task<bool> IsAdmin();
    Task<string?> GetUsername();
    Task<bool> IsHostOfDraw(int drawId);
}