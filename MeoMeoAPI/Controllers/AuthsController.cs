using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.DTOs.Auth;
using MeoMeo.Domain.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthsController: ControllerBase
{
    private readonly IAuthService _authService;

    public AuthsController(IAuthService authService)
    {
        _authService = authService;
    }
    [HttpPost("connect-token")]
    public Task<AuthenResponse> LoginAsync(AuthenRequest input) => _authService.LoginAsync(input);

    [HttpPost("logout")]
    public Task LogoutAsync(RefreshTokenRequest input) => _authService.LogoutAsync(input);

    [HttpPost("refresh-token")]
    public Task<AuthenResponse> RefreshTokenAsync(RefreshTokenRequest input) => _authService.RefreshTokenAsync(input);
    
}