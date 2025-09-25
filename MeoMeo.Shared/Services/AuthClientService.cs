using System.IdentityModel.Tokens.Jwt;
using MeoMeo.Contract.DTOs.Auth;
using MeoMeo.Contract.Commons;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Utilities;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace MeoMeo.Shared.Services;

public class AuthClientService : IAuthClientService
{
    private readonly IApiCaller _apiCaller;
    private readonly ILogger<AuthClientService> _logger;
    private readonly ProtectedLocalStorage _localStorage;
    private const string StorageKey = "accessToken";
    private const string RefreshStorageKey = "refreshToken";
    public AuthClientService(IApiCaller apiCaller, ILogger<AuthClientService> logger, ProtectedLocalStorage localStorage)
    {
        _apiCaller = apiCaller;
        _logger = logger;
        _localStorage = localStorage;
    }

    public async Task<AuthenResponse?> LoginAsync(AuthenRequest request)
    {
        try
        {
            var authResponse = await _apiCaller.PostAsync<AuthenRequest, AuthenResponse>("api/auths/connect-token", request);

            if (authResponse?.ResponseStatus == MeoMeo.Contract.Commons.BaseStatus.Success)
            {
                await SetTokensAsync(authResponse.AccessToken, authResponse.RefreshToken);
            }

            return authResponse;
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
            return (await _localStorage.GetAsync<string>(StorageKey)).Value;
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
            return (await _localStorage.GetAsync<string>(RefreshStorageKey)).Value;
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
            await _localStorage.SetAsync(StorageKey, accessToken);
            await _localStorage.SetAsync(RefreshStorageKey, refreshToken);
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
            await _localStorage.DeleteAsync(StorageKey);
            await _localStorage.DeleteAsync(RefreshStorageKey);
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

    public async Task<BaseResponse?> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        try
        {
            var response = await _apiCaller.PostAsync<ForgotPasswordRequest, BaseResponse>("api/auths/forgot-password", request);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during forgot password");
            return new BaseResponse
            {
                ResponseStatus = BaseStatus.Error,
                Message = "Có lỗi xảy ra trong quá trình xử lý. Vui lòng thử lại!"
            };
        }
    }
}