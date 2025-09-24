using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.PromotionDetail;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace MeoMeo.Application.Services
{
    public class PromotionDetailServices : IPromotionDetailServices
    {
        private readonly IPromotionDetailRepository _repository;
        private readonly IMapper _mapper;

        public PromotionDetailServices(IPromotionDetailRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagingExtensions.PagedResult<CreateOrUpdatePromotionDetailDTO>> GetAllPromotionDetailAsync(GetListPromotionDetailRequestDTO request)
        {
            var query = _repository.Query();

            if (request.PromotionIdFilter != Guid.Empty)
            {
                query = query.Where(pd => pd.PromotionId == request.PromotionIdFilter);
            }

            query = query.OrderByDescending(pd => pd.CreationTime);

            var pagedResult = await _repository.GetPagedAsync(query, request.PageIndex, request.PageSize);
            var dtoList = _mapper.Map<List<CreateOrUpdatePromotionDetailDTO>>(pagedResult.Items);

            return new PagingExtensions.PagedResult<CreateOrUpdatePromotionDetailDTO>
            {
                TotalRecords = pagedResult.TotalRecords,
                PageIndex = pagedResult.PageIndex,
                PageSize = pagedResult.PageSize,
                Items = dtoList
            };
        }

        public async Task<PagingExtensions.PagedResult<PromotionDetailWithProductInfoDTO>> GetPromotionDetailWithProductInfoAsync(GetPromotionDetailWithProductInfoRequestDTO request)
        {
            try
            {
                var query = _repository.Query()
                    .Include(pd => pd.ProductDetail)
                        .ThenInclude(pd => pd.Product)
                    .Include(pd => pd.ProductDetail)
                        .ThenInclude(pd => pd.Size)
                    .Include(pd => pd.ProductDetail)
                        .ThenInclude(pd => pd.Colour)
                    .Where(pd => pd.PromotionId == request.PromotionId);

                query = query.OrderByDescending(pd => pd.CreationTime);

                var pagedResult = await _repository.GetPagedAsync(query, request.PageIndex, request.PageSize);

                var dtoList = pagedResult.Items.Select(pd => new PromotionDetailWithProductInfoDTO
                {
                    Id = pd.Id,
                    PromotionId = pd.PromotionId,
                    ProductDetailId = pd.ProductDetailId,
                    Discount = pd.Discount,
                    Note = pd.Note ?? string.Empty,
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

                return new PagingExtensions.PagedResult<PromotionDetailWithProductInfoDTO>
                {
                    TotalRecords = pagedResult.TotalRecords,
                    PageIndex = pagedResult.PageIndex,
                    PageSize = pagedResult.PageSize,
                    Items = dtoList
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPromotionDetailWithProductInfoAsync: {ex.Message}");
                return new PagingExtensions.PagedResult<PromotionDetailWithProductInfoDTO>
                {
                    TotalRecords = 0,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    Items = new List<PromotionDetailWithProductInfoDTO>()
                };
            }
        }

        public async Task<CreateOrUpdatePromotionDetailResponseDTO> GetPromotionDetailByIdAsync(Guid id)
        {
            var promotionDetail = await _repository.GetByIdAsync(id);
            if (promotionDetail == null)
            {
                return new CreateOrUpdatePromotionDetailResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không tìm thấy promotion detail"
                };
            }

            var dto = _mapper.Map<CreateOrUpdatePromotionDetailResponseDTO>(promotionDetail);
            dto.ResponseStatus = BaseStatus.Success;
            return dto;
        }

        public async Task<PromotionDetail> CreatePromotionDetailAsync(CreateOrUpdatePromotionDetailDTO promotionDetail)
        {
            var entity = _mapper.Map<PromotionDetail>(promotionDetail);
            entity.Id = Guid.NewGuid();
            entity.CreationTime = DateTime.Now;
            entity.LastModificationTime = DateTime.Now;

            return await _repository.CreatePromotionDetailAsync(entity);
        }

        public async Task<CreateOrUpdatePromotionDetailResponseDTO> UpdatePromotionDetailAsync(CreateOrUpdatePromotionDetailDTO promotionDetail)
        {
            var existing = await _repository.GetByIdAsync(promotionDetail.Id!.Value);
            if (existing == null)
            {
                return new CreateOrUpdatePromotionDetailResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không tìm thấy promotion detail"
                };
            }

            _mapper.Map(promotionDetail, existing);
            existing.LastModificationTime = DateTime.Now;

            await _repository.UpdatePromotionDetailAsync(existing);
            var dto = _mapper.Map<CreateOrUpdatePromotionDetailResponseDTO>(existing);
            dto.ResponseStatus = BaseStatus.Success;
            return dto;
        }

        public async Task<bool> DeletePromotionDetailAsync(Guid id)
        {
            var promotionDetail = await _repository.GetByIdAsync(id);
            if (promotionDetail == null)
            {
                return false;
            }

            await _repository.DeletePromotionDetailAsync(id);
            return true;
        }
    }
}
