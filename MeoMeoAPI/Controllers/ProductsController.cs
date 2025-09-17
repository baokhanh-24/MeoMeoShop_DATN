using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs.Product;
using MeoMeo.Contract.Commons;
using MeoMeo.Shared.Utilities;
using MeoMeo.Domain.Commons;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MeoMeo.API.Extensions;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductServices _productServices;
        private readonly IWebHostEnvironment _environment;

        public ProductsController(IProductServices productServices, IWebHostEnvironment environment)
        {
            _productServices = productServices;
            _environment = environment;
        }

        [HttpGet("get-all-product-async")]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _productServices.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("get-paged-products-async")]
        public async Task<IActionResult> GetPagedProducts([FromQuery] GetListProductRequestDTO request)
        {
            var result = await _productServices.GetPagedProductsAsync(request);
            return Ok(result);
        }

        [HttpGet("get-paged-products-for-portal-async")]
        public async Task<IActionResult> GetPagedProductsForPortal([FromQuery] GetListProductRequestDTO request)
        {
            var result = await _productServices.GetPagedProductsForPortalAsync(request);
            return Ok(result);
        }

        [HttpGet("get-header-products")]
        public async Task<IActionResult> GetHeaderProducts()
        {
            var result = await _productServices.GetHeaderProductsAsync();
            return Ok(result);
        }

        [HttpGet("get-product-for-create-update/{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var result = await _productServices.GetProductByIdAsync(id);
            if (result == null)
            {
                return NotFound(new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không tìm thấy sản phẩm"
                });
            }
            return Ok(result);
        }

        [HttpGet("get-product-by-id-async/{id}")]
        public async Task<IActionResult> GetProductByIdForEdit(Guid id)
        {
            var result = await _productServices.GetProductByIdAsync(id);
            if (result == null)
            {
                return NotFound(new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không tìm thấy sản phẩm"
                });
            }
            return Ok(result);
        }

        [HttpGet("get-product-with-details/{id}")]
        public async Task<IActionResult> GetProductWithDetails(Guid id)
        {
            var result = await _productServices.GetProductWithDetailsAsync(id);
            if (result.ResponseStatus == BaseStatus.Error)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost("create-product-async")]
        [RequestSizeLimit(200 * 1024 * 1024)] // 200MB limit
        public async Task<BaseResponse> CreateProduct([FromForm] CreateOrUpdateProductDTO productDto)
        {
            try
            {
                var filesToUpload = productDto.MediaUploads?.Where(m => m.UploadFile != null).Select(m => m.UploadFile).ToList() ?? new List<IFormFile>();
                List<FileUploadResult> uploadedFiles = new List<FileUploadResult>();

                if (filesToUpload.Any())
                {
                    var productId = Guid.NewGuid();
                    uploadedFiles = await FileUploadHelper.UploadFilesAsync(_environment, filesToUpload, "Products", productId);
                    productDto.Id = productId;
                }

                // Call service with uploaded files
                var result = await _productServices.CreateProductAsync(productDto, uploadedFiles);

                if (result.ResponseStatus == BaseStatus.Error)
                {
                    // Rollback uploaded files if service failed
                    FileUploadHelper.DeleteUploadedFiles(_environment, uploadedFiles);
                }

                return result;
            }
            catch (Exception ex)
            {
                return new BaseResponse()
                {
                    Message = $"Lỗi khi tạo sản phẩm: {ex.Message}",
                    ResponseStatus = BaseStatus.Error
                };
            }
        }
        [HttpPut("update-product-async")]
        [RequestSizeLimit(200 * 1024 * 1024)] // 200MB limi
        public async Task<BaseResponse> UpdateProduct([FromForm] CreateOrUpdateProductDTO productDto)
        {
            try
            {
                // Upload new files
                var newFiles = productDto.MediaUploads?.Where(m => m.UploadFile != null).Select(m => m.UploadFile).ToList() ?? new List<IFormFile>();
                List<FileUploadResult> uploadedFiles = new List<FileUploadResult>();

                if (newFiles.Any())
                {
                    uploadedFiles = await FileUploadHelper.UploadFilesAsync(_environment, newFiles, "Products", productDto.Id.Value);
                }
                // Get old images for deletion
                var oldImages = await _productServices.GetOldImagesAsync(productDto.Id.Value);
                var keepImageIds = productDto.MediaUploads?.Where(m => m.Id.HasValue).Select(m => m.Id.Value).ToList() ?? new List<Guid>();
                var imagesToDelete = oldImages.Where(img => !keepImageIds.Contains(img.Id)).ToList();
                // Call service update
                var result = await _productServices.UpdateAsync(productDto, uploadedFiles);

                // Delete old files if update successful
                if (result.ResponseStatus == BaseStatus.Success)
                {
                    foreach (var img in imagesToDelete)
                    {
                        FileUploadHelper.DeleteUploadedFiles(_environment, new List<FileUploadResult> { new FileUploadResult { RelativePath = img.URL } });
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return new BaseResponse()
                {
                    Message = $"Lỗi khi cập nhật sản phẩm: {ex.Message}",
                    ResponseStatus = BaseStatus.Error
                };
            }
        }


        [HttpDelete("delete-product-async/{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            // Get old images for file deletion
            var oldImages = await _productServices.GetOldImagesAsync(id);

            var result = await _productServices.DeleteAsync(id);

            if (result.ResponseStatus == BaseStatus.Error)
            {
                return BadRequest(result);
            }

            // Delete physical files after successful database deletion
            if (result.ResponseStatus == BaseStatus.Success)
            {
                foreach (var img in oldImages)
                {
                    if (img.URL.StartsWith("/Products/"))
                    {
                        FileUploadHelper.DeleteUploadedFiles(_environment, new List<FileUploadResult> { new FileUploadResult { RelativePath = img.URL.TrimStart('/') } });
                    }
                }
            }

            return Ok(result);
        }

        [HttpGet("best-sellers-week")]
        public async Task<IActionResult> GetWeeklyBestSellers([FromQuery] int take = 10)
        {
            var result = await _productServices.GetWeeklyBestSellersAsync(take);
            return Ok(result);
        }

        [HttpGet("top-rated")]
        public async Task<IActionResult> GetTopRated([FromQuery] int take = 12)
        {
            var result = await _productServices.GetTopRatedProductsAsync(take);
            return Ok(result);
        }

        [HttpPost("by-ids")]
        public async Task<IActionResult> GetByIds([FromBody] List<Guid> ids)
        {
            if (ids == null || ids.Count == 0) return Ok(new List<ProductResponseDTO>());
            var result = await _productServices.GetProductsByIdsAsync(ids);
            return Ok(result);
        }
        [HttpPut("update-variant-status-async")]
        public async Task<IActionResult> UpdateVariantStatus([FromBody] UpdateProductStatusDTO input)
        {
            var result = await _productServices.UpdateVariantStatusAsync(input);

            if (result.ResponseStatus == BaseStatus.Error)
            {
                return BadRequest(new { Message = "Không thể cập nhật trạng thái biến thể sản phẩm" });
            }

            return Ok(new { Message = "Cập nhật trạng thái biến thể sản phẩm thành công" });
        }

        // Legacy endpoints for backward compatibility
        [HttpGet("get-all-product-async-legacy")]
        public async Task<IActionResult> GetProduct()
        {
            var result = await _productServices.GetAllAsync();
            return Ok(result);
        }

        [HttpPost("create-product-legacy")]
        public async Task<IActionResult> CreateProductLegacy([FromBody] CreateOrUpdateProductDTO productDto)
        {
            var result = await _productServices.CreateProductAsync(productDto, new List<FileUploadResult>());
            return Ok(result);
        }

        [HttpPut("update-product-legacy/{id}")]
        public async Task<IActionResult> UpdateProductLegacy(Guid id, [FromBody] CreateOrUpdateProductDTO productDto)
        {
            productDto.Id = id;
            var result = await _productServices.UpdateAsync(productDto, new List<FileUploadResult>());
            return Ok(result);
        }

        [HttpDelete("delete-product-legacy/{id}")]
        public async Task<IActionResult> DeleteProductLegacy(Guid id)
        {
            var result = await _productServices.DeleteAsync(id);
            return Ok(result);
        }

        // Search endpoints for POS system
        [HttpGet("search-products-async")]
        public async Task<IActionResult> SearchProducts([FromQuery] ProductSearchRequestDTO request)
        {
            try
            {

                var result = await _productServices.SearchProductsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Lỗi khi tìm kiếm sản phẩm: {ex.Message}"
                });
            }
        }

        [HttpGet("get-product-by-sku-async/{sku}")]
        public async Task<IActionResult> GetProductBySku(string sku)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sku))
                {
                    return BadRequest(new BaseResponse
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "SKU không được để trống"
                    });
                }

                var result = await _productServices.GetProductBySkuAsync(sku.Trim());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Lỗi khi lấy sản phẩm theo SKU: {ex.Message}"
                });
            }
        }

        [HttpGet("get-related-products-async/{productId}")]
        public async Task<IActionResult> GetRelatedProducts(Guid productId, [FromQuery] int pageSize = 4)
        {
            try
            {
                var result = await _productServices.GetRelatedProductsAsync(productId, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Lỗi khi lấy sản phẩm liên quan: {ex.Message}"
                });
            }
        }

        [HttpGet("check-variant-dependencies-async/{variantId}")]
        public async Task<IActionResult> CheckVariantDependencies(Guid variantId)
        {
            try
            {
                var result = await _productServices.CheckVariantDependenciesAsync(variantId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Lỗi khi kiểm tra phụ thuộc biến thể: {ex.Message}"
                });
            }
        }
    }
}