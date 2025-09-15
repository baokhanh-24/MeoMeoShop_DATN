using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Permission;
using Microsoft.AspNetCore.Mvc;
using MeoMeo.API.Extensions;

namespace MeoMeoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionGroupController : ControllerBase
    {
        private readonly IPermissionGroupService _permissionGroupService;

        public PermissionGroupController(IPermissionGroupService permissionGroupService)
        {
            _permissionGroupService = permissionGroupService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PermissionGroupDTO>>> GetAllPermissionGroups()
        {
            var groups = await _permissionGroupService.GetAllPermissionGroupsAsync();
            return Ok(groups);
        }

        [HttpGet("tree")]
        public async Task<ActionResult<List<PermissionGroupDTO>>> GetPermissionGroupsTree()
        {
            var groups = await _permissionGroupService.GetPermissionGroupsTreeAsync();
            return Ok(groups);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PermissionGroupDTO>> GetPermissionGroupById(Guid id)
        {
            var group = await _permissionGroupService.GetPermissionGroupByIdAsync(id);
            return Ok(group);
        }

        // CRUD operations removed - PermissionGroups are fixed in database
        // Only GET operations are allowed
    }
}