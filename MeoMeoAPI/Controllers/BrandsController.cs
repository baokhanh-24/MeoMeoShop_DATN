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
    public class BrandsController : ControllerBase
    {
        private readonly IBrandServices _brandServices;

        public BrandsController(IBrandServices context)
        {
            _brandServices = context;
        }

        // GET: api/Brands
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Brand>>> Getbrands()
        {
            var result = await _brandServices.GetAllBrandsAsync();
            return Ok(result);
        }

        // GET: api/Brands/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Brand>> GetBrand(Guid id)
        {
            var getId = await _brandServices.GetBrandByIdAsync(id);
            if (getId == null)
            {
                return NotFound();
            }
            return Ok(getId);
        }

        // PUT: api/Brands/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBrand(Guid id,[FromBody] BrandDTO brand)
        {
            var getBrand = await _brandServices.UpdateBrandAsync(id, brand);
            return Ok(getBrand);
        }

        // POST: api/Brands
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Brand>> PostBrand([FromBody] BrandDTO brand)
        {
            var createbrand = await _brandServices.CreateBrandAsync(brand);
            return CreatedAtAction("GetBrand", new { id = createbrand.Id }, createbrand);
        }

        // DELETE: api/Brands/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(Guid id)
        {
            var deleteBrand = await _brandServices.DeleteBrandAsync(id);
            return Ok(deleteBrand);
        }
    }
}
