using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs.ProductReview;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeoMeo.Contract.Commons;
using AutoMapper;

namespace MeoMeo.Application.Services
{
    public class ProductReviewService : IProductReviewService
    {
        private readonly IProductReviewRepository _reviewRepo;
        private readonly IBaseRepository<ProductReviewFile> _fileRepo;
        private readonly IMapper _mapper;
        public ProductReviewService(IProductReviewRepository reviewRepo, IBaseRepository<ProductReviewFile> fileRepo, IMapper mapper)
        {
            _reviewRepo = reviewRepo;
            _fileRepo = fileRepo;
            _mapper = mapper;
        }
        public async Task<BaseResponse> CreateProductReviewAsync(ProductReviewCreateOrUpdateDTO dto,List<FileUploadResult> filesUpload)
        {
            var review = new ProductReview
            {
                Id = Guid.NewGuid(),
                Content = dto.Content,
                Rating = dto.Rating,
                IsHidden = dto.IsHidden,
                CustomerId = dto.CustomerId,
                OrderId = dto.OrderId,
                ProductDetailId = dto.ProductDetailId,
            };
            await _reviewRepo.CreateProductReviewAsync(review);
            if (dto.Files != null && dto.Files.Count > 0)
            {
                foreach (var file in filesUpload)
                {
                    var reviewFile = new ProductReviewFile
                    {
                        Id = Guid.NewGuid(),
                        ProductReviewId = review.Id,
                        FileName = file.FileName,
                        FileUrl = file.RelativePath ?? file.FullPath,
                        FileType = Convert.ToInt32(file.FileType)
                    };
                    await _fileRepo.AddAsync(reviewFile);
                }
            }
            return  new BaseResponse();
        }



        public async Task<List<ProductReviewFile>> GetOldFilesAsync(Guid reviewId)
        {
            return (await _fileRepo.GetAllAsync()).Where(f => f.ProductReviewId == reviewId).ToList();
        }
        public async Task<BaseResponse> UpdateProductReviewAsync(ProductReviewCreateOrUpdateDTO dto, List<FileUploadResult> filesUpload)
        {
            var review = await _reviewRepo.GetProductReviewByIdAsync(dto.Id.Value);
            if (review == null) return null;
            _mapper.Map(dto, review);
            // Lấy file cũ
            var oldFiles = (await _fileRepo.GetAllAsync()).Where(f => f.ProductReviewId == dto.Id.Value).ToList();
            var keepFileIds = dto.Files?.Where(f => f.Id != null).Select(f => f.Id.Value).ToList() ?? new List<Guid>();
            var filesToDelete = oldFiles.Where(f => !keepFileIds.Contains(f.Id)).ToList();
           await _fileRepo.DeleteRangeAsync(filesToDelete);
            if (filesUpload != null && filesUpload.Count > 0)
            {
                List<ProductReviewFile> lstReview = new List<ProductReviewFile>();
                foreach (var file in filesUpload)
                {
                    var reviewFile = new ProductReviewFile
                    {
                        Id = Guid.NewGuid(),
                        ProductReviewId = review.Id,
                        FileName = file.FileName,
                        FileUrl = file.RelativePath ?? file.FullPath,
                        FileType = file.FileType ?? 1,
                    };
                    lstReview.Add(reviewFile);
                  
                }
                await _fileRepo.AddRangeAsync(lstReview);
            }
            await _reviewRepo.UpdateProductReviewAsync(review);
            return  new BaseResponse();
        }
        public async Task<BaseResponse> DeleteProductReviewAsync(Guid id)
        {
            var oldFiles = (await _fileRepo.GetAllAsync()).Where(f => f.ProductReviewId == id).ToList();
            await _fileRepo.AddRangeAsync(oldFiles);
            await _reviewRepo.DeleteAsync(id);
            return new BaseResponse();
        }
        public async Task<IEnumerable<ProductReview>> GetAllProductReviewsAsync()
        {
            return await _reviewRepo.GetAllProductReviewsAsync();
        }
    }
} 