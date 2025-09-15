using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.PromotionDetail;
using MeoMeo.Domain.Commons;
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
    public class PromotionDetailServices : IPromotionDetailServices
    {
        private readonly IPromotionDetailRepository _repository;
        private readonly IMapper _mapper;

        public PromotionDetailServices(IPromotionDetailRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PromotionDetail> CreatePromotionDetailAsync(CreateOrUpdatePromotionDetailDTO promotionDetail)
        {
            var mappedPromotionDetail = _mapper.Map<PromotionDetail>(promotionDetail);
            mappedPromotionDetail.Id = Guid.NewGuid();
            return await _repository.CreatePromotionDetailAsync(mappedPromotionDetail);
        }

        public async Task<bool> DeletePromotionDetailAsync(Guid id)
        {
            var promotionDetailToDelete = await _repository.GetPromotionDetailByIdAsync(id);

            if (promotionDetailToDelete == null)
            {
                return false;
            }

            await _repository.DeletePromotionDetailAsync(promotionDetailToDelete.Id);
            return true;
        }

        public async Task<PagingExtensions.PagedResult<CreateOrUpdatePromotionDetailDTO>> GetAllPromotionDetailAsync(GetListPromotionDetailRequestDTO request)
        {
            try
            {
                var query = _repository.Query();
                if (request.PromotionIdFilter != Guid.Empty)
                {
                    query = query.Where(c => c.PromotionId == request.PromotionIdFilter);
                }
                if (request.DiscountFilter != null)
                {
                    query = query.Where(c => EF.Functions.Like(c.Discount.ToString(), $"%{request.DiscountFilter}%"));
                }
                if (!string.IsNullOrEmpty(request.NoteFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.Note, $"%{request.NoteFilter}%"));
                }
                var filtedPromotionDetail = await _repository.GetPagedAsync(query, request.PageIndex, request.PageSize);
                var dtoItems = _mapper.Map<List<CreateOrUpdatePromotionDetailDTO>>(filtedPromotionDetail.Items);
                return new PagingExtensions.PagedResult<CreateOrUpdatePromotionDetailDTO>
                {
                    TotalRecords = filtedPromotionDetail.TotalRecords,
                    PageIndex = filtedPromotionDetail.PageIndex,
                    PageSize = filtedPromotionDetail.PageSize,
                    Items = dtoItems
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
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

                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    query = query.Where(pd =>
                        EF.Functions.Like(pd.ProductDetail.Product.Name, $"%{request.SearchTerm}%") ||
                        EF.Functions.Like(pd.ProductDetail.Sku, $"%{request.SearchTerm}%"));
                }

                var filteredPromotionDetails = await _repository.GetPagedAsync(query, request.PageIndex, request.PageSize);

                var dtoItems = filteredPromotionDetails.Items.Select(pd => new PromotionDetailWithProductInfoDTO
                {
                    Id = pd.Id,
                    PromotionId = pd.PromotionId,
                    ProductDetailId = pd.ProductDetailId,
                    Discount = pd.Discount,
                    Note = pd.Note,
                    CreationTime = pd.CreationTime,
                    LastModificationTime = pd.LastModificationTime,

                    // Product Detail Information
                    ProductName = pd.ProductDetail.Product.Name,
                    SKU = pd.ProductDetail.Sku,
                    Thumbnail = pd.ProductDetail.Product.Thumbnail,
                    OriginalPrice = pd.ProductDetail.Price,
                    DiscountPrice = pd.ProductDetail.Price * (1 - pd.Discount / 100),
                    // Size and Colour Information
                    SizeName = pd.ProductDetail.Size.Value,
                    ColourName = pd.ProductDetail.Colour.Name,
                }).ToList();

                return new PagingExtensions.PagedResult<PromotionDetailWithProductInfoDTO>
                {
                    TotalRecords = filteredPromotionDetails.TotalRecords,
                    PageIndex = filteredPromotionDetails.PageIndex,
                    PageSize = filteredPromotionDetails.PageSize,
                    Items = dtoItems
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<CreateOrUpdatePromotionDetailResponseDTO> GetPromotionDetailByIdAsync(Guid id)
        {
            CreateOrUpdatePromotionDetailResponseDTO responseDTO = new CreateOrUpdatePromotionDetailResponseDTO();

            var check = await _repository.GetPromotionDetailByIdAsync(id);
            if (check == null)
            {
                responseDTO.ResponseStatus = BaseStatus.Error;
                responseDTO.Message = "Không tìm thấy promotion detail";
                return responseDTO;
            }

            responseDTO = _mapper.Map<CreateOrUpdatePromotionDetailResponseDTO>(check);
            responseDTO.ResponseStatus = BaseStatus.Success;
            responseDTO.Message = "";
            return responseDTO;
        }

        public async Task<CreateOrUpdatePromotionDetailResponseDTO> UpdatePromotionDetailAsync(CreateOrUpdatePromotionDetailDTO promotionDetail)
        {
            var itemPromotionDetail = await _repository.GetPromotionDetailByIdAsync(Guid.Parse(promotionDetail.Id.ToString()));
            if (itemPromotionDetail == null)
            {
                return new CreateOrUpdatePromotionDetailResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy promotion" };
            }
            _mapper.Map(promotionDetail, itemPromotionDetail);

            await _repository.UpdatePromotionDetailAsync(itemPromotionDetail);
            return new CreateOrUpdatePromotionDetailResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Cập nhật thành công" };
        }
    }
}
