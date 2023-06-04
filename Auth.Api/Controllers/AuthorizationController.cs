using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
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

        throw new InvalidOperationException("The specified grant type is not supported.");
    }

    private Task<IActionResult> HandleExchangePasswordGrantType(OpenIddictRequest request)
    {
        if (request.Username != Email || request.Password != Password)
        {
            return Task.FromResult<IActionResult>(Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme));
        }

        var identity = new ClaimsIdentity(
            authenticationType: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        identity.AddClaim(OpenIddictConstants.Claims.Name, Fullname);

        identity.AddClaim(OpenIddictConstants.Claims.Email, Email);

        identity.AddClaim(OpenIddictConstants.Claims.Subject, Guid.NewGuid().ToString());

        identity.SetDestinations(GetDestinations);

        var principal = new ClaimsPrincipal(identity);

        return Task.FromResult<IActionResult>(
            SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme));
    }

    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        yield return OpenIddictConstants.Destinations.AccessToken;
    }
}