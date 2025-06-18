using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface ICartDetaillRepository : IBaseRepository<CartDetail>
    {
        public Task<IEnumerable<CartDetail>> GetAllCartDetail();
        public Task<CartDetail> GetCartDetailById(Guid id);
        public Task<CartDetail> Create(CartDetail cartDetails);
        public Task<CartDetail> Update(CartDetail cartDetails);
        public Task<bool> Delete(Guid id);
    }
}
