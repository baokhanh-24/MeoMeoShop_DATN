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

        public async Task<Promotion> GetPromotionByIdAsync(Guid id)
        {
            return await _repository.GetPromotionByIdAsync(id);
        }

        public async Task<Promotion> UpdatePromotionAsync(CreateOrUpdatePromotionDTO promotion)
        {
            Promotion promotionCheck = new Promotion();

            promotionCheck = _mapper.Map<Promotion>(promotion);

            var result = await _repository.UpdatePromotionAsync(promotionCheck);

            return result;
        }
    }
}
