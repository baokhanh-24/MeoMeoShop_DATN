using MeoMeo.Contract.DTOs.Auth;
using MeoMeo.Contract.Commons;

namespace MeoMeo.Shared.IServices;

public interface IAuthClientService
{
    Task<AuthenResponse?> LoginAsync(AuthenRequest request);
    Task<bool> LogoutAsync(RefreshTokenRequest request);
    Task<AuthenResponse?> RefreshTokenAsync(RefreshTokenRequest request);
    Task<bool> IsAuthenticatedAsync();
    Task<string?> GetAccessTokenAsync();
    Task<string?> GetRefreshTokenAsync();
    Task SetTokensAsync(string accessToken, string refreshToken);
    Task ClearTokensAsync();
    Task<bool> IsTokenExpiredAsync();
    Task<BaseResponse?> ForgotPasswordAsync(ForgotPasswordRequest request);
}