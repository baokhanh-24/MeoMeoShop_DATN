using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Auth;
using MeoMeo.Contract.DTOs.PermissionGroup;
using MeoMeo.Contract.DTOs.Permission;
using MeoMeo.Contract.DTOs.RolePermission;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.Shared.Constants;
using MeoMeo.Shared.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MeoMeo.Application.Services;

public class AuthService:IAuthService
{
    private IRefreshTokenRepository _refreshTokenRepository { get; }
    private IRoleRepository _roleRepository { get; }
    private IUserRoleRepository _userRoleRepository { get; }
    private IPermissionRepository _permissionRepository { get; }
    private IPermissionGroupRepository _permissionGroupRepository { get; }
    private IUserTokenRepository _userTokenRepository { get; }
    private IUserRepository _userRepository { get; }
    private ICustomerRepository _customerRepository { get; }
    private IEmployeeRepository _employeeRepository { get; }
    private IMapper _mapper { get; }
    private IConfiguration _configuration { get; }

    public AuthService(IRefreshTokenRepository refreshTokenRepository, IRoleRepository roleRepository, IUserRoleRepository userRoleRepository, IPermissionRepository permissionRepository, IPermissionGroupRepository permissionGroupRepository, IUserTokenRepository userTokenRepository, IUserRepository userRepository, IMapper mapper, ICustomerRepository customerRepository, IEmployeeRepository employeeRepository, IConfiguration configuration)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _permissionRepository = permissionRepository;
        _permissionGroupRepository = permissionGroupRepository;
        _userTokenRepository = userTokenRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _customerRepository = customerRepository;
        _employeeRepository = employeeRepository;
        _configuration = configuration;
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
                return new AuthenResponse { ResponseStatus = BaseStatus.Error, Message =mess  };
            }
            var userRoles = await _userRoleRepository.GetRolesByUserId(existedUser.Id);
            var roles = await _roleRepository.GetNameByRoleIds(userRoles.Select(c=>c.RoleId));
            var permissionQuery = await GetListPermissionGroupByArrRoleId(existedUser.Id);
            var userMapped= _mapper.Map<UserDTO>(existedUser);
            if (roles.Count > 0 && roles.Contains("Customer"))
            {
                var customer = await _customerRepository.Query().FirstOrDefaultAsync(c => c.UserId == userMapped.Id);
                userMapped.FullName = customer.Name;
            }
            else if  (roles.Count > 0 && roles.Contains("Customer") ||  roles.Contains("Admin") )
            {
                var employee = await _employeeRepository.Query().FirstOrDefaultAsync(c => c.UserId == userMapped.Id);
                userMapped.FullName = employee.Name;
            }
            
            string token = GenerateToken(userMapped, roles, permissionQuery);
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
            return null;
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
            var permissionQuery = await GetListPermissionGroupByArrRoleId(user.Id);
            var userMapped= _mapper.Map<UserDTO>(user);
            if (roles.Count > 0 && roles.Contains("Customer"))
            {
                var customer = await _customerRepository.Query().FirstOrDefaultAsync(c => c.UserId == userMapped.Id);
                userMapped.FullName = customer.Name;
            }
            else if  (roles.Count > 0 && roles.Contains("Customer") ||  roles.Contains("Admin") )
            {
                var employee = await _employeeRepository.Query().FirstOrDefaultAsync(c => c.UserId == userMapped.Id);
                userMapped.FullName = employee.Name;
            }
            // Generate new access token
            string accessToken = GenerateToken(userMapped, roles, permissionQuery);
            
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
    private static string RandomString(int length)
    {
        var random = new Random();

        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        return new string(Enumerable.Repeat(chars, length)
                        .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private string GenerateToken(UserDTO user, List<string> roles, IEnumerable<PermissionGroupDTO> permissionQuery)
    {
        List<string> listPermission = new List<string>();
        
        // Process permissions if any
        if (permissionQuery != null && permissionQuery.Any())
        {
            foreach (var x in permissionQuery)
            {
                foreach (var per in x.SubPermissionGroups)
                {
                    foreach (var subPer in per.Permissions.Where(c=>c.IsGranted==true))
                    {
                        var permission= FunctionHelper.PermissionHelper
                            .GetPermission(subPer.Function, subPer.Command);
                        listPermission.Add(permission);
                    }
                }
            }
        }

        // Always create basic claims
        var claims = new[]
        {
            new Claim(ClaimTypeConst.UserName, user.UserName ?? ""),
            new Claim(ClaimTypeConst.UserId, user.Id.ToString()),
            new Claim(ClaimTypeConst.AdminOrUser, roles.Contains("Admin") ? "Admin" : "User"),
            new Claim(ClaimTypeConst.Roles, string.Join(";", roles ?? new List<string>())),
            new Claim(ClaimTypeConst.Email, user.Email ?? ""),
            new Claim(ClaimTypeConst.Avatar, user.Avatar ?? ""),
            new Claim(ClaimTypeConst.FullName, user.FullName ?? ""),
            new Claim(ClaimTypeConst.Permissions, string.Join(";", listPermission))
        };
        
        return GenerateTokenByClaim(claims);
    }

    private async Task<IEnumerable<PermissionGroupDTO>> GetListPermissionGroupByArrRoleId(Guid userId)
    {
        try
        {
            IEnumerable<RolePermissionDTO> userPermissionDtos = new List<RolePermissionDTO>();
            var permissionGroups = await _permissionGroupRepository.GetAllAsync();
            
            // Get permissions by user ID
            var rolePermissions = await _permissionRepository.GetPermissionByUserId(userId);
          
            userPermissionDtos = rolePermissions.Select(rp => new RolePermissionDTO
            {
                RoleId = rp.RoleId.GetHashCode(),
                PermissionId = rp.PermissionId.GetHashCode()
            });
            var response = permissionGroups.Select(pg => new PermissionGroupDTO
            {
                Id = pg.Id.GetHashCode(),
                Name = pg.Name,
                Description = pg.Description,
                ParentId = pg.ParentId.HasValue ? pg.ParentId.Value.GetHashCode() : null,
                Order = pg.Order,
                SubPermissionGroups = new List<SubPermissionGroupDTO>()
            }).ToList();

            foreach (var x in response.Where(c => c.ParentId == null))
            {
                var lstSubPermissionDtos = response.Where(c => c.ParentId == x.Id).ToList();
                
                x.SubPermissionGroups = new List<SubPermissionGroupDTO>();
                if (lstSubPermissionDtos.Count() == 0)
                {
                    var addSubItem = new SubPermissionGroupDTO();
                    addSubItem.Name = x.Name;
                    addSubItem.Id = x.Id;
                    addSubItem.Description = x.Description;
                    var permissions = new List<PermissionDTO>();
                    
                    addSubItem.Permissions = permissions;
                    
                    if (userPermissionDtos.Count() > 0)
                    {
                        int index = 0;
                        foreach (var per in addSubItem.Permissions)
                        {
                            if (userPermissionDtos.Any(c => c.PermissionId == per.Id.GetHashCode()))
                            {
                                per.IsGranted = true;
                            }
                            else
                            {
                                per.IsGranted = false;
                            }

                            ++index;
                            if (index == addSubItem.Permissions.Count())
                            {
                                if (addSubItem.Permissions.Count(c => c.IsGranted == true) == addSubItem.Permissions.Count())
                                {
                                    addSubItem.IsGranted = true;
                                }
                                else
                                {
                                    addSubItem.IsGranted = false;
                                }
                            }
                        }
                    }

                    x.SubPermissionGroups.Add(addSubItem);
                    x.IsGranted = x.SubPermissionGroups.Any(c => c.IsGranted == false) ? false : true;
                }
                else
                {
                    x.SubPermissionGroups = new List<SubPermissionGroupDTO>();
                    int indexSub = 0;
                    foreach (var subPermission in lstSubPermissionDtos)
                    {
                        ++indexSub;
                        var addSubItem = new SubPermissionGroupDTO();
                        addSubItem.Name = subPermission.Name;
                        addSubItem.Description = subPermission.Description;
                        addSubItem.Id = subPermission.Id;
                        var permissions = new List<PermissionDTO>();
                        
                        addSubItem.Permissions = permissions;
                        
                        if (userPermissionDtos.Count() > 0)
                        {
                            int index = 0;
                            foreach (var per in addSubItem.Permissions)
                            {
                                if (userPermissionDtos.Any(c => c.PermissionId == per.Id.GetHashCode()))
                                {
                                    per.IsGranted = true;
                                }
                                else
                                {
                                    per.IsGranted = false;
                                }

                                ++index;
                                if (index == addSubItem.Permissions.Count())
                                {
                                    if (addSubItem.Permissions.Count(c => c.IsGranted == true) == addSubItem.Permissions.Count())
                                    {
                                        addSubItem.IsGranted = true;
                                    }
                                    else
                                    {
                                        addSubItem.IsGranted = false;
                                    }
                                }
                            }
                        }
                        
                        x.SubPermissionGroups.Add(addSubItem);
                        if (indexSub == lstSubPermissionDtos.Count())
                        {
                            x.IsGranted = x.SubPermissionGroups.Any(c => c.IsGranted == false) ? false : true;
                        }
                    }
                }
            }

            return response.Where(c => c.ParentId == null).ToList();
        }
        catch (Exception ex)
        { return new List<PermissionGroupDTO>();
        }
    }

    private async Task<IEnumerable<PermissionGroupDTO>> GetListPermissionGroupByArrRoleIdV2(List<int> RoleIds)
    {
        try
        {
            IEnumerable<RolePermissionDTO> userPermissionDtos = new List<RolePermissionDTO>();
            var permissionGroups = await _permissionGroupRepository.GetAllAsync();
            
            if (RoleIds.Count > 0)
            {
                // Get role permissions for the given role IDs
                var rolePermissions = await _permissionRepository.GetPermissionByUserId(Guid.Empty);
                // Note: This is a workaround since the method expects a UserId, not RoleIds
                // In a real implementation, you would need to modify the repository or add a new method
                userPermissionDtos = rolePermissions.Select(rp => new RolePermissionDTO
                {
                    RoleId = rp.RoleId.GetHashCode(),
                    PermissionId = rp.PermissionId.GetHashCode()
                });
            }

            var response = permissionGroups.Select(pg => new PermissionGroupDTO
            {
                Id = pg.Id.GetHashCode(),
                Name = pg.Name,
                Description = pg.Description,
                ParentId = pg.ParentId.HasValue ? pg.ParentId.Value.GetHashCode() : null,
                Order = pg.Order,
                SubPermissionGroups = new List<SubPermissionGroupDTO>()
            }).ToList();

            foreach (var x in response.Where(c => c.ParentId == null))
            {
                var lstSubPermissionDtos = response.Where(c => c.ParentId == x.Id).ToList();
                
                x.SubPermissionGroups = new List<SubPermissionGroupDTO>();
                if (lstSubPermissionDtos.Count() == 0)
                {
                    var addSubItem = new SubPermissionGroupDTO();
                    addSubItem.Name = x.Name;
                    addSubItem.Id = x.Id;
                    addSubItem.Description = x.Description;
                    var permissions = new List<PermissionDTO>();
                    
                    addSubItem.Permissions = permissions;
                    
                    if (userPermissionDtos.Count() > 0)
                    {
                        int index = 0;
                        foreach (var per in addSubItem.Permissions)
                        {
                            if (userPermissionDtos.Any(c => c.PermissionId == per.Id.GetHashCode()))
                            {
                                per.IsGranted = true;
                            }
                            else
                            {
                                per.IsGranted = false;
                            }

                            ++index;
                            if (index == addSubItem.Permissions.Count())
                            {
                                if (addSubItem.Permissions.Count(c => c.IsGranted == true) == addSubItem.Permissions.Count())
                                {
                                    addSubItem.IsGranted = true;
                                }
                                else
                                {
                                    addSubItem.IsGranted = false;
                                }
                            }
                        }
                    }

                    x.SubPermissionGroups.Add(addSubItem);
                    x.IsGranted = x.SubPermissionGroups.Any(c => c.IsGranted == false) ? false : true;
                }
                else
                {
                    x.SubPermissionGroups = new List<SubPermissionGroupDTO>();
                    int indexSub = 0;
                    foreach (var subPermission in lstSubPermissionDtos)
                    {
                        ++indexSub;
                        var addSubItem = new SubPermissionGroupDTO();
                        addSubItem.Name = subPermission.Name;
                        addSubItem.Description = subPermission.Description;
                        addSubItem.Id = subPermission.Id;
                        var permissions = new List<PermissionDTO>();
                        
                        addSubItem.Permissions = permissions;
                        
                        if (userPermissionDtos.Count() > 0)
                        {
                            int index = 0;
                            foreach (var per in addSubItem.Permissions)
                            {
                                if (userPermissionDtos.Any(c => c.PermissionId == per.Id.GetHashCode()))
                                {
                                    per.IsGranted = true;
                                }
                                else
                                {
                                    per.IsGranted = false;
                                }

                                ++index;
                                if (index == addSubItem.Permissions.Count())
                                {
                                    if (addSubItem.Permissions.Count(c => c.IsGranted == true) == addSubItem.Permissions.Count())
                                    {
                                        addSubItem.IsGranted = true;
                                    }
                                    else
                                    {
                                        addSubItem.IsGranted = false;
                                    }
                                }
                            }
                        }

                        x.SubPermissionGroups.Add(addSubItem);
                        if (indexSub == lstSubPermissionDtos.Count())
                        {
                            x.IsGranted = x.SubPermissionGroups.Any(c => c.IsGranted == false) ? false : true;
                        }
                    }
                }
            }

            return response;
        }
        catch (Exception ex)
        {
            return new List<PermissionGroupDTO>();
        }
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