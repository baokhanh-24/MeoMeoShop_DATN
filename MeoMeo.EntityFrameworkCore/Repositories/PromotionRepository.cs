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

        public async Task<bool> DeletePromotionAsync(Promotion promotion)
        {
            try
            {
                var promotionDeleted = _context.promotions.Remove(promotion);

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
            return true;
        }

        public async Task<List<Promotion>> GetAllPromotionAsync()
        {
            var getAllPromotion = await GetAllAsync();
            return getAllPromotion.ToList();
        }

        public async Task<Promotion> GetPromotionAsync(Guid id)
        {
            var promotion = await GetByIdAsync(id);
            return promotion;
        }

        public async Task<Promotion> UpdatePromotionAsync(Promotion promotion)
        {
            var promotionUpdated = _context.promotions.Update(promotion);

            await _context.SaveChangesAsync();

            return promotion;
        }
    }
}
