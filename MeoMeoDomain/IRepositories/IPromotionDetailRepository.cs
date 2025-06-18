using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface IPromotionDetailRepository : IBaseRepository<PromotionDetail>
    {
        Task<PromotionDetail> CreatePromotionDetailAsync(PromotionDetail promotionDetail);
        Task<List<PromotionDetail>> GetAllPromotionDetailAsync();
        Task<PromotionDetail> GetPromotionDetailByIdAsync(Guid id);
        Task<PromotionDetail> UpdatePromotionDetailAsync(PromotionDetail promotionDetail);
        Task<bool> DeletePromotionDetailAsync(Guid id);
    }
}
