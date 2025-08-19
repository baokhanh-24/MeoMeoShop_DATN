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
        Task<CartResponseDTO> GetCartByUserIdAsync(Guid userId);
        Task<CartResponseDTO> CreateCartAsync(CartDTO cartDto);
        Task<CartResponseDTO> UpdateCartAsync(CartDTO cartDto);
        Task SaveChangesAsync();
        Task<CartResponseDTO> AddProductToCartAsync(AddToCartDTO cartDto);
        Task<CartWithDetailsResponseDTO> GetCurrentCartAsync(Guid customerId);
        Task<CartResponseDTO> UpdateCartQuantityAsync(UpdateCartQuantityDTO dto);
        Task<CartResponseDTO> RemoveCartItemAsync(Guid cartDetailId, Guid customerId);
    }
}
