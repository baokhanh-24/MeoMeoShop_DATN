using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Auth;
using MeoMeo.Shared.Utilities;

namespace MeoMeo.Shared.IServices
{
    public interface IUserProfileClientService
    {
        Task<UserDTO?> GetCurrentUserAsync();
        Task<BaseResponse> UpdateUserProfileAsync(CreateOrUpdateUserDTO user);
        Task<BaseResponse> ChangePasswordAsync(ChangePasswordDTO changePasswordDto);
    }
}
