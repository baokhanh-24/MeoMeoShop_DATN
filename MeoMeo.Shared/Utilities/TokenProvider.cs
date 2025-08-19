
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;

namespace MeoMeo.Shared.Utilities;

public class TokenProvider : ITokenProvider
{
    private const string StorageKey = "accessToken";
    private const string RefreshStorageKey = "refreshToken";
    private readonly ProtectedLocalStorage _localStorage;
    private readonly ILogger<TokenProvider> _logger;

    public TokenProvider( ILogger<TokenProvider> logger, ProtectedLocalStorage localStorage)
    {
        _logger = logger;
        _localStorage = localStorage;
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        try
        {
            
            var token = (await _localStorage.GetAsync<string>(StorageKey)).Value;
            return string.IsNullOrWhiteSpace(token) ? null : token;
        }
        catch (Exception jsEx)
        {
            _logger.LogError(jsEx, "Failed to read access token from localStorage (JSException)");
            return null;
        }
    }

    public async Task<string?> GetRefreshTokenAsync()
    {
        try
        {
            var token = (await _localStorage.GetAsync<string>(RefreshStorageKey)).Value;
            return string.IsNullOrWhiteSpace(token) ? null : token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get refresh token from localStorage");
            return null;
        }
    }

    public async Task SetAccessTokenAsync(string token)
    {
        try
        {
            await _localStorage.SetAsync(StorageKey, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set access token in localStorage");
        }
    }

    public async Task SetRefreshTokenAsync(string token)
    {
        try
        {
            await _localStorage.SetAsync(RefreshStorageKey, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set refresh token in localStorage");
        }
    }

    public async Task ClearAsync()
    {
        try
        {
            await _localStorage.DeleteAsync(StorageKey);
            await _localStorage.DeleteAsync(RefreshStorageKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear tokens from localStorage");
        }
    }
}
