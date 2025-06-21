using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
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
            mappedPromotion.Id = Guid.NewGuid();
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

        public async Task<List<Promotion>> GetAllPromotionAsync()
        {
            return await _repository.GetAllPromotionAsync();
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
