using MeoMeo.Application.IServices;
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
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _productdetailservices.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _productdetailservices.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductDetail model)
        {
            await _productdetailservices.AddAsync(model);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ProductDetail model)
        {
            if (id != model.Id) return BadRequest();
            await _productdetailservices.UpdateAsync(model);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _productdetailservices.DeleteAsync(id);
            return Ok();
        }
    }
}



