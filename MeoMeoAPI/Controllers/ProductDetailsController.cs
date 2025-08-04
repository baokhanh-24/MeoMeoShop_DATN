    using MeoMeo.API.Extensions;
    using MeoMeo.Application.IServices;
    using MeoMeo.Contract.Commons;
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
            private readonly IWebHostEnvironment _env;
            public ProductDetailsController(IProductDetailServices productdetailservices, IWebHostEnvironment env)
            {
                _productdetailservices = productdetailservices;
                _env = env;
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

            [HttpGet("get-by-id-detail/{id}")]
            public async Task<CreateOrUpdateProductDetailDTO> GetByIdDetail(Guid id)
            {
                var result = await _productdetailservices.GetProductDetailByIdAsync(id);
                return result;
            }

            [HttpPost("create-product-detail-async")]
            [RequestSizeLimit(100 * 1024 * 1024)]
            public async Task<IActionResult> CreateProductDetailAsync([FromForm] CreateOrUpdateProductDetailDTO productDetail)
            {   
                var productId= Guid.NewGuid();
                var lstMediaToUpload = productDetail.MediaUploads?.Select(c => c.UploadFile).ToList() ?? new List<IFormFile>();
                var listFileUploaded=  await FileUploadHelper.UploadFilesAsync(_env,lstMediaToUpload,"Products",productId);
                productDetail.ProductId = productId;
                var result = await _productdetailservices.CreateProductDetailAsync(productDetail, listFileUploaded);
                if (result.ResponseStatus == BaseStatus.Error)
                { 
                    FileUploadHelper.DeleteUploadedFiles(_env,listFileUploaded);
                }
                return Ok(result);
            }


            [HttpPut("update-product-detail-async/{id}")]
            [RequestSizeLimit(100 * 1024 * 1024)]
            public async Task<IActionResult> Update(Guid id, [FromForm] CreateOrUpdateProductDetailDTO productDetail)
            {
                // Lọc ra các ảnh mới (Id == null)
                var newImages = productDetail.MediaUploads?.Where(img => img.Id == null).Select(img => img.UploadFile).ToList();
                List<FileUploadResult> listFileUploaded = new List<FileUploadResult>();
                if (newImages != null && newImages.Count > 0)
                {
                    listFileUploaded = await FileUploadHelper.UploadFilesAsync(_env, newImages, "Products", id);
                }
                // Lấy danh sách ảnh cũ
                var oldImages = await _productdetailservices.GetOldImagesAsync(id);
                var newImageIds = productDetail.MediaUploads?.Where(i => i.Id != null).Select(i => i.Id.Value).ToList() ?? new List<Guid>();
                var imagesToDelete = oldImages.Where(img => !newImageIds.Contains(img.Id)).ToList();
                // Gọi service cập nhật, truyền cả danh sách ảnh mới đã upload
                var result = await _productdetailservices.UpdateProductDetailAsync(productDetail, listFileUploaded);
                if (result.ResponseStatus == BaseStatus.Success)
                {
                    // Xóa file vật lý các ảnh không còn
                    foreach (var img in imagesToDelete)
                    {
                        FileUploadHelper.DeleteUploadedFiles(_env,new List<FileUploadResult> { new FileUploadResult { RelativePath = img.URL } });
                    }
                }
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



