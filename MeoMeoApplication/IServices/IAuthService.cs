using MeoMeo.Contract.DTOs.Auth;
using MeoMeo.Contract.Commons;

namespace MeoMeo.Application.IServices;

public interface IAuthService
{
    Task<AuthenResponse> LoginAsync(AuthenRequest input);
    Task LogoutAsync(RefreshTokenRequest input);
    Task<AuthenResponse> RefreshTokenAsync(RefreshTokenRequest input);
    Task<BaseResponse> ForgotPasswordAsync(ForgotPasswordRequest input);
}