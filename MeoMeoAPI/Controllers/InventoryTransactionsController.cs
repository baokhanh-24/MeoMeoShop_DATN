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
    public class InventoryTransactionsController : ControllerBase
    {
        private readonly IIventoryTranSactionServices _inventoryTranSactionServices;

        public InventoryTransactionsController(IIventoryTranSactionServices context)
        {
            _inventoryTranSactionServices = context;
        }

        // GET: api/InventoryTransactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryTransaction>>> GetinventoryTransactions()
        {
            var result = await _inventoryTranSactionServices.GetAllAsync();
            return Ok(result);
        }

        // GET: api/InventoryTransactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryTransaction>> GetInventoryTransaction(Guid id)
        {
            var transaction = await _inventoryTranSactionServices.GetByIdAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            return Ok(transaction);
        }

        // PUT: api/InventoryTransactions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInventoryTransaction(Guid id, [FromBody] InventoryTranSactionDTO inventoryTransaction)
        {
            await _inventoryTranSactionServices.UpdateAsync(id, inventoryTransaction);
            return Ok();
        }

        // POST: api/InventoryTransactions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<InventoryTransaction>> PostInventoryTransaction([FromBody] InventoryTranSactionDTO inventoryTransaction)
        {
            var id = await _inventoryTranSactionServices.CreateAsync(inventoryTransaction);
            return CreatedAtAction("GetInventoryTransaction", new { id = id }, inventoryTransaction);
        }

        // DELETE: api/InventoryTransactions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventoryTransaction(Guid id)
        {
            await _inventoryTranSactionServices.DeleteAsync(id);
            return Ok();
        }
    }
}
