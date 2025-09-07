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
using MeoMeo.Domain.Commons.Enums;
using Microsoft.EntityFrameworkCore;

namespace MeoMeo.Application.Services
{
    public class ProductReviewService : IProductReviewService
    {
        private readonly IProductReviewRepository _reviewRepo;
        private readonly IProductReviewFileRepository _fileRepo;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        public ProductReviewService(IProductReviewRepository reviewRepo,IProductReviewFileRepository fileRepo, IMapper mapper, IOrderRepository orderRepository)
        {
            _reviewRepo = reviewRepo;
            _fileRepo = fileRepo;
            _mapper = mapper;
            _orderRepository = orderRepository;
        }
        public async Task<BaseResponse> CreateProductReviewAsync(ProductReviewCreateOrUpdateDTO dto, List<FileUploadResult> filesUpload)
        {
            var review = new ProductReview
            {
                Id = Guid.NewGuid(),
                Content = dto.Content,
                Rating = (float)dto.Rating,
                IsHidden = dto.IsHidden,
                CustomerId = dto.CustomerId.Value,
                OrderId = dto.OrderId,
                ProductDetailId = dto.ProductDetailId,
            };
            await _reviewRepo.CreateProductReviewAsync(review);
            if (dto.MediaUploads != null && dto.MediaUploads.Count > 0)
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
                .Include(pr => pr.ProductDetail)
                .ThenInclude(c => c.Size)
                .Include(pr => pr.ProductDetail)
                .ThenInclude(c => c.Colour)
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
                CustomerPhone = MaskPhone(review.Customer?.PhoneNumber)  ?? string.Empty,
                CustomerAvatar = review.Customer?.User?.Avatar ?? string.Empty,
                ColourName = review.ProductDetail.Colour?.Name ?? string.Empty,
                SizeName =  review.ProductDetail.Size?.Value ?? string.Empty,
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
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = result
            };
        }
        // Ẩn số điện thoại - chỉ hiển thị 3 số đầu và 3 số cuối
    public static string MaskPhone(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return phoneNumber;

        // Loại bỏ tất cả ký tự không phải số
        string cleanPhone = new string(phoneNumber.Where(char.IsDigit).ToArray());
        
        if (cleanPhone.Length < 6)
            return phoneNumber; // Nếu số quá ngắn, trả về nguyên bản

        if (cleanPhone.Length <= 10)
        {
            // Số điện thoại ngắn: hiển thị 3 số đầu + *** + 3 số cuối
            return $"{cleanPhone.Substring(0, 3)}***{cleanPhone.Substring(cleanPhone.Length - 3)}";
        }
        else
        {
            // Số điện thoại dài (có mã quốc gia): hiển thị 4 số đầu + *** + 3 số cuối
            return $"{cleanPhone.Substring(0, 4)}***{cleanPhone.Substring(cleanPhone.Length - 3)}";
        }
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
            var keepFileIds = dto.MediaUploads?.Where(f => f.Id != null).Select(f => f.Id.Value).ToList() ?? new List<Guid>();
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

        public async Task<List<OrderItemForReviewDTO>> GetUnreviewedOrderItemsAsync(Guid customerId)
        {
            try
            {
                // Lấy tất cả đơn hàng đã hoàn thành của customer
                var completedOrders = await _orderRepository.Query()
                    .Where(o => o.CustomerId == customerId && o.Status == EOrderStatus.Completed)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.ProductDetail)
                            .ThenInclude(pd => pd.Product)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.ProductDetail)
                            .ThenInclude(pd => pd.Size)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.ProductDetail)
                            .ThenInclude(pd => pd.Colour)
                    .ToListAsync();

                // Lấy tất cả ProductDetailId đã được review bởi customer này
                var reviewedProductDetailIds = await _reviewRepo.Query()
                    .Where(r => r.CustomerId == customerId)
                    .Select(r => r.ProductDetailId)
                    .ToListAsync();

                var unreviewedItems = new List<OrderItemForReviewDTO>();

                foreach (var order in completedOrders)
                {
                    foreach (var orderDetail in order.OrderDetails)
                    {
                        // Chỉ lấy những sản phẩm chưa được review
                        if (!reviewedProductDetailIds.Contains(orderDetail.ProductDetailId))
                        {
                            unreviewedItems.Add(new OrderItemForReviewDTO
                            {
                                OrderId = order.Id,
                                OrderCode = order.Code,
                                OrderDate = order.CreationTime,
                                ProductDetailId = orderDetail.ProductDetailId,
                                ProductName = orderDetail.ProductDetail?.Product?.Name ?? "N/A",
                                ProductImage = orderDetail.ProductDetail?.Product?.Thumbnail ?? "",
                                SizeName = orderDetail.ProductDetail?.Size.Value ?? "N/A",
                                ColorName = orderDetail.ProductDetail?.Colour?.Name ?? "N/A",
                                Quantity = orderDetail.Quantity,
                                Price =(decimal) orderDetail.Price,
                                Discount =(decimal) orderDetail.Discount
                            });
                        }
                    }
                }

                return unreviewedItems.OrderByDescending(x => x.OrderDate).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting unreviewed order items: {ex.Message}");
            }
        }

        public async Task<List<ProductReviewDTO>> GetCustomerReviewsAsync(Guid customerId)
        {
            try
            {
                var reviews = await _reviewRepo.Query()
                    .Where(r => r.CustomerId == customerId)
                    .Include(r => r.ProductDetail)
                        .ThenInclude(pd => pd.Product)
                    .Include(r => r.ProductDetail)
                        .ThenInclude(pd => pd.Size)
                    .Include(r => r.ProductDetail)
                        .ThenInclude(pd => pd.Colour)
                    .Include(r => r.ProductReviewFiles)
                    .OrderByDescending(r => r.CreationTime)
                    .ToListAsync();

                return _mapper.Map<List<ProductReviewDTO>>(reviews);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting customer reviews: {ex.Message}");
            }
        }


    }
}