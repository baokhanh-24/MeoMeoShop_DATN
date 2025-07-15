using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Promotion;
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
                var statusCounts = await _repository.Query().GroupBy(p => p.Status).Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                }).ToListAsync();
                metaDataValue.TotalAll = statusCounts.Sum(s => s.Count);
                metaDataValue.Draft = statusCounts.FirstOrDefault(s => s.Status == EPromotionStatus.Draft)?.Count ?? 0;
                metaDataValue.NotHappenedYet = statusCounts.FirstOrDefault(s => s.Status == EPromotionStatus.NotHappenedYet)?.Count ?? 0;
                metaDataValue.IsGoingOn = statusCounts.FirstOrDefault(s => s.Status == EPromotionStatus.IsGoingOn)?.Count ?? 0;
                metaDataValue.Ended = statusCounts.FirstOrDefault(s => s.Status == EPromotionStatus.Ended)?.Count ?? 0;
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
                if (request.StatusFilter != null)
                {
                    query = query.Where(c => c.Status == request.StatusFilter);
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
