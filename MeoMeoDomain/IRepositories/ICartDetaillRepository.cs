using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface ICartDetaillRepository
    {
        Task<List<CartDetail>> GetAllCartDetail();
        Task<CartDetail> GetCartDetailById(Guid id);
        Task Create(CartDetail cartDetails);
        Task Update(Guid cartDetailId, Guid productId,int quantity);
        Task Delete(Guid id);
    }
}
