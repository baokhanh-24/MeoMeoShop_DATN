
using MeoMeo.Shared.Utilities;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace MeoMeo.Shared.Middlewares;

public class AuthenticationHttpMessageHandler : DelegatingHandler
{
    private readonly ILogger<AuthenticationHttpMessageHandler> _logger;
    private const string StorageKey = "accessToken";
    private const string RefreshStorageKey = "refreshToken";
    private readonly ProtectedLocalStorage _localStorage;
    public AuthenticationHttpMessageHandler(ILogger<AuthenticationHttpMessageHandler> logger, ProtectedLocalStorage localStorage)
    {
        _logger = logger;
        _localStorage = localStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            var token =(await _localStorage.GetAsync<string>(StorageKey)).Value;
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            
            if (!string.IsNullOrEmpty(token))
            {
                // Thêm Authorization header
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding authentication header");
        }

        var response = await base.SendAsync(request, cancellationToken);

        // Kiểm tra nếu token hết hạn (401 Unauthorized)
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            try
            {
                // Thử refresh token
                var refreshToken =  (await _localStorage.GetAsync<string>(RefreshStorageKey)).Value;
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    // TODO: Implement token refresh logic
                    // var authService = request.GetType().GetProperty("Services")?.GetValue(request) as IAuthService;
                    // if (authService != null)
                    // {
                    //     var refreshRequest = new RefreshTokenRequest { RefreshToken = refreshToken };
                    //     var authResponse = await authService.RefreshTokenAsync(refreshRequest);
                    //     if (authResponse?.ResponseStatus == MeoMeo.Contract.Commons.BaseStatus.Success)
                    //     {
                    //         // Retry the original request with new token
                    //         request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResponse.AccessToken);
                    //         return await base.SendAsync(request, cancellationToken);
                    //     }
                    // }
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
            }
        }

        return response;
    }
} 