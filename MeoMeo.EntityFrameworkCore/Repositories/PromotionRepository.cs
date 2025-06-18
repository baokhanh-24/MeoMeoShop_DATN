using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class PromotionRepository : BaseRepository<Promotion>, IPromotionRepository
    {
        public PromotionRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<Promotion> CreatePromotionAsync(Promotion promotion)
        {
            var promotionAdded = await AddAsync(promotion);
            return promotionAdded;
        }

        public async Task<bool> DeletePromotionAsync(Guid id)
        {
            await DeleteAsync(id);
            return true;
        }

        public async Task<List<Promotion>> GetAllPromotionAsync()
        {
            var getAllPromotion = await GetAllAsync();
            return getAllPromotion.ToList();
        }

        public async Task<Promotion> GetPromotionByIdAsync(Guid id)
        {
            var promotion = await GetByIdAsync(id);
            return promotion;
        }

        public async Task<Promotion> UpdatePromotionAsync(Promotion promotion)
        {
            var promotionUpdated = await UpdateAsync(promotion);
            return promotionUpdated;
        }
    }
}
