using MeoMeo.Contract.DTOs.Order;

namespace MeoMeo.Shared.IServices;

using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;

public interface ICartClientService
{
    Task<CartResponseDTO?> AddToCartAsync(AddToCartDTO request);
    Task<CartWithDetailsResponseDTO?> GetCurrentCartAsync();
    Task<CartResponseDTO?> UpdateQuantityAsync(UpdateCartQuantityDTO dto);
    Task<CartResponseDTO?> RemoveItemAsync(Guid cartDetailId);
    Task<CreateOrderResultDTO?> CheckoutAsync(CreateOrderDTO request);
}