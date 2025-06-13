using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface ICartRepository
    {
        Task<List<Cart>> GetAllCart();
        Task<Cart> GetCartById(Guid id);
        Task Create(Cart cart);
        Task Update(Cart cart);
        Task Savechanges();
    }
}
