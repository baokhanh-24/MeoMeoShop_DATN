using System.IdentityModel.Tokens.Jwt;
using MeoMeo.Contract.DTOs.Auth;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace MeoMeo.Shared.Services;

public class AuthClientService : IAuthClientService
{
    private readonly IApiCaller _apiCaller;
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<AuthClientService> _logger;

    public AuthClientService(IApiCaller apiCaller, IJSRuntime jsRuntime, ILogger<AuthClientService> logger)
    {
        _apiCaller = apiCaller;
        _jsRuntime = jsRuntime;
        _logger = logger;
    }

    public async Task<AuthenResponse?> LoginAsync(AuthenRequest request)
    {
        try
        {
            var authResponse = await _apiCaller.PostAsync<AuthenRequest, AuthenResponse>("api/auths/connect-token", request);

            if (authResponse?.ResponseStatus == MeoMeo.Contract.Commons.BaseStatus.Success)
            {
                await SetTokensAsync(authResponse.AccessToken, authResponse.RefreshToken);
                return authResponse;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return null;
        }
    }

    public async Task<bool> LogoutAsync(RefreshTokenRequest request)
    {
        try
        {
            var response = await _apiCaller.PostAsync<RefreshTokenRequest, object>("api/auths/logout", request);
            await ClearTokensAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return false;
        }
    }

    public async Task<AuthenResponse?> RefreshTokenAsync(RefreshTokenRequest request)
    {
        try
        {
            var authResponse = await _apiCaller.PostAsync<RefreshTokenRequest, AuthenResponse>("api/auths/refresh-token", request);

            if (authResponse?.ResponseStatus == MeoMeo.Contract.Commons.BaseStatus.Success)
            {
                await SetTokensAsync(authResponse.AccessToken, authResponse.RefreshToken);
                return authResponse;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return null;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetAccessTokenAsync();
        if (string.IsNullOrEmpty(token))
            return false;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            return jwtToken.ValidTo > DateTime.UtcNow;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "accessToken");
        }
        catch
        {
            return null;
        }
    }

    public async Task<string?> GetRefreshTokenAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "refreshToken");
        }
        catch
        {
            return null;
        }
    }

    public async Task SetTokensAsync(string accessToken, string refreshToken)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "accessToken", accessToken);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "refreshToken", refreshToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting tokens");
        }
    }

    public async Task ClearTokensAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "accessToken");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "refreshToken");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing tokens");
        }
    }

    public async Task<bool> IsTokenExpiredAsync()
    {
        var token = await GetAccessTokenAsync();
        if (string.IsNullOrEmpty(token))
            return true;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            return jwtToken.ValidTo <= DateTime.UtcNow;
        }
        catch
        {
            return true;
        }
    }
}