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

        public async Task<List<PromotionDetail>> GetAllPromotionDetailAsync()
        {
            return await _repository.GetAllPromotionDetailAsync();
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
