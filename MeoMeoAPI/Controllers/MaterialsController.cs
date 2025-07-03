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
using MeoMeo.Contract.DTOs;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MeoMeo.Domain.Commons;
using static MeoMeo.Domain.Commons.PagingExtensions;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialsController : ControllerBase
    {
        private readonly IMaterialServices _materialServices;

        public MaterialsController(IMaterialServices  materialServices)
        {
            _materialServices = materialServices;
        }

        // GET: api/Materials
        [HttpGet]
        public async Task<ActionResult<PagedResult<CreateOrUpdateMaterialDTO>>> Getmaterials([FromQuery] GetListMaterialRequest request)
        {
            var result = await _materialServices.GetAllMaterialsAsync(request);
            return Ok(result);
        }

        // GET: api/Materials/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Material>> GetMaterialById(Guid id)
        {
            var result = await _materialServices.GetMaterialsByIdAsync(id);
            return Ok(result);
        }

        // PUT: api/Materials/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMaterial(Guid id, [FromBody] CreateOrUpdateMaterialDTO dTO)
        {
            dTO.Id = id;
            var result = await _materialServices.UpdateMaterialsAsync(dTO);
            return Ok(result);
        }

        // POST: api/Materials
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Material>> PostMaterial([FromBody] CreateOrUpdateMaterialDTO material)
        {
            var result = await _materialServices.CreateMaterialsAsync(material);
            return Ok(result);
        }

        // DELETE: api/Materials/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterial(Guid id)
        {
            var result = await _materialServices.DeleteMaterialsAsync(id);
            return Ok(result);
        }
    }
}
