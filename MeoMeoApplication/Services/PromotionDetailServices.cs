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

        public async Task<List<PromotionDetail>> CreatePromotionDetailAsync(CreateOrUpdatePromotionDetailDTO promotionDetail)
        {
            PromotionDetail promotionDetailDB = new PromotionDetail();

            promotionDetailDB.PromotionId = promotionDetail.PromotionId;
            promotionDetailDB.Discount = promotionDetail.Discount;
            promotionDetailDB.Note = promotionDetail.Note;
            promotionDetailDB.CreationTime = promotionDetail.CreationTime;
            promotionDetailDB.LastModificationTime = promotionDetail.LastModificationTime;


            var result = await _repository.CreatePromotionDetailAsync(promotionDetailDB);
            return result;
        }

        public async Task<bool> DeletePromotionDetailAsync(Guid id)
        {
            var lstAllPromotionDetail = await _repository.GetAllAsync();

            var promotionDetailToDelete = lstAllPromotionDetail.FirstOrDefault(x => x.Id == id);

            if (promotionDetailToDelete == null)
            {
                return false;
            }

            var result = await _repository.DeletePromotionDetailAsync(promotionDetailToDelete);

            return result;
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
            PromotionDetail promotionDetailDB = new PromotionDetail();

            promotionDetailDB.Id = promotionDetail.Id;
            promotionDetailDB.PromotionId = promotionDetail.PromotionId;
            promotionDetailDB.Discount = promotionDetail.Discount;
            promotionDetailDB.Note = promotionDetail.Note;
            promotionDetailDB.CreationTime = promotionDetail.CreationTime;
            promotionDetailDB.LastModificationTime = promotionDetail.LastModificationTime;
            

            var result = await _repository.UpdatePromotionDetailAsync(promotionDetailDB);
            return result;
        }
    }
}
