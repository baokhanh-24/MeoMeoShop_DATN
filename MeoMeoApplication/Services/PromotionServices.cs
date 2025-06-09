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
            return await _repository.AddAsync(mappedPromotion);
        }

        public async Task<bool> DeletePromotionAsync(Guid id)
        {

            var lstAllPromotion = await _repository.GetAllAsync();

            var promotionToDelete = lstAllPromotion.FirstOrDefault(x => x.Id == id);

            if (promotionToDelete == null)
            {
                return false;
            }

            var result = await _repository.DeletePromotionAsync(promotionToDelete);

            return result;

        }

        public async Task<List<Promotion>> GetAllPromotionAsync()
        {
            return await _repository.GetAllPromotionAsync();
        }

        public async Task<Promotion> GetPromotionByIdAsync(Guid id)
        {
            return await _repository.GetPromotionAsync(id);
        }

        public async Task<Promotion> UpdatePromotionAsync(CreateOrUpdatePromotionDTO promotion)
        {
            Promotion promotionDB = new Promotion();

            promotionDB.Id = promotion.Id;
            promotionDB.Title = promotion.Title;
            promotionDB.StartDate = promotion.StartDate;
            promotionDB.EndDate = promotion.EndDate;
            promotionDB.Description = promotion.Description;
            promotionDB.Status = promotion.Status;


            var result = await _repository.UpdatePromotionAsync(promotionDB);
            return result;
        }
    }
}
