using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeoMeo.Domain.Entities;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using MeoMeo.Application.IServices;
using static MeoMeo.Domain.Commons.PagingExtensions;
using MeoMeo.Contract.DTOs.SystemConfig;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemConfigsController : ControllerBase
    {
        private readonly ISystemConfigService _systemConfigService;

        public SystemConfigsController(ISystemConfigService context)
        {
            _systemConfigService = context;
        }

        // GET: api/SystemConfigs
        [HttpGet]
        public async Task<ActionResult<PagedResult<CreateOrUpdateSystemConfigDTO>>> GetsystemConfigs([FromQuery] GetListSystemConfigRequestDTO requestDTO)
        {
            var result = await _systemConfigService.GetAllSystemConfigAsync(requestDTO);
            return Ok(result);
        }

        // GET: api/SystemConfigs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SystemConfig>> GetSystemConfig(Guid id)
        {
            var result = await _systemConfigService.GetSystemConfigByIdAsync(id);
            return Ok(result);
        }

        // PUT: api/SystemConfigs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSystemConfig(Guid id, [FromBody] CreateOrUpdateSystemConfigDTO systemConfig)
        {
            systemConfig.Id = id;
            var result = await _systemConfigService.UpdateSystemConfigAsync(systemConfig);
            return Ok(result);  
        }

        // POST: api/SystemConfigs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SystemConfig>> PostSystemConfig([FromBody] CreateOrUpdateSystemConfigDTO systemConfig)
        {
            var result = await _systemConfigService.CreateSystemConfigAsync(systemConfig);
            return Ok(result);
        }

        // DELETE: api/SystemConfigs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSystemConfig(Guid id)
        {
            var result = await _systemConfigService.DeleteSystemConfigAsync(id);
            return Ok(result);
        }
    }
}
