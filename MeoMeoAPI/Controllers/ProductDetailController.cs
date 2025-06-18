using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace MeoMeo.API.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ProductDetailController : ControllerBase
        
    {
        private readonly IProductDetailServices _productdetailservices;

        public ProductDetailController(IProductDetailServices productdetailservices)
        {
            _productdetailservices = productdetailservices;
        }
        [HttpGet()]
        public async Task<IActionResult> GetAll()
        {
            var productDetails = await _productdetailservices.GetProductDetailAllAsync();
            return Ok(productDetails);
        }

        [HttpGet("find-productdetail-by-id/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var productDetail = await _productdetailservices.GetProductDetailByIdAsync(id);

            return Ok(productDetail);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrUpdateProductDetailDTO productDetail)
        {
            var result = await _productdetailservices.AddProductDetailAsync(productDetail);
            return Ok(result);
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateOrUpdateProductDetailDTO productDetail)
        {
           

            var existing = await _productdetailservices.GetProductDetailByIdAsync(id);
            if (existing == null)
                return NotFound("ProductDetail not found.");

            await _productdetailservices.UpdateProductDetailAsync(productDetail);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _productdetailservices.GetProductDetailByIdAsync(id);
            if (existing == null)
                return NotFound("ProductDetail not found.");

            await _productdetailservices.DeleteProductDetailAsync(id);
            return NoContent();
        }
    }
}
   


