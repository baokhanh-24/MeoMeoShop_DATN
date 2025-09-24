using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Auth;
using MeoMeo.Contract.DTOs.Permission;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.Shared.Constants;
using MeoMeo.Shared.Utilities;
using MeoMeo.Shared.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace MeoMeo.Application.Services;

public class AuthService : IAuthService
{
    private IRefreshTokenRepository _refreshTokenRepository { get; }
    private IRoleRepository _roleRepository { get; }
    private IUserRoleRepository _userRoleRepository { get; }
    private IUserTokenRepository _userTokenRepository { get; }
    private IUserRepository _userRepository { get; }
    private ICustomerRepository _customerRepository { get; }
    private IEmployeeRepository _employeeRepository { get; }
    private IMapper _mapper { get; }
    private IConfiguration _configuration { get; }
    private IEmailService _emailService { get; }
    private ILogger<AuthService> _logger { get; }

    public AuthService(IRefreshTokenRepository refreshTokenRepository, IRoleRepository roleRepository, IUserRoleRepository userRoleRepository, IUserTokenRepository userTokenRepository, IUserRepository userRepository, IMapper mapper, ICustomerRepository customerRepository, IEmployeeRepository employeeRepository, IConfiguration configuration, IEmailService emailService, ILogger<AuthService> logger)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _userTokenRepository = userTokenRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _customerRepository = customerRepository;
        _employeeRepository = employeeRepository;
        _configuration = configuration;
        _emailService = emailService;
        _logger = logger;
    }


    private const int ExpriesIn = 86400 * 365 * 2;


    public async Task<AuthenResponse> LoginAsync(AuthenRequest input)
    {
        try
        {
            string hashPW = FunctionHelper.ComputerSha256Hash(input.Password);
            var existedUser = await _userRepository.Query().FirstOrDefaultAsync(c => c.UserName == input.UserName && c.PasswordHash == hashPW);
            if (existedUser is null)
            {
                return new AuthenResponse()
                {
                    Message = "Thông tin tài khoản hoặc mật khẩu không chính xác xin vui lòng thử lại",
                    ResponseStatus = BaseStatus.Error
                };
            }

            if (existedUser.IsLocked)
            {
                string mess = "Tài khoản đang bị khóa. Vui lòng hiên hệ với quản trị viên";
                if (existedUser.LockedEndDate > DateTime.Now)
                {
                    mess = $"Tài khoản của bị khóa đến {existedUser.LockedEndDate}";
                }
                return new AuthenResponse { ResponseStatus = BaseStatus.Error, Message = mess };
            }
            var userRoles = await _userRoleRepository.GetRolesByUserId(existedUser.Id);
            var roles = await _roleRepository.GetNameByRoleIds(userRoles.Select(c => c.RoleId));
            var userMapped = _mapper.Map<UserDTO>(existedUser);
            if (roles.Count > 0 && roles.Contains("Customer"))
            {
                var customer = await _customerRepository.Query().FirstOrDefaultAsync(c => c.UserId == userMapped.Id);
                if (customer != null)
                {
                    userMapped.FullName = customer.Name;
                    userMapped.CustomerId = customer.Id;
                }
            }
            else if (roles.Count > 0 && (roles.Contains("Admin") || roles.Contains("Employee")))
            {
                var employee = await _employeeRepository.Query().FirstOrDefaultAsync(c => c.UserId == userMapped.Id);
                if (employee != null)
                {
                    userMapped.FullName = employee.Name;
                    userMapped.EmployeeId = employee.Id;
                }
            }

            string token = GenerateToken(userMapped, roles);
            var userToken = new UserToken
            {
                UserId = existedUser.Id,
                AccessToken = token,
                RefreshToken = RandomString(25) + Guid.NewGuid(),
                ExpiryDate = DateTime.Now.AddYears(7),
                IsRevoked = false,
                IsUsed = false,
                IsUpdateToken = false,
            };
            await _userTokenRepository.UpdateRevokeAllOldTokenAsync(existedUser.Id);
            await _userTokenRepository.SaveAccessTokenAsync(userToken);
            existedUser.LastLogin = DateTime.Now;
            await _userRepository.UpdateAsync(existedUser);
            return new AuthenResponse
            {
                ResponseStatus = BaseStatus.Success,
                AccessToken = token,
                TokenType = JwtBearerDefaults.AuthenticationScheme,
                ExpriesIn = ExpriesIn,
                RefreshToken = userToken.RefreshToken
            };
        }
        catch (Exception ex)
        {
            return new AuthenResponse
            {
                ResponseStatus = BaseStatus.Error,
                Message = "Có lỗi xảy ra khi đăng nhập"
            };
        }
    }

    public async Task LogoutAsync(RefreshTokenRequest input)
    {
        try
        {
            // Find the user token by refresh token
            var userToken = await _userTokenRepository.Query()
                .FirstOrDefaultAsync(x => x.RefreshToken == input.RefreshToken);

            if (userToken != null)
            {
                if (userToken.IsUsed)
                {
                    throw new Exception("RefreshTokenUsed");
                }

                // Delete the user token
                await _userTokenRepository.DeleteAsync(userToken.Id);
            }

            // Revoke the token
            await _userTokenRepository.RevokeTokenAsync(Guid.Parse(input.RefreshToken));
        }
        catch (Exception ex)
        {
            // Log the exception if needed
            Console.WriteLine($"Logout error: {ex.Message}");
            throw;
        }
    }

    public async Task<AuthenResponse> RefreshTokenAsync(RefreshTokenRequest input)
    {
        try
        {
            var refreshToken = await _userTokenRepository.Query()
                .FirstOrDefaultAsync(x => x.RefreshToken == input.RefreshToken);

            if (refreshToken is null)
            {
                return new AuthenResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "RefreshTokenInvalid"
                };
            }

            if (refreshToken.IsRevoked && !refreshToken.IsUpdateToken)
            {
                return new AuthenResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "RefreshTokenRevoked"
                };
            }

            if (refreshToken.ExpiryDate < DateTime.Now)
            {
                return new AuthenResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "RefreshTokenExpired"
                };
            }

            if (!refreshToken.IsRevoked)
            {
                return new AuthenResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "RefreshTokenIsUsed"
                };
            }

            // Get the user
            var user = await _userRepository.GetUserByIdAsync(refreshToken.UserId);
            if (user is null)
            {
                return new AuthenResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "AccountNotFound"
                };
            }

            if (user.IsLocked)
            {
                return new AuthenResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "AccountInActive"
                };
            }

            // Get user roles and permissions
            var userRoles = await _userRoleRepository.GetRolesByUserId(user.Id);
            var roles = await _roleRepository.GetNameByRoleIds(userRoles.Select(c => c.RoleId));
            var userMapped = _mapper.Map<UserDTO>(user);
            if (roles.Count > 0 && roles.Contains("Customer"))
            {
                var customer = await _customerRepository.Query().FirstOrDefaultAsync(c => c.UserId == userMapped.Id);
                if (customer != null)
                {
                    userMapped.FullName = customer.Name;
                }
            }
            else if (roles.Count > 0 && (roles.Contains("Admin") || roles.Contains("Employee")))
            {
                var employee = await _employeeRepository.Query().FirstOrDefaultAsync(c => c.UserId == userMapped.Id);
                if (employee != null)
                {
                    userMapped.FullName = employee.Name;
                }
            }
            // Generate new access token
            string accessToken = GenerateToken(userMapped, roles);

            // Create new refresh token
            var newRefreshToken = RandomString(25) + Guid.NewGuid();

            // Create new user token
            var userTokenNew = new UserToken
            {
                UserId = user.Id,
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                ExpiryDate = DateTime.Now.AddYears(7),
                IsRevoked = false,
                IsUsed = false,
                IsUpdateToken = false,
            };

            // Save the new token
            await _userTokenRepository.SaveAccessTokenAsync(userTokenNew);

            return new AuthenResponse
            {
                ResponseStatus = BaseStatus.Success,
                AccessToken = accessToken,
                TokenType = JwtBearerDefaults.AuthenticationScheme,
                ExpriesIn = ExpriesIn,
                RefreshToken = newRefreshToken
            };
        }
        catch (Exception ex)
        {
            return new AuthenResponse
            {
                ResponseStatus = BaseStatus.Error,
                Message = ex.Message
            };
        }
    }

    public async Task<BaseResponse> ForgotPasswordAsync(ForgotPasswordRequest input)
    {
        try
        {
            // Tìm user theo UserName và Email
            var user = await _userRepository.Query()
                .FirstOrDefaultAsync(u => u.UserName == input.UserName && u.Email == input.Email);

            if (user == null)
            {
                return new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không tìm thấy tài khoản với thông tin đã cung cấp. Vui lòng kiểm tra lại tên đăng nhập và email."
                };
            }

            if (user.IsLocked)
            {
                return new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Tài khoản đang bị khóa. Vui lòng liên hệ với quản trị viên."
                };
            }

            // Tạo mật khẩu random mới
            var newPassword = GenerateRandomPassword();
            var hashedPassword = FunctionHelper.ComputerSha256Hash(newPassword);

            // Cập nhật mật khẩu mới vào database
            user.PasswordHash = hashedPassword;
            await _userRepository.UpdateAsync(user);

            // Gửi email với mật khẩu mới
            var emailSent = await _emailService.SendNewPasswordEmailAsync(
                user.Email,
                newPassword,
                user.UserName ?? "User");

            if (emailSent)
            {
                return new BaseResponse
                {
                    ResponseStatus = BaseStatus.Success,
                    Message = "Mật khẩu mới đã được gửi về email của bạn. Vui lòng kiểm tra hộp thư và đăng nhập lại."
                };
            }
            else
            {
                return new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không thể gửi email. Vui lòng thử lại sau."
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ForgotPasswordAsync");
            return new BaseResponse
            {
                ResponseStatus = BaseStatus.Error,
                Message = "Có lỗi xảy ra trong quá trình xử lý. Vui lòng thử lại."
            };
        }
    }

    private string GenerateRandomPassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private static string RandomString(int length)
    {
        var random = new Random();

        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        return new string(Enumerable.Repeat(chars, length)
                        .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private string GenerateToken(UserDTO user, List<string> roles)
    {
        var claims = new[]
        {
            new Claim(ClaimTypeConst.UserName, user.UserName ?? ""),
            new Claim(ClaimTypeConst.UserId, user.Id.ToString()),
            new Claim(ClaimTypeConst.AdminOrUser, roles.Contains("Admin") ? "Admin" : "User"),
            new Claim(ClaimTypeConst.Roles, string.Join(";", roles ?? new List<string>())),
            new Claim(ClaimTypeConst.Email, user.Email ?? ""),
            new Claim(ClaimTypeConst.Avatar, user.Avatar ?? ""),
            new Claim(ClaimTypeConst.FullName, user.FullName ?? ""),
            new Claim(ClaimTypeConst.CustomerId, user.CustomerId?.ToString() ?? ""),
            new Claim(ClaimTypeConst.EmployeeId, user.EmployeeId?.ToString() ?? ""),
        };

        return GenerateTokenByClaim(claims);
    }

    private string GenerateTokenByClaim(IEnumerable<Claim> claims)
    {
        var jwtKey = _configuration.GetSection("Jwt:Key").Value ?? throw new InvalidOperationException("JWT Key is not configured");
        var jwtIssuer = _configuration.GetSection("Jwt:Issuer").Value ?? throw new InvalidOperationException("JWT Issuer is not configured");
        var jwtAudience = _configuration.GetSection("Jwt:Audience").Value ?? throw new InvalidOperationException("JWT Audience is not configured");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(jwtIssuer,
            jwtAudience,
            claims,
            expires: DateTime.Now.AddSeconds(ExpriesIn),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}