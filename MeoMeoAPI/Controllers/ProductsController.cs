using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductServices _productServices;

        public ProductsController(IProductServices productServices)
        {
            _productServices = productServices;
        }

        [HttpGet("find-product-by-id/{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var result = await _productServices.GetProductAsync(id);
            return Ok(result);
        }

        [HttpPost("create-product")]
        public async Task<IActionResult> CreateProduct([FromBody]CreateOrUpdateProductDTO product)
        {
            var result = await _productServices.CreateProductAsync(product);
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Product model)
        {
            if (id != model.Id) return BadRequest();
            await _productServices.UpdateAsync(model);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _productServices.DeleteAsync(id);
            return Ok();
        }
    }
}
