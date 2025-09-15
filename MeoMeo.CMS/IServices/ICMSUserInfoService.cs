using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Auth;

namespace MeoMeo.CMS.IServices
{
    public interface ICMSUserInfoService
    {
        UserDTO? CurrentUser { get; }
        DateTime LastUpdate { get; }
        bool IsLoading { get; }

        event Action? OnUserInfoChanged;

        Task LoadUserInfoAsync();
        Task RefreshUserInfoAsync();
        void ClearUserInfo();
    }
}
