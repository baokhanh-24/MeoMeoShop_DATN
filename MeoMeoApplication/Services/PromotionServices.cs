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
        private readonly IMapper _mapper;

        public PromotionServices(IPromotionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Promotion> CreatePromotionAsync(CreateOrUpdatePromotionDTO promotion)
        {
            var mappedPromotion = _mapper.Map<Promotion>(promotion);
            //mappedPromotion.Id = Guid.NewGuid();
            return await _repository.CreatePromotionAsync(mappedPromotion);
        }

        public async Task<bool> DeletePromotionAsync(Guid id)
        {

            var promotionToDelete = await _repository.GetPromotionByIdAsync(id);

            if (promotionToDelete == null)
            {
                return false;
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
            var itemPromotion = await _repository.GetPromotionByIdAsync(Guid.Parse(promotion.Id.ToString()));
            if (itemPromotion == null)
            {
                return new CreateOrUpdatePromotionResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy promotion" };
            }
            _mapper.Map(promotion, itemPromotion);

            await _repository.UpdatePromotionAsync(itemPromotion);
            return new CreateOrUpdatePromotionResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Cập nhật thành công" };
        }
    }
}
