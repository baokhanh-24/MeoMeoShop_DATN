using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Permission;
using Microsoft.AspNetCore.Mvc;
using MeoMeo.API.Extensions;

namespace MeoMeoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleService _userRoleService;

        public UserRoleController(IUserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }

        [HttpGet("users")]
        public async Task<ActionResult<List<UserWithRolesDTO>>> GetAllUsersWithRoles()
        {
            var users = await _userRoleService.GetAllUsersWithRolesAsync();
            return Ok(users);
        }

        [HttpGet("users/{userId}")]
        public async Task<ActionResult<UserWithRolesDTO>> GetUserWithRoles(Guid userId)
        {
            var user = await _userRoleService.GetUserWithRolesAsync(userId);
            return Ok(user);
        }

        [HttpPost("assign")]
        public async Task<ActionResult<BaseResponse>> AssignRoleToUser([FromBody] AssignRoleToUserDTO dto)
        {
            var result = await _userRoleService.AssignRoleToUserAsync(dto);
            return Ok(result);
        }

        [HttpDelete("users/{userId}/roles/{roleId}")]
        public async Task<ActionResult<bool>> RemoveRoleFromUser(Guid userId, Guid roleId)
        {
            var result = await _userRoleService.RemoveRoleFromUserAsync(userId, roleId);
            return Ok(result.ResponseStatus == BaseStatus.Success);
        }

        [HttpGet("users/{userId}/roles")]
        public async Task<ActionResult<List<UserRoleDTO>>> GetUserRoles(Guid userId)
        {
            var userRoles = await _userRoleService.GetUserRolesAsync(userId);
            return Ok(userRoles);
        }

        [HttpGet("roles/{roleId}/users")]
        public async Task<ActionResult<List<UserRoleDTO>>> GetRoleUsers(Guid roleId)
        {
            var roleUsers = await _userRoleService.GetRoleUsersAsync(roleId);
            return Ok(roleUsers);
        }
    }
}