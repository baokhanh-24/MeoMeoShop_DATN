using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Auth;

namespace MeoMeo.Application.IServices
{
    public interface IPasswordResetService
    {
        Task<BaseResponse> RequestPasswordResetAsync(string email);
        Task<BaseResponse> ResetPasswordAsync(string resetToken, string newPassword);
        Task<BaseResponse> ValidateResetTokenAsync(string resetToken);
    }
}
