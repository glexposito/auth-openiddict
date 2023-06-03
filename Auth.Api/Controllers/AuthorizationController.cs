using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace Auth.Api.Controllers;

[ApiController]
public class AuthorizationController : ControllerBase
{
    private const string Fullname = "Ken Masters";
    private const string Email = "ken@masters.com";
    private const string Password = "123456";

    [HttpPost("~/connect/token"), Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
                      throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        if (request.IsPasswordGrantType())
        {
            return await HandleExchangePasswordGrantType(request);
        }

        if (request.IsRefreshTokenGrantType())
        {
            return await HandleExchangeRefreshTokenGrantType();
        }

        throw new InvalidOperationException("The specified grant type is not supported.");
    }

    private Task<IActionResult> HandleExchangePasswordGrantType(OpenIddictRequest request)
    {
        if (request.Username != Email || request.Password != Password)
        {
            return Task.FromResult<IActionResult>(Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme));
        }

        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: OpenIddictConstants.Claims.Name,
            roleType: OpenIddictConstants.Claims.Role);

        identity.AddClaim(OpenIddictConstants.Claims.Subject,
            Guid.NewGuid().ToString(),
            OpenIddictConstants.Destinations.AccessToken);

        identity.AddClaim(OpenIddictConstants.Claims.Name, Fullname,
            OpenIddictConstants.Destinations.AccessToken);

        identity.AddClaim(OpenIddictConstants.Claims.Email, Email,
            OpenIddictConstants.Destinations.AccessToken);
        
        var claimsPrincipal = new ClaimsPrincipal(identity);
        claimsPrincipal.SetScopes(OpenIddictConstants.Scopes.OfflineAccess);

        return Task.FromResult<IActionResult>(SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme));
    }
    
    private Task<IActionResult> HandleExchangeRefreshTokenGrantType()
    {
        if (User.Identity == null)
        {
            return Task.FromResult<IActionResult>(Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme));
        }
        
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: OpenIddictConstants.Claims.Name,
            roleType: OpenIddictConstants.Claims.Role);

        identity.AddClaim(OpenIddictConstants.Claims.Subject,
            Guid.NewGuid().ToString(),
            OpenIddictConstants.Destinations.AccessToken);

        identity.AddClaim(OpenIddictConstants.Claims.Name, Fullname,
            OpenIddictConstants.Destinations.AccessToken);

        identity.AddClaim(OpenIddictConstants.Claims.Email, Email,
            OpenIddictConstants.Destinations.AccessToken);
            
        var claimsPrincipal = new ClaimsPrincipal(identity);
        claimsPrincipal.SetScopes(OpenIddictConstants.Scopes.OfflineAccess);
        
        return Task.FromResult<IActionResult>(SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme));
    }
}