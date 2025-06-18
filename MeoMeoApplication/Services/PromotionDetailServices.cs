using AutoMapper;
using MeoMeo.Application.IServices;
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

        public async Task<PromotionDetail> GetPromotionDetailByIdAsync(Guid id)
        {
            return await _repository.GetPromotionDetailByIdAsync(id);
        }

        public async Task<PromotionDetail> UpdatePromotionDetailAsync(CreateOrUpdatePromotionDetailDTO promotionDetail)
        {
            PromotionDetail promotionDetailCheck = new PromotionDetail();

            promotionDetailCheck = _mapper.Map<PromotionDetail>(promotionDetail);

            var result = await _repository.UpdatePromotionDetailAsync(promotionDetailCheck);

            return result;
        }
    }
}
