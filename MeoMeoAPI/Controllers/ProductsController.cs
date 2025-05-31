using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
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
    }
}
