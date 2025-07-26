using MeoMeo.Contract.DTOs.Auth;

namespace MeoMeo.Application.IServices;

public interface IAuthService
{
    Task<AuthenResponse> LoginAsync(AuthenRequest input);
    Task LogoutAsync(RefreshTokenRequest input);

    Task<AuthenResponse> RefreshTokenAsync(RefreshTokenRequest input);
}