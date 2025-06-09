using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface IPromotionRepository : IBaseRepository<Promotion>
    {
        Task<List<Promotion>> GetAllPromotionAsync();
        Task<Promotion> GetPromotionAsync(Guid id);
        Task<Promotion> UpdatePromotionAsync(Promotion promotion);
        Task<bool> DeletePromotionAsync(Promotion promotion);
    }
}
