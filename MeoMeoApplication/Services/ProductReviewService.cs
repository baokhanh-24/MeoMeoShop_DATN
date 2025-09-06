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
using MeoMeo.Domain.Commons;
using Microsoft.EntityFrameworkCore;

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
        public async Task<BaseResponse> CreateProductReviewAsync(ProductReviewCreateOrUpdateDTO dto, List<FileUploadResult> filesUpload)
        {
            var review = new ProductReview
            {
                Id = Guid.NewGuid(),
                Content = dto.Content,
                Rating = (float)dto.Rating,
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
            return new BaseResponse();
        }


        public async Task<PagingExtensions.PagedResult<ProductReviewDTO>> GetProductReviewsByProductDetailIdAsync(GetListProductReviewDTO  request)
        {
            var listIds = request.ListProductDetailIds.Split(',').Select(c=> Guid.Parse(c)).ToList();
            var query = _reviewRepo.Query()
                .Where(pr => listIds.Contains(pr.ProductDetailId) && !pr.IsHidden)
                .Include(pr => pr.Customer)
                    .ThenInclude(c => c.User)
                .Include(pr => pr.ProductReviewFiles)
                .OrderByDescending(pr => pr.CreationTime);

            var totalRecords = await query.CountAsync();
            var mainResults = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize).OrderByDescending(c => c.CreationTime)
                .ToListAsync();
            var result = mainResults.Select(review => new ProductReviewDTO
            {
                Id = review.Id,
                Content = review.Content,
                Rating = (decimal)review.Rating,
                IsHidden = review.IsHidden,
                CustomerId = review.CustomerId,
                OrderId = review.OrderId,
                ProductDetailId = review.ProductDetailId,
                Answer = review.Answer,
                ReplyDate = review.ReplyDate,
                CreationTime = review.CreationTime,
                CustomerName = review.Customer?.Name ?? string.Empty,
                CustomerPhone = review.Customer?.PhoneNumber ?? string.Empty,
                CustomerAvatar = review.Customer?.User?.Avatar ?? string.Empty,
                ProductReviewFiles = review.ProductReviewFiles?.Select(f => new ProductReviewFileDTO
                {
                    Id = f.Id,
                    ProductReviewId = f.ProductReviewId,
                    FileName = f.FileName,
                    FileUrl = f.FileUrl,
                    FileType = f.FileType
                }).ToList() ?? new List<ProductReviewFileDTO>()
            }).ToList();

            return new PagingExtensions.PagedResult<ProductReviewDTO>
            {
                TotalRecords = totalRecords,
                PageIndex = 1,
                PageSize = 10,
                Items = result
            };
        }

        public async Task<List<ProductReviewFile>> GetOldFilesAsync(Guid reviewId)
        {
            return (await _fileRepo.GetAllAsync()).Where(f => f.ProductReviewId == reviewId).ToList();
        }
        public async Task<BaseResponse> UpdateProductReviewAsync(ProductReviewCreateOrUpdateDTO dto, List<FileUploadResult> filesUpload)
        {
            var review = await _reviewRepo.GetProductReviewByIdAsync(dto.Id.Value);
            if (review == null)
            {
                return null;
            }
            _mapper.Map(dto, review);
            // Lấy file cũ
            var oldFiles = (await _fileRepo.GetAllAsync()).Where(f => f.ProductReviewId == dto.Id.Value).ToList();
            var keepFileIds = dto.Files?.Where(f => f.Id != null).Select(f => f.Id.Value).ToList() ?? new List<Guid>();
            var filesToDelete = oldFiles.Where(f => !keepFileIds.Contains(f.Id)).ToList();
            foreach (var old in filesToDelete)
            {
                await _fileRepo.DeleteAsync(old.Id);
            }
            // Thêm file mới
            if (filesUpload != null && filesUpload.Count > 0)
            {
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
                    await _fileRepo.AddAsync(reviewFile);
                }
            }
            await _reviewRepo.UpdateProductReviewAsync(review);
            return new BaseResponse();
        }
        public async Task<BaseResponse> DeleteProductReviewAsync(Guid id)
        {
            var oldFiles = (await _fileRepo.GetAllAsync()).Where(f => f.ProductReviewId == id).ToList();
            foreach (var old in oldFiles)
            {
                await _fileRepo.DeleteAsync(old.Id);
            }
            await _reviewRepo.DeleteProductReviewAsync(id);
            return new BaseResponse();
        }

        public async Task<IEnumerable<ProductReview>> GetAllProductReviewsAsync()
        {
            return await _reviewRepo.GetAllProductReviewsAsync();
        }


    }
}