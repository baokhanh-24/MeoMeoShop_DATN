using MeoMeo.Contract.DTOs;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Services;
using MeoMeo.Shared.Utilities;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using MeoMeo.PORTAL.IServices;

namespace MeoMeo.PORTAL.Services
{
    
    public class UserInfoService : IUserInfoService
    {
        private readonly ICustomerClientService _customerService;
        private readonly CustomAuthStateProvider _authStateProvider;
        private CustomerDTO? _currentUser;
        private bool _isLoading = false;
        private DateTime _lastUpdate = DateTime.MinValue;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

        public event Action? OnUserInfoChanged;

        public CustomerDTO? CurrentUser => _currentUser;
        public bool IsLoading => _isLoading;

        public UserInfoService(ICustomerClientService customerService, CustomAuthStateProvider authStateProvider)
        {
            _customerService = customerService;
            _authStateProvider = authStateProvider;
        }

        public async Task LoadUserInfoAsync()
        {
            if (_currentUser != null && DateTime.Now - _lastUpdate < _cacheDuration)
            {
                return; // Use cached data
            }

            await RefreshUserInfoAsync();
        }

        public async Task RefreshUserInfoAsync()
        {
            try
            {
                _isLoading = true;
                var authState = await _authStateProvider.GetAuthenticationStateAsync();
                var customerIdClaim = authState.User.FindFirst("CustomerId");

                if (customerIdClaim != null && Guid.TryParse(customerIdClaim.Value, out var customerId))
                {
                    _currentUser = await _customerService.GetCustomersByIdAsync(customerId);
                    _lastUpdate = DateTime.Now;
                    OnUserInfoChanged?.Invoke();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading user info: {ex.Message}");
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
