using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
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
        [HttpGet()]
        public async Task<IActionResult> GetProduct()
        {
            var result = await _productServices.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("find-product-by-id/{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var result = await _productServices.GetProductByIdAsync(id);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateOrUpdateProductDTO productDto)
        {
            var result = await _productServices.CreateProductAsync(productDto);
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] CreateOrUpdateProductDTO productDto)
        {
            var result = await _productServices.UpdateAsync(productDto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var result = await _productServices.GetProductByIdAsync(id);
           
            return Ok(result);
        }
    }

}