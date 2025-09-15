using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MeoMeo.CMS.IServices;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace MeoMeo.CMS.Utilities
{
    public class CMSAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ICMSAuthService _authService;
        private readonly ILogger<CMSAuthStateProvider> _logger;

        public CMSAuthStateProvider(ICMSAuthService authService, ILogger<CMSAuthStateProvider> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var isAuthenticated = await _authService.IsAuthenticatedAsync();
                _logger.LogInformation("CMS IsAuthenticated: {IsAuthenticated}", isAuthenticated);

                if (!isAuthenticated)
                {
                    _logger.LogInformation("CMS User is not authenticated, returning anonymous state");
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                var token = await _authService.GetAccessTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("CMS Token is null or empty");
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                var claims = ParseClaimsFromJwt(token);
                _logger.LogInformation("CMS Parsed {ClaimCount} claims from JWT", claims.Count());

                var identity = new ClaimsIdentity(claims, "jwt");
                var user = new ClaimsPrincipal(identity);

                _logger.LogInformation("CMS Created authenticated user: {UserName}",
                    user.FindFirst("UserName")?.Value ?? "Unknown");

                return new AuthenticationState(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting CMS authentication state");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public void NotifyUserAuthentication(string token)
        {
            try
            {
                _logger.LogInformation("CMS NotifyUserAuthentication called with token length: {TokenLength}", token?.Length ?? 0);

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Attempted to notify CMS authentication with empty token");
                    NotifyUserLogout();
                    return;
                }

                var claims = ParseClaimsFromJwt(token);
                _logger.LogInformation("CMS Parsed {ClaimCount} claims from token", claims.Count());

                if (!claims.Any())
                {
                    _logger.LogWarning("No claims found in CMS token");
                    NotifyUserLogout();
                    return;
                }

                var identity = new ClaimsIdentity(claims, "jwt");
                var user = new ClaimsPrincipal(identity);

                _logger.LogInformation("CMS Notifying authentication state change for user: {UserName}",
                    user.FindFirst("UserName")?.Value ?? "Unknown");

                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
                _logger.LogInformation("CMS Authentication state change notification sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CMS NotifyUserAuthentication");
                NotifyUserLogout();
            }
        }

        public void NotifyUserLogout()
        {
            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public async Task NotifyUserInfoRefresh()
        {
            try
            {
                _logger.LogInformation("CMS NotifyUserInfoRefresh called");

                // Refresh user info trong auth service
                await _authService.RefreshUserInfoAsync();

                // Notify authentication state change để các component reload
                var authState = await GetAuthenticationStateAsync();
                NotifyAuthenticationStateChanged(Task.FromResult(authState));

                _logger.LogInformation("CMS User info refresh notification sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CMS NotifyUserInfoRefresh");
            }
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                var claims = token.Claims.ToList();

                _logger.LogInformation("CMS JWT Token claims:");
                foreach (var claim in claims)
                {
                    _logger.LogInformation("CMS Claim: {Type} = {Value}", claim.Type, claim.Value);
                }

                return claims;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing CMS JWT claims");
                return new List<Claim>();
            }
        }
    }
}
