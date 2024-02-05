using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace RaffleKing.Infrastructure;

public static class StartupExtensions
{
    public static void MapAccountServices(this IEndpointRouteBuilder app)
    {
        app
            .MapGet("/Logout", async (HttpContext context, string returnUrl = "/") =>
            {
                await context.SignOutAsync(IdentityConstants.ApplicationScheme);
                var logoutReturnUrl = QueryHelpers.AddQueryString(returnUrl, "from", "logout");
                context.Response.Redirect(logoutReturnUrl);
            })
            .RequireAuthorization();
    }
}