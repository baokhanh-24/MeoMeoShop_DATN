using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Promotion;
using MeoMeo.Contract.DTOs.PromotionDetail;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Commons.Enums;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.Services
{
    public class PromotionServices : IPromotionServices
    {
        private readonly IPromotionRepository _repository;
        private readonly IPromotionDetailRepository _promotionDetailRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public PromotionServices(IPromotionRepository repository, IPromotionDetailRepository promotionDetailRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _promotionDetailRepository = promotionDetailRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateOrUpdatePromotionResponseDTO> CreatePromotionAsync(CreateOrUpdatePromotionDTO promotion)
        {
            try
            {
                var mappedPromotion = _mapper.Map<Promotion>(promotion);
                mappedPromotion.Id = Guid.NewGuid();
                mappedPromotion.CreationTime = DateTime.Now;

                var createdPromotion = await _repository.CreatePromotionAsync(mappedPromotion);

                return new CreateOrUpdatePromotionResponseDTO
                {
                    Id = createdPromotion.Id,
                    Title = createdPromotion.Title,
                    StartDate = createdPromotion.StartDate,
                    EndDate = createdPromotion.EndDate,
                    Description = createdPromotion.Description,
                    CreationTime = createdPromotion.CreationTime,
                    LastModificationTime = createdPromotion.LastModificationTime,
                    CreatedBy = createdPromotion.CreatedBy,
                    UpdatedBy = createdPromotion.UpdatedBy,
                    ResponseStatus = BaseStatus.Success,
                    Message = "Tạo promotion thành công"
                };
            }
            catch (Exception ex)
            {
                return new CreateOrUpdatePromotionResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Lỗi khi tạo promotion: {ex.Message}"
                };
            }
        }

        public async Task<bool> DeletePromotionAsync(Guid id)
        {
            var promotionToDelete = await _repository.Query()
                .Include(p => p.PromotionDetails)
                .ThenInclude(pd => pd.OrderDetails)
                .ThenInclude(od => od.Order)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (promotionToDelete == null)
            {
                return false;
            }

            // Check if promotion has been used in any completed orders
            var hasBeenUsed = promotionToDelete.PromotionDetails?
                .SelectMany(pd => pd.OrderDetails)
                .Any(od => od.Order.Status == EOrderStatus.Completed) ?? false;

            if (hasBeenUsed)
            {
                return false; // Cannot delete promotion that has been used
            }

            await _repository.DeletePromotionAsync(promotionToDelete.Id);
            return true;
        }

        public async Task<PagingExtensions.PagedResult<CreateOrUpdatePromotionDTO, GetListPromotionResponseDTO>> GetAllPromotionAsync(GetListPromotionRequestDTO request)
        {
            var metaDataValue = new GetListPromotionResponseDTO();
            try
            {
                var query = _repository.Query();

                // Calculate status counts based on dates
                var now = DateTime.Now;
                var allPromotions = await _repository.Query().ToListAsync();
                metaDataValue.TotalAll = allPromotions.Count;
                metaDataValue.NotHappenedYet = allPromotions.Count(p => p.StartDate.HasValue && p.StartDate.Value > now);
                metaDataValue.IsGoingOn = allPromotions.Count(p => p.StartDate.HasValue && p.EndDate.HasValue &&
                    p.StartDate.Value <= now && p.EndDate.Value >= now);
                metaDataValue.Ended = allPromotions.Count(p => p.EndDate.HasValue && p.EndDate.Value < now);

                if (!string.IsNullOrEmpty(request.TitleFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.Title, $"%{request.TitleFilter}%"));
                }
                if (request.StartDateFilter != null)
                {
                    query = query.Where(c => c.StartDate.HasValue && DateOnly.FromDateTime(c.StartDate.Value) == request.StartDateFilter.Value);
                }
                if (request.EndDateFilter != null)
                {
                    query = query.Where(c => c.EndDate.HasValue && DateOnly.FromDateTime(c.EndDate.Value) == request.EndDateFilter.Value);
                }
                if (!string.IsNullOrEmpty(request.DescriptionFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.Description, $"%{request.DescriptionFilter}%"));
                }

                // Sort by CreationTime descending (newest first)
                query = query.OrderByDescending(c => c.CreationTime);

                var filteredPromotion = await _repository.GetPagedAsync(query, request.PageIndex, request.PageSize);
                var dtoItems = _mapper.Map<List<CreateOrUpdatePromotionDTO>>(filteredPromotion.Items);
                metaDataValue.ResponseStatus = BaseStatus.Success;
                return new PagingExtensions.PagedResult<CreateOrUpdatePromotionDTO, GetListPromotionResponseDTO>
                {
                    TotalRecords = filteredPromotion.TotalRecords,
                    PageIndex = filteredPromotion.PageIndex,
                    PageSize = filteredPromotion.PageSize,
                    Items = dtoItems,
                    Metadata = metaDataValue,
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<CreateOrUpdatePromotionResponseDTO> GetPromotionByIdAsync(Guid id)
        {
            CreateOrUpdatePromotionResponseDTO responseDTO = new CreateOrUpdatePromotionResponseDTO();

            var check = await _repository.GetPromotionByIdAsync(id);
            if (check == null)
            {
                responseDTO.ResponseStatus = BaseStatus.Error;
                responseDTO.Message = "Không tìm thấy promotion";
                return responseDTO;
            }

            responseDTO = _mapper.Map<CreateOrUpdatePromotionResponseDTO>(check);
            responseDTO.ResponseStatus = BaseStatus.Success;
            responseDTO.Message = "";
            return responseDTO;
        }

        public async Task<GetPromotionDetailResponseDTO> GetPromotionDetailAsync(Guid id)
        {
            try
            {
                var promotion = await _repository.Query()
                    .Include(p => p.PromotionDetails)
                        .ThenInclude(pd => pd.ProductDetail)
                            .ThenInclude(pd => pd.Product)
                    .Include(p => p.PromotionDetails)
                        .ThenInclude(pd => pd.ProductDetail)
                            .ThenInclude(pd => pd.Size)
                    .Include(p => p.PromotionDetails)
                        .ThenInclude(pd => pd.ProductDetail)
                            .ThenInclude(pd => pd.Colour)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (promotion == null)
                {
                    return new GetPromotionDetailResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Không tìm thấy promotion"
                    };
                }

                var products = promotion.PromotionDetails.Select(pd => new PromotionDetailWithProductInfoDTO
                {
                    Id = pd.Id,
                    PromotionId = pd.PromotionId,
                    ProductDetailId = pd.ProductDetailId,
                    Discount = pd.Discount,
                    Note = pd.Note,
                    CreationTime = pd.CreationTime,
                    LastModificationTime = pd.LastModificationTime,

                    ProductName = pd.ProductDetail.Product.Name,
                    SKU = pd.ProductDetail.Sku,
                    Thumbnail = pd.ProductDetail.Product.Thumbnail,
                    OriginalPrice = pd.ProductDetail.Price,
                    DiscountPrice = pd.ProductDetail.Price * (1 - pd.Discount / 100),
                    SizeName = pd.ProductDetail.Size.Value,
                    ColourName = pd.ProductDetail.Colour.Name,
                }).ToList();

                var totalDiscountAmount = products.Sum(p => p.OriginalPrice - p.DiscountPrice);
                var averageDiscountPercent = products.Any() ? products.Average(p => p.Discount) : 0;

                return new GetPromotionDetailResponseDTO
                {
                    Id = promotion.Id,
                    Title = promotion.Title,
                    StartDate = promotion.StartDate,
                    EndDate = promotion.EndDate,
                    Description = promotion.Description,
                    CreationTime = promotion.CreationTime,
                    LastModificationTime = promotion.LastModificationTime,
                    CreatedBy = promotion.CreatedBy,
                    UpdatedBy = promotion.UpdatedBy,
                    Products = products,
                    TotalProducts = products.Count,
                    TotalDiscountAmount = totalDiscountAmount,
                    AverageDiscountPercent = averageDiscountPercent,
                    ResponseStatus = BaseStatus.Success,
                    Message = "Lấy thông tin promotion thành công"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new GetPromotionDetailResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Lỗi khi lấy thông tin promotion: " + ex.Message
                };
            }
        }

        // Helper method to calculate promotion status based on dates
        public static EPromotionStatus CalculatePromotionStatus(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
                return EPromotionStatus.NotHappenedYet;

            var now = DateTime.Now;

            if (startDate.Value > now)
                return EPromotionStatus.NotHappenedYet;
            else if (startDate.Value <= now && endDate.Value >= now)
                return EPromotionStatus.IsGoingOn;
            else
                return EPromotionStatus.Ended;
        }

        public async Task<CreateOrUpdatePromotionResponseDTO> UpdatePromotionAsync(CreateOrUpdatePromotionDTO promotion)
        {
            var itemPromotion = await _repository.Query()
                .Include(p => p.PromotionDetails)
                .ThenInclude(pd => pd.OrderDetails)
                .ThenInclude(od => od.Order)
                .FirstOrDefaultAsync(p => p.Id == Guid.Parse(promotion.Id.ToString()));

            if (itemPromotion == null)
            {
                return new CreateOrUpdatePromotionResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy promotion" };
            }

            // Check if promotion has been used in any completed orders
            var hasBeenUsed = itemPromotion.PromotionDetails?
                .SelectMany(pd => pd.OrderDetails)
                .Any(od => od.Order.Status == EOrderStatus.Completed) ?? false;

            if (hasBeenUsed)
            {
                return new CreateOrUpdatePromotionResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không thể sửa promotion đã được sử dụng trong đơn hàng"
                };
            }

            _mapper.Map(promotion, itemPromotion);

            await _repository.UpdatePromotionAsync(itemPromotion);
            return new CreateOrUpdatePromotionResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Cập nhật thành công" };
        }

        public async Task<CreateOrUpdatePromotionResponseDTO> CreatePromotionWithDetailsAsync(UpdatePromotionWithDetailsDTO request)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Create new promotion
                var mappedPromotion = _mapper.Map<Promotion>(request.Promotion);
                mappedPromotion.Id = request.PromotionId;
                mappedPromotion.CreationTime = DateTime.Now;

                await _repository.CreatePromotionAsync(mappedPromotion);

                // Create promotion details
                foreach (var detailDto in request.PromotionDetails)
                {
                    var newDetail = new PromotionDetail
                    {
                        Id = Guid.NewGuid(),
                        PromotionId = request.PromotionId,
                        ProductDetailId = detailDto.ProductDetailId.Value,
                        Discount = detailDto.Discount,
                        Note = detailDto.Note,
                        CreationTime = DateTime.Now,
                        LastModificationTime = DateTime.Now
                    };

                    await _promotionDetailRepository.CreatePromotionDetailAsync(newDetail);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return new CreateOrUpdatePromotionResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Tạo promotion và chi tiết thành công" };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new CreateOrUpdatePromotionResponseDTO { ResponseStatus = BaseStatus.Error, Message = $"Lỗi khi tạo: {ex.Message}" };
            }
        }

        public async Task<CreateOrUpdatePromotionResponseDTO> UpdatePromotionWithDetailsAsync(UpdatePromotionWithDetailsDTO request)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var itemPromotion = await _repository.Query()
                    .Include(p => p.PromotionDetails)
                    .ThenInclude(pd => pd.OrderDetails)
                    .ThenInclude(od => od.Order)
                    .FirstOrDefaultAsync(p => p.Id == request.PromotionId);

                if (itemPromotion == null)
                {
                    await _unitOfWork.RollbackAsync();
                    return new CreateOrUpdatePromotionResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy promotion" };
                }

                // Check if promotion has been used in any completed orders
                var hasBeenUsed = itemPromotion.PromotionDetails?
                    .SelectMany(pd => pd.OrderDetails)
                    .Any(od => od.Order.Status == EOrderStatus.Completed) ?? false;

                if (hasBeenUsed)
                {
                    await _unitOfWork.RollbackAsync();
                    return new CreateOrUpdatePromotionResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Không thể sửa promotion đã được sử dụng trong đơn hàng"
                    };
                }

                // Update promotion basic info
                _mapper.Map(request.Promotion, itemPromotion);
                await _repository.UpdatePromotionAsync(itemPromotion);

                // Delete existing promotion details
                if (itemPromotion.PromotionDetails != null && itemPromotion.PromotionDetails.Any())
                {
                    foreach (var detail in itemPromotion.PromotionDetails.ToList())
                    {
                        await _promotionDetailRepository.DeletePromotionDetailAsync(detail.Id);
                    }
                }

                // Create new promotion details
                foreach (var detailDto in request.PromotionDetails)
                {
                    var newDetail = new PromotionDetail
                    {
                        Id = Guid.NewGuid(),
                        PromotionId = request.PromotionId,
                        ProductDetailId = detailDto.ProductDetailId.Value,
                        Discount = detailDto.Discount,
                        Note = detailDto.Note,
                        CreationTime = DateTime.Now,
                        LastModificationTime = DateTime.Now
                    };

                    await _promotionDetailRepository.CreatePromotionDetailAsync(newDetail);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return new CreateOrUpdatePromotionResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Cập nhật promotion và chi tiết thành công" };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new CreateOrUpdatePromotionResponseDTO { ResponseStatus = BaseStatus.Error, Message = $"Lỗi khi cập nhật: {ex.Message}" };
            }
        }
    }
}
