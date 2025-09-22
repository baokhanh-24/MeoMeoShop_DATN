using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Permission;
using Microsoft.AspNetCore.Mvc;
using MeoMeo.API.Extensions;

namespace MeoMeoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<ActionResult<List<RoleDTO>>> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDTO>> GetRoleById(Guid id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            return Ok(role);
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse>> CreateRole([FromBody] CreateOrUpdateRoleDTO dto)
        {
            var result = await _roleService.CreateRoleAsync(dto);
            return Ok(result);
        }

        [HttpPut]
        public async Task<ActionResult<BaseResponse>> UpdateRole([FromBody] CreateOrUpdateRoleDTO dto)
        {
            var result = await _roleService.UpdateRoleAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteRole(Guid id)
        {
            var result = await _roleService.DeleteRoleAsync(id);
            return Ok(result.ResponseStatus == BaseStatus.Success);
        }

        [HttpPost("assign-user")]
        public async Task<ActionResult<BaseResponse>> AssignUserToRole([FromBody] AssignUserToRoleDTO dto)
        {
            var result = await _roleService.AssignUserToRoleAsync(dto.UserId, dto.RoleId);
            return Ok(result);
        }

        [HttpDelete("remove-user")]
        public async Task<ActionResult<BaseResponse>> RemoveUserFromRole([FromQuery] Guid userId, [FromQuery] Guid roleId)
        {
            var result = await _roleService.RemoveUserFromRoleAsync(userId, roleId);
            return Ok(result);
        }
    }
}