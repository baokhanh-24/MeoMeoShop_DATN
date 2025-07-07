using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs.ProductDetail;
using MeoMeo.Domain.Commons;
using Microsoft.AspNetCore.Mvc;
namespace MeoMeo.API.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ProductDetailsController : ControllerBase

    {
        private readonly IProductDetailServices _productdetailservices;

        public ProductDetailsController(IProductDetailServices productdetailservices)
        {
            _productdetailservices = productdetailservices;
        }
        [HttpGet("get-all-product-detail-async")]
        public async Task<PagingExtensions.PagedResult<ProductDetailDTO>> GetAllProductDetailAsync([FromQuery] GetListProductDetailRequestDTO request)
        {
            var result = await _productdetailservices.GetAllProductDetailAsync(request);
            return result;
        }

        [HttpGet("find-product-detail-by-id-async/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _productdetailservices.GetProductDetailByIdAsync(id);

            return Ok(result);
        }

        [HttpPost("create-product-detail-async")]
        public async Task<IActionResult> CreateProductDetailAsync([FromBody] CreateOrUpdateProductDetailDTO productDetail)
        {
            var result = await _productdetailservices.CreateProductDetailAsync(productDetail);
            return Ok(result);
        }


        [HttpPut("update-product-detail-async/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateOrUpdateProductDetailDTO productDetail)
        {


            var result = await _productdetailservices.UpdateProductDetailAsync(productDetail);

            return Ok(result);
        }

        [HttpDelete("delete-product-detail-async/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _productdetailservices.DeleteProductDetailAsync(id);

            return Ok(result);
        }
    }
}



