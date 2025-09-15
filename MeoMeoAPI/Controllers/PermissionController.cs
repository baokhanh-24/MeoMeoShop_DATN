using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Permission;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.CMS.Controllers
{
    [ApiController]
    [Route("api/cms/[controller]")]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PermissionDTO>>> GetAllPermissions()
        {
            var permissions = await _permissionService.GetAllPermissionsAsync();
            return Ok(permissions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PermissionDTO>> GetPermissionById(Guid id)
        {
            var permission = await _permissionService.GetPermissionByIdAsync(id);
            return Ok(permission);
        }

        // CRUD operations removed - Permissions are fixed in database
        // Only GET operations are allowed

        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<List<PermissionDTO>>> GetPermissionsByGroup(Guid groupId)
        {
            var permissions = await _permissionService.GetPermissionsByGroupIdAsync(groupId);
            return Ok(permissions);
        }

        [HttpGet("role/{roleId}")]
        public async Task<ActionResult<List<PermissionDTO>>> GetPermissionsByRole(Guid roleId)
        {
            var permissions = await _permissionService.GetPermissionsByRoleIdAsync(roleId);
            return Ok(permissions);
        }
    }
}
