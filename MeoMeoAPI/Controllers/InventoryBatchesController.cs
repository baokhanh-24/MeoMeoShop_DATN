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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryBatch>>> GetAllInventoryBatch()
        {
            var result = await _inventoryBatchServices.GetAllAsync();
            return Ok(result);
        }

        // GET: api/InventoryBatches/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryBatch>> GetByIdInventoryBatch(Guid id)
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
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInventoryBatch(Guid id, [FromBody] InventoryBatchDTO dto)
        {
            await _inventoryBatchServices.UpdateAsync(id, dto);
            return Ok();
        }

        // POST: api/InventoryBatches
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<InventoryBatch>> CreateInventoryBatch([FromBody] InventoryBatchDTO dto )
        {
            var id = await _inventoryBatchServices.CreateAsync(dto);
            return CreatedAtAction(nameof(GetByIdInventoryBatch), new { id }, null);
        }

        // DELETE: api/InventoryBatches/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventoryBatch(Guid id)
        {
            await _inventoryBatchServices.DeleteAsync(id);
            return Ok();
        }
    }
}
