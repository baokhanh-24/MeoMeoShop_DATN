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

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductReviewsController : ControllerBase
    {
        private readonly IProductReviewService _service;
        private readonly IWebHostEnvironment _env;
        public ProductReviewsController(IProductReviewService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm]  ProductReviewCreateOrUpdateDTO request)
        {
            var fileDtos = new List<ProductReviewFileDTO>();
            var reviewId= Guid.NewGuid();
            var lstMediaToUpload = request.Files.Select(c => c.UploadFile).ToList();
            var listFileUploaded=  await FileUploadHelper.UploadFilesAsync(_env,lstMediaToUpload,"Reviews",reviewId);
            var result = await _service.CreateProductReviewAsync(request,listFileUploaded );
            if (result.ResponseStatus == BaseStatus.Error)
            {
                FileUploadHelper.DeleteUploadedFiles(listFileUploaded);
            }
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromForm] ProductReviewCreateOrUpdateDTO request)
        {
            var newFiles = request.Files?.Where(f => f.Id == null).Select(f => f.UploadFile).ToList();
            List<FileUploadResult> uploadedFiles = new List<FileUploadResult>();
            if (newFiles != null && newFiles.Count > 0)
            {
                uploadedFiles = await FileUploadHelper.UploadFilesAsync(_env, newFiles, "ReviewFiles", request.Id.Value);
            }
            var oldFiles = await _service.GetOldFilesAsync(request.Id.Value);
            var keepFileIds = request.Files?.Where(f => f.Id != null).Select(f => f.Id.Value).ToList() ?? new List<Guid>();
            var filesToDelete = oldFiles.Where(f => !keepFileIds.Contains(f.Id)).ToList();
            foreach (var img in filesToDelete)
            {
                FileUploadHelper.DeleteUploadedFiles(new List<FileUploadResult> { new FileUploadResult { FullPath = img.FileUrl } });
            }
            var result = await _service.UpdateProductReviewAsync(request, uploadedFiles);
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
    }
} 