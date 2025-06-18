using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface ICartService
    {
        Task<IEnumerable<Cart>> GetAllCartsAsync();
        Task<Cart> GetCartByIdAsync(Guid id);
        Task<Cart> CreateCartAsync(CartDTO cartDto);
        Task<Cart> UpdateCartAsync(CartDTO cartDto);
        Task SaveChangesAsync();
    }
}
