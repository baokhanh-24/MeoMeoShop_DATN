using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MeoMeo.Shared.IServices;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace MeoMeo.Shared.Utilities;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IAuthClientService _authService;
    private readonly ILogger<CustomAuthStateProvider> _logger;

    public CustomAuthStateProvider(IAuthClientService authService, ILogger<CustomAuthStateProvider> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var isAuthenticated = await _authService.IsAuthenticatedAsync();
            _logger.LogInformation("IsAuthenticated: {IsAuthenticated}", isAuthenticated);
            
            if (!isAuthenticated)
            {
                _logger.LogInformation("User is not authenticated, returning anonymous state");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var token = await _authService.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token is null or empty");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var claims = ParseClaimsFromJwt(token);
            _logger.LogInformation("Parsed {ClaimCount} claims from JWT", claims.Count());
            
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            _logger.LogInformation("Created authenticated user: {UserName}", 
                user.FindFirst("UserName")?.Value ?? "Unknown");

            return new AuthenticationState(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting authentication state");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public void NotifyUserAuthentication(string token)
    {
        try
        {
            _logger.LogInformation("NotifyUserAuthentication called with token length: {TokenLength}", token?.Length ?? 0);
            
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Attempted to notify authentication with empty token");
                NotifyUserLogout();
                return;
            }

            var claims = ParseClaimsFromJwt(token);
            _logger.LogInformation("Parsed {ClaimCount} claims from token", claims.Count());
            
            if (!claims.Any())
            {
                _logger.LogWarning("No claims found in token");
                NotifyUserLogout();
                return;
            }

            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            _logger.LogInformation("Notifying authentication state change for user: {UserName}", 
                user.FindFirst("UserName")?.Value ?? "Unknown");

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
            _logger.LogInformation("Authentication state change notification sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in NotifyUserAuthentication");
            NotifyUserLogout();
        }
    }

    public void NotifyUserLogout()
    {
        var identity = new ClaimsIdentity();
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            var claims = token.Claims.ToList();
            
            _logger.LogInformation("JWT Token claims:");
            foreach (var claim in claims)
            {
                _logger.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);
            }
            
            return claims;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing JWT claims");
            return new List<Claim>();
        }
    }
} 