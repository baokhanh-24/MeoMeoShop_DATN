using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MeoMeo.CMS.IServices;
using MeoMeo.Contract.DTOs.Auth;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Utilities;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;

namespace MeoMeo.CMS.Services
{
    public class CMSAuthService : ICMSAuthService
    {
        private readonly IApiCaller _apiCaller;
        private readonly ILogger<CMSAuthService> _logger;
        private readonly ProtectedLocalStorage _localStorage;
        private readonly IUserRoleClientService _userRoleService;
        private const string StorageKey = "accessToken";
        private const string RefreshStorageKey = "refreshToken";
        private const string UserStorageKey = "userInfo";

        public CMSAuthService(
            IApiCaller apiCaller,
            ILogger<CMSAuthService> logger,
            ProtectedLocalStorage localStorage,
            IUserRoleClientService userRoleService)
        {
            _apiCaller = apiCaller;
            _logger = logger;
            _localStorage = localStorage;
            _userRoleService = userRoleService;
        }

        public async Task<AuthenResponse?> LoginAsync(AuthenRequest request)
        {
            try
            {
                var authResponse = await _apiCaller.PostAsync<AuthenRequest, AuthenResponse>("api/auths/connect-token", request);
                if (authResponse?.ResponseStatus == MeoMeo.Contract.Commons.BaseStatus.Success)
                {
                    await SetTokensAsync(authResponse.AccessToken, authResponse.RefreshToken);

                    // Lưu thông tin user
                    var userInfo = await GetUserInfoFromToken(authResponse.AccessToken);
                    if (userInfo != null)
                    {
                        await _localStorage.SetAsync(UserStorageKey, userInfo);
                    }
                }
                return authResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during CMS login");
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
                _logger.LogError(ex, "Error during CMS logout");
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

                    // Cập nhật thông tin user
                    var userInfo = await GetUserInfoFromToken(authResponse.AccessToken);
                    if (userInfo != null)
                    {
                        await _localStorage.SetAsync(UserStorageKey, userInfo);
                    }

                    return authResponse;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during CMS token refresh");
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
                _logger.LogError(ex, "Error setting CMS tokens");
            }
        }

        public async Task ClearTokensAsync()
        {
            try
            {
                await _localStorage.DeleteAsync(StorageKey);
                await _localStorage.DeleteAsync(RefreshStorageKey);
                await _localStorage.DeleteAsync(UserStorageKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing CMS tokens");
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

        public async Task<UserDTO?> GetCurrentUserAsync()
        {
            try
            {
                var userInfo = (await _localStorage.GetAsync<UserDTO>(UserStorageKey)).Value;
                if (userInfo != null)
                    return userInfo;

                // Nếu không có trong localStorage, lấy từ token
                var token = await GetAccessTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    return await GetUserInfoFromToken(token);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> HasRoleAsync(string role)
        {
            try
            {
                var token = await GetAccessTokenAsync();
                if (string.IsNullOrEmpty(token))
                    return false;

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                // Đọc roles từ token claims (theo cấu trúc thực tế)
                var rolesClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "roles");
                if (rolesClaim != null)
                {
                    var roles = rolesClaim.Value.Split(';');
                    return roles.Any(r => r.Trim().ToLower() == role.ToLower());
                }

                // Fallback: kiểm tra adminOrUser claim
                var adminOrUserClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "adminOrUser");
                if (adminOrUserClaim != null)
                {
                    return adminOrUserClaim.Value.ToLower() == role.ToLower();
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking role from token");
                return false;
            }
        }

        public async Task<bool> HasPermissionAsync(string permission)
        {
            try
            {
                var token = await GetAccessTokenAsync();
                if (string.IsNullOrEmpty(token))
                    return false;

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                // Đọc permissions từ token claims (theo cấu trúc thực tế)
                var permissionsClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "permissions");
                if (permissionsClaim != null)
                {
                    var permissions = permissionsClaim.Value.Split(';');
                    return permissions.Any(p => p.Trim().ToLower() == permission.ToLower());
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking permission from token");
                return false;
            }
        }

        private Task<UserDTO?> GetUserInfoFromToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                // Đọc userId từ claim "id" (theo cấu trúc thực tế)
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    // Xử lý FullName có thể là array
                    var fullNameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "fullName");
                    string fullName = "";
                    if (fullNameClaim != null)
                    {
                        fullName = fullNameClaim.Value;
                    }
                    // Xử lý EmployeeId và CustomerId
                    Guid? employeeId = null;
                    Guid? customerId = null;

                    var employeeIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "employeeId");
                    if (employeeIdClaim != null && !string.IsNullOrEmpty(employeeIdClaim.Value) &&
                        Guid.TryParse(employeeIdClaim.Value, out var empId))
                    {
                        employeeId = empId;
                    }

                    var customerIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "customerId");
                    if (customerIdClaim != null && !string.IsNullOrEmpty(customerIdClaim.Value) &&
                        Guid.TryParse(customerIdClaim.Value, out var custId))
                    {
                        customerId = custId;
                    }

                    return Task.FromResult<UserDTO?>(new UserDTO
                    {
                        Id = userId,
                        UserName = jwtToken.Claims.FirstOrDefault(c => c.Type == "userName")?.Value ?? "",
                        Email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "",
                        FullName = fullName,
                        EmployeeId = employeeId,
                        CustomerId = customerId
                    });
                }

                return Task.FromResult<UserDTO?>(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing user info from token");
                return Task.FromResult<UserDTO?>(null);
            }
        }

        public async Task<List<string>> GetUserRolesAsync()
        {
            try
            {
                var token = await GetAccessTokenAsync();
                if (string.IsNullOrEmpty(token))
                    return new List<string>();

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                // Đọc roles từ token claims (theo cấu trúc thực tế)
                var rolesClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "roles");
                if (rolesClaim != null)
                {
                    return await Task.FromResult(rolesClaim.Value.Split(';')
                        .Select(r => r.Trim())
                        .Where(r => !string.IsNullOrEmpty(r))
                        .ToList());
                }

                // Fallback: kiểm tra adminOrUser claim
                var adminOrUserClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "adminOrUser");
                if (adminOrUserClaim != null)
                {
                    return await Task.FromResult(new List<string> { adminOrUserClaim.Value });
                }

                return await Task.FromResult(new List<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user roles from token");
                return new List<string>();
            }
        }

        public async Task<List<string>> GetUserPermissionsAsync()
        {
            try
            {
                var token = await GetAccessTokenAsync();
                if (string.IsNullOrEmpty(token))
                    return new List<string>();

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                // Đọc permissions từ token claims (theo cấu trúc thực tế)
                var permissionsClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "permissions");
                if (permissionsClaim != null)
                {
                    return await Task.FromResult(permissionsClaim.Value.Split(';')
                        .Select(p => p.Trim())
                        .Where(p => !string.IsNullOrEmpty(p))
                        .ToList());
                }

                return await Task.FromResult(new List<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user permissions from token");
                return new List<string>();
            }
        }

        public async Task<Dictionary<string, string>> GetTokenClaimsAsync()
        {
            try
            {
                var token = await GetAccessTokenAsync();
                if (string.IsNullOrEmpty(token))
                    return new Dictionary<string, string>();

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                return jwtToken.Claims.ToDictionary(c => c.Type, c => c.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting token claims");
                return new Dictionary<string, string>();
            }
        }

        public async Task RefreshUserInfoAsync()
        {
            try
            {
                var token = await GetAccessTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    // Cập nhật thông tin user từ token mới nhất
                    var userInfo = await GetUserInfoFromToken(token);
                    if (userInfo != null)
                    {
                        await _localStorage.SetAsync(UserStorageKey, userInfo);
                        _logger.LogInformation("CMS User info refreshed successfully");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing CMS user info");
            }
        }
    }
}
