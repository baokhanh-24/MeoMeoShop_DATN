using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class PromotionDetailRepository : BaseRepository<PromotionDetail>, IPromotionDetailRepository
    {
        public PromotionDetailRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<PromotionDetail> CreatePromotionDetailAsync(PromotionDetail promotionDetail)
        {
            var promotionDetailAdded = await AddAsync(promotionDetail);
            return promotionDetailAdded;
        }

        public async Task<bool> DeletePromotionDetailAsync(Guid id)
        {
            await DeleteAsync(id);
            return true;
        }

        public async Task<List<PromotionDetail>> GetAllPromotionDetailAsync()
        {
            var getAllPromotionDetail = await GetAllAsync();
            return getAllPromotionDetail.ToList();
        }

        public async Task<PromotionDetail> GetPromotionDetailByIdAsync(Guid id)
        {
            var promotionDetail = await GetByIdAsync(id);
            return promotionDetail;
        }

        public async Task<PromotionDetail> UpdatePromotionDetailAsync(PromotionDetail promotionDetail)
        {
            var promotionDetailUpdated = await UpdateAsync(promotionDetail);
            return promotionDetailUpdated;
        }
    }
}
