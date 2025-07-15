using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.InventoryBatch;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MeoMeo.Domain.Commons.PagingExtensions;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryBatchesController : ControllerBase
    {
        private readonly IInventoryBatchServices _inventoryBatchServices;

        public InventoryBatchesController(IInventoryBatchServices context)
        {
            _inventoryBatchServices = context;
        }

        // GET: api/InventoryBatches
        [HttpGet("get-all-inventoryBatch-async")]
        public async Task<PagingExtensions.PagedResult<InventoryBatchDTO>> GetAllInventoryBatch([FromQuery] GetListInventoryBatchRequestDTO request)
        {
            var result =  await _inventoryBatchServices.GetAllAsync(request);
            return result;
        }

        // GET: api/InventoryBatches/5
        [HttpGet("find-inventoryBatch-by-id-async/{id}")]
        public async Task<ActionResult<InventoryBatchDTO>> GetByIdInventoryBatch(Guid id)
        {
            var result = await _inventoryBatchServices.GetByIdAsync(id);
            if(result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // PUT: api/InventoryBatches/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update-inventoryBatch-async/{id}")]
        public async Task<IActionResult> UpdateInventoryBatch(Guid id, [FromBody] InventoryBatchDTO dto)
        {
            var updateBatch = await _inventoryBatchServices.UpdateAsync(id, dto);
            return Ok(updateBatch);
        }

        // POST: api/InventoryBatches
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("create-inventoryBatch-async")]
        public async Task<InventoryBatchResponseDTO> CreateInventoryBatch([FromBody] List<InventoryBatchDTO> dto )
        {
            var result = await _inventoryBatchServices.CreateAsync(dto);
            return result;
        }

        // DELETE: api/InventoryBatches/5
        [HttpDelete("delete-inventoryBatch-async/{id}")]
        public async Task<bool> DeleteInventoryBatch(Guid id)
        {
             var result =  await _inventoryBatchServices.DeleteAsync(id);
            return result;       
        }
    }
}
