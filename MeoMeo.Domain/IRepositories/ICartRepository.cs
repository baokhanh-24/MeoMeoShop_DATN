using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface ICartRepository : IBaseRepository<Cart>
    {
        public Task<IEnumerable<Cart>> GetAllCart();
        public Task<Cart> GetCartById(Guid id);
        public Task<Cart> Create(Cart cart);
        public Task<Cart> Update(Cart cart);
        public Task Savechanges();
        Task<Cart?> GetByCustomerIdWithDetailsAsync(Guid customerId);
    }
} 