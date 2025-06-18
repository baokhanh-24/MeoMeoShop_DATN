using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeoMeo.Domain.Entities;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using MeoMeo.Domain.IRepositories;
using MeoMeo.Contract.DTOs;
using ProductSeasonDTO = MeoMeo.Contract.DTOs.ProductSeasonDTO;
using MeoMeo.Application.IServices;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductSeasonsController : ControllerBase
    {
        private readonly IProductSeasonServices  _productSeasonRepository;

        public ProductSeasonsController(IProductSeasonServices context)
        {
            _productSeasonRepository = context;
        }

        // GET: api/ProductSeasons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductSeason>>> GetproductSeasons()
        {
            var phat = await _productSeasonRepository.GetAllProductSeasonAsync();
            return Ok(phat);
        }

        // GET: api/ProductSeasons/5
        [HttpGet("product/{ProductId}/season/{SeasonId}")]
        public async Task<ActionResult<ProductSeason>> GetProductSeason(Guid ProductId, Guid SeasonId)
        {
            var entity = await _productSeasonRepository.GetProductSeasonByIdAsync(ProductId, SeasonId);
            if (entity == null)
            {
                return NotFound();
            }
            return Ok(entity);
        }


        // POST: api/ProductSeasons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost()]
        public async Task<ActionResult<ProductSeason>> PostProductSeason( [FromBody] ProductSeasonDTO productSeason)
        {
            try
            {
                var phat = await _productSeasonRepository.CreateProductSeasonAsync(productSeason);
                return Ok(phat);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/ProductSeasons/5
        [HttpDelete("product/{ProductId}/season/{SeasonId}")]
        public async Task<IActionResult> DeleteProductSeason(Guid ProductId, Guid SeasonId)
        {
            var phat = await _productSeasonRepository.DeleteProductSeasonAsync(ProductId, SeasonId);
            if (!phat)
            {
                return NotFound();
            }
            return Ok(phat);
        }
    }
}
