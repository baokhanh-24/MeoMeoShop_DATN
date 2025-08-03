
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace MeoMeo.Shared.Middlewares;

public class AuthenticationHttpMessageHandler : DelegatingHandler
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<AuthenticationHttpMessageHandler> _logger;

    public AuthenticationHttpMessageHandler(IJSRuntime jsRuntime, ILogger<AuthenticationHttpMessageHandler> logger)
    {
        _jsRuntime = jsRuntime;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            // Lấy token từ localStorage
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "accessToken");
            
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
                var refreshToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "refreshToken");
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

                // Nếu refresh token cũng thất bại, xóa tokens và redirect về login
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "accessToken");
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "refreshToken");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
            }
        }

        return response;
    }
} 