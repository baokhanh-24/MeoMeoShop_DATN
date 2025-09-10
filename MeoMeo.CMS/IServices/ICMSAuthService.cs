using MeoMeo.Contract.DTOs.Auth;

namespace MeoMeo.CMS.IServices
{
    public interface ICMSAuthService
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
        Task<UserDTO?> GetCurrentUserAsync();
        Task<bool> HasRoleAsync(string role);
        Task<bool> HasPermissionAsync(string permission);
        Task<List<string>> GetUserRolesAsync();
        Task<List<string>> GetUserPermissionsAsync();
        Task<Dictionary<string, string>> GetTokenClaimsAsync();
    }
}
