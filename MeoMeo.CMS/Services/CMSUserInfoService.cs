using MeoMeo.CMS.IServices;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Auth;
using MeoMeo.Shared.IServices;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace MeoMeo.CMS.Services
{
    public class CMSUserInfoService : ICMSUserInfoService
    {
        private readonly ICMSAuthService _authService;
        private readonly IEmployeesClientService _employeeService;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILogger<CMSUserInfoService> _logger;

        private UserDTO? _currentUser;
        private DateTime _lastUpdate = DateTime.MinValue;
        private bool _isLoading = false;

        public UserDTO? CurrentUser => _currentUser;
        public DateTime LastUpdate => _lastUpdate;
        public bool IsLoading => _isLoading;

        public event Action? OnUserInfoChanged;

        public CMSUserInfoService(
            ICMSAuthService authService,
            IEmployeesClientService employeeService,
            AuthenticationStateProvider authStateProvider,
            ILogger<CMSUserInfoService> logger)
        {
            _authService = authService;
            _employeeService = employeeService;
            _authStateProvider = authStateProvider;
            _logger = logger;
        }

        public async Task LoadUserInfoAsync()
        {
            try
            {
                _isLoading = true;
                var authState = await _authStateProvider.GetAuthenticationStateAsync();
                var userIdClaim = authState.User.FindFirst("id");
                var employeeIdClaim = authState.User.FindFirst("employeeId");

                _logger.LogInformation("Loading user info for userId: {UserId}, employeeId: {EmployeeId}",
                    userIdClaim?.Value, employeeIdClaim?.Value);

                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    // Thử lấy từ token trước để đảm bảo có user info
                    _currentUser = await _authService.GetCurrentUserAsync();

                    if (_currentUser != null)
                    {
                        _logger.LogInformation("Got user from token: {UserName}", _currentUser.UserName);

                        // Sau đó thử fetch thêm thông tin từ backend
                        try
                        {
                            // Sử dụng EmployeeId từ token hoặc từ user.EmployeeId
                            var employeeId = _currentUser.EmployeeId;
                            if (employeeIdClaim != null && Guid.TryParse(employeeIdClaim.Value, out var empIdFromToken))
                            {
                                employeeId = empIdFromToken;
                            }

                            if (employeeId.HasValue)
                            {
                                var userResponse = await _employeeService.GetEmployeesByIdAsync(employeeId.Value);
                                var avatarUrl = await _employeeService.GetAvatarUrlAsync();

                                _logger.LogInformation("Employee response status: {Status}, Avatar URL: {AvatarUrl}",
                                    userResponse.ResponseStatus, avatarUrl);

                                if (userResponse.ResponseStatus == MeoMeo.Contract.Commons.BaseStatus.Success)
                                {
                                    // Cập nhật avatar nếu có
                                    if (!string.IsNullOrEmpty(avatarUrl))
                                    {
                                        _currentUser.Avatar = avatarUrl;
                                    }
                                    _currentUser.Email=userResponse.Email;
                                    _currentUser.UserName=userResponse.UserName;
                                    _currentUser.FullName = userResponse.Name;
                                    // Map Employee data
                                    _currentUser.Employee = new EmployeeDTO
                                    {
                                        Id = userResponse.Id,
                                        UserId = userResponse.UserId,
                                        Name = userResponse.Name,
                                        Code = userResponse.Code,
                                        PhoneNumber = userResponse.PhoneNumber,
                                        DateOfBird = userResponse.DateOfBird,
                                        Address = userResponse.Address,
                                        Status = (int)userResponse.Status
                                    };
                                    _logger.LogInformation("Successfully updated user info with employee data");
                                }
                            }
                            else
                            {
                                _logger.LogWarning("No EmployeeId found in token or user data");
                            }
                        }
                        catch (Exception backendEx)
                        {
                            _logger.LogWarning(backendEx, "Failed to fetch additional data from backend, using token data only");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Failed to get user from token");
                    }

                    _lastUpdate = DateTime.Now;
                    OnUserInfoChanged?.Invoke();
                    _logger.LogInformation("CMS User info loaded successfully for user: {UserId}", userId);
                }
                else
                {
                    _logger.LogWarning("No valid userId found in claims");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading CMS user info: {Message}", ex.Message);
                _currentUser = null;
            }
            finally
            {
                _isLoading = false;
            }
        }

        public async Task RefreshUserInfoAsync()
        {
            try
            {
                _isLoading = true;
                var authState = await _authStateProvider.GetAuthenticationStateAsync();
                var userIdClaim = authState.User.FindFirst("id");
                var employeeIdClaim = authState.User.FindFirst("employeeId");

                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    // Thử lấy từ token trước để đảm bảo có user info
                    _currentUser = await _authService.GetCurrentUserAsync();

                    if (_currentUser != null)
                    {
                        // Sau đó thử fetch thêm thông tin từ backend
                        try
                        {
                            // Sử dụng EmployeeId từ token hoặc từ user.EmployeeId
                            var employeeId = _currentUser.EmployeeId;
                            if (employeeIdClaim != null && Guid.TryParse(employeeIdClaim.Value, out var empIdFromToken))
                            {
                                employeeId = empIdFromToken;
                            }

                            if (employeeId.HasValue)
                            {
                                var userResponse = await _employeeService.GetEmployeesByIdAsync(employeeId.Value);
                                var avatarUrl = await _employeeService.GetAvatarUrlAsync();

                                if (userResponse.ResponseStatus == MeoMeo.Contract.Commons.BaseStatus.Success)
                                {
                                    // Cập nhật avatar nếu có
                                    if (!string.IsNullOrEmpty(avatarUrl))
                                    {
                                        _currentUser.Avatar = avatarUrl;
                                    }

                                    // Map Employee data
                                    _currentUser.Employee = new EmployeeDTO
                                    {
                                        Id = userResponse.Id,
                                        UserId = userResponse.UserId,
                                        Name = userResponse.Name,
                                        Code = userResponse.Code,
                                        PhoneNumber = userResponse.PhoneNumber,
                                        DateOfBird = userResponse.DateOfBird,
                                        Address = userResponse.Address,
                                        Status = (int)userResponse.Status
                                    };

                                    // Cập nhật FullName từ Employee.Name
                                    _currentUser.FullName = userResponse.Name;
                                }
                            }
                        }
                        catch (Exception backendEx)
                        {
                            _logger.LogWarning(backendEx, "Failed to fetch additional data from backend, using token data only");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Failed to get user from token during refresh");
                    }

                    _lastUpdate = DateTime.Now;
                    OnUserInfoChanged?.Invoke();
                    _logger.LogInformation("CMS User info refreshed successfully for user: {UserId}", userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing CMS user info: {Message}", ex.Message);
                // Fallback về token nếu có lỗi
                try
                {
                    _currentUser = await _authService.GetCurrentUserAsync();
                }
                catch (Exception fallbackEx)
                {
                    _logger.LogError(fallbackEx, "Fallback to token also failed: {Message}", fallbackEx.Message);
                }
            }
            finally
            {
                _isLoading = false;
            }
        }

        public void ClearUserInfo()
        {
            _currentUser = null;
            _lastUpdate = DateTime.MinValue;
            OnUserInfoChanged?.Invoke();
        }
    }
}
