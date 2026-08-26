using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using RealStatePortal.Application.Abstractions.Authentication;

namespace RealStatePortal.Api.Controllers;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
[ApiController]
[Route("api/auth")]
public sealed class AuthController(ICurrentUserService currentUser, IConfiguration configuration) : ControllerBase
{
    [HttpGet("/auth/login")]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = "/")
    {
        var safeReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : "/";
        var redirectUri = BuildFrontendUrl(safeReturnUrl);
        return Challenge(new AuthenticationProperties { RedirectUri = redirectUri }, OpenIdConnectDefaults.AuthenticationScheme);
    }

    [HttpGet("/auth/logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        var authority = configuration["Auth0:Authority"]?.TrimEnd('/');
        var clientId = configuration["Auth0:ClientId"];
        if (string.IsNullOrWhiteSpace(authority) || string.IsNullOrWhiteSpace(clientId))
        {
            return Redirect(BuildFrontendUrl("/"));
        }

        var logoutUrl = QueryHelpers.AddQueryString(
            $"{authority}/v2/logout",
            new Dictionary<string, string?>
            {
                ["client_id"] = clientId,
                ["returnTo"] = BuildFrontendUrl("/")
            });

        return Redirect(logoutUrl);
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        return Ok(new
        {
            currentUser.IsAuthenticated,
            UserName = User.Identity?.Name ?? User.FindFirst("name")?.Value,
            Email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value,
            currentUser.Auth0UserId,
            currentUser.Roles
        });
    }

    private string BuildFrontendUrl(string path)
    {
        var frontendBaseUrl = configuration["Frontend:BaseUrl"]?.TrimEnd('/')
            ?? "http://localhost:5173";
        return $"{frontendBaseUrl}{path}";
    }
}