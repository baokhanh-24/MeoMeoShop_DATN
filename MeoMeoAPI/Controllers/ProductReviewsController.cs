using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs.ProductReview;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Linq;
using MeoMeo.Domain.Entities;
using MeoMeo.API.Extensions;
using MeoMeo.Contract.Commons;
using Microsoft.AspNetCore.Hosting;
using AutoMapper;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductReviewsController : ControllerBase
    {
        private readonly IProductReviewService _service;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductReviewsController(IProductReviewService service, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpPost]
        [RequestSizeLimit(200 * 1024 * 1024)] // 200MB limit
        public async Task<IActionResult> Create([FromForm] ProductReviewCreateOrUpdateDTO request)
        {

            var customerId = _httpContextAccessor.HttpContext.GetCurrentCustomerId();
            if (customerId == Guid.Empty)
            {
                return BadRequest();
            }
            request.CustomerId = customerId;
            var reviewId = Guid.NewGuid();
            var lstMediaToUpload = request.MediaUploads.Where(c => c.UploadFile != null).Select(c => c.UploadFile!).ToList();
            var listFileUploaded = await FileUploadHelper.UploadFilesAsync(_env, lstMediaToUpload, "Reviews", reviewId);
            var result = await _service.CreateProductReviewAsync(request, listFileUploaded);
            if (result.ResponseStatus == BaseStatus.Error)
            {
                FileUploadHelper.DeleteUploadedFiles(_env, listFileUploaded);
            }
            return Ok(result);
        }
        [HttpPut("{id}")]
        [RequestSizeLimit(200 * 1024 * 1024)] // 200MB limit
        public async Task<IActionResult> Update([FromForm] ProductReviewCreateOrUpdateDTO request)
        {
            var customerId = _httpContextAccessor.HttpContext.GetCurrentCustomerId();
            if (customerId == Guid.Empty)
            {
                return BadRequest();
            }
            request.CustomerId = customerId;
            var newFiles = request.MediaUploads?.Where(f => f.Id == null && f.UploadFile != null).Select(f => f.UploadFile!).ToList();
            List<FileUploadResult> uploadedFiles = new List<FileUploadResult>();
            if (newFiles != null && newFiles.Count > 0)
            {
                uploadedFiles = await FileUploadHelper.UploadFilesAsync(_env, newFiles, "Reviews", request.Id!.Value);
            }
            var oldFiles = await _service.GetOldFilesAsync(request.Id!.Value);
            var keepFileIds = request.MediaUploads?.Where(f => f.Id.HasValue).Select(f => f.Id!.Value).ToList() ?? new List<Guid>();
            var filesToDelete = oldFiles.Where(f => !keepFileIds.Contains(f.Id)).ToList();
     
            var result = await _service.UpdateProductReviewAsync(request, uploadedFiles);
            if (result.ResponseStatus == BaseStatus.Error)
            {
                foreach (var img in uploadedFiles)
                {
                    FileUploadHelper.DeleteUploadedFiles(_env, new List<FileUploadResult> { new FileUploadResult { RelativePath = img.RelativePath } });
                }
            }
            foreach (var img in filesToDelete)
            {
                FileUploadHelper.DeleteUploadedFiles(_env, new List<FileUploadResult> { new FileUploadResult { RelativePath = img.FileUrl } });
            }
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteProductReviewAsync(id);
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllProductReviewsAsync();
            return Ok(result);
        }

        [HttpGet("product-detail")]
        public async Task<IActionResult> GetByProductDetailIds([FromQuery] GetListProductReviewDTO request)
        {
            var result = await _service.GetProductReviewsByProductDetailIdAsync(request);
            return Ok(result);
        }

        [HttpGet("unreviewed-items")]
        public async Task<IActionResult> GetUnreviewedOrderItems()
        {
            var customerId = _httpContextAccessor.HttpContext.GetCurrentCustomerId();
            if (customerId == Guid.Empty)
            {
                return BadRequest(new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Vui lòng đăng nhập để xem đánh giá"
                });
            }

            try
            {
                var result = await _service.GetUnreviewedOrderItemsAsync(customerId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Có lỗi xảy ra: {ex.Message}"
                });
            }
        }

        [HttpGet("my-reviews")]
        public async Task<IActionResult> GetMyReviews([FromQuery] GetListMyReviewedDTO request)
        {
            var customerId = _httpContextAccessor.HttpContext.GetCurrentCustomerId();
            if (customerId == Guid.Empty)
            {
                return BadRequest(new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Vui lòng đăng nhập để xem đánh giá"
                });
            }

            try
            {
                var result = await _service.GetCustomerReviewsAsync(customerId,request );
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Có lỗi xảy ra: {ex.Message}"
                });
            }
        }
    }
}