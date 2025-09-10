using MeoMeo.Contract.DTOs;

namespace MeoMeo.PORTAL.IServices
{
    public interface IUserInfoService
    {
        CustomerDTO? CurrentUser { get; }
        bool IsLoading { get; }
        event Action? OnUserInfoChanged;
        Task LoadUserInfoAsync();
        Task RefreshUserInfoAsync();
        void ClearUserInfo();
    }
}