using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Shared.IServices;
using MeoMeo.Contract.DTOs.Order;
using MeoMeo.Shared.Utilities;

namespace MeoMeo.Shared.Services;

public class CartClientService : ICartClientService
{
    private readonly IApiCaller _apiCaller;

    public CartClientService(IApiCaller apiCaller)
    {
        _apiCaller = apiCaller;
    }

    public async Task<CartResponseDTO?> AddToCartAsync(AddToCartDTO request)
    {
        return await _apiCaller.PostAsync<AddToCartDTO, CartResponseDTO>("api/Cart/addProductToCart", request);
    }

    public async Task<CartWithDetailsResponseDTO?> GetCurrentCartAsync()
    {
        return await _apiCaller.GetAsync<CartWithDetailsResponseDTO>("api/Cart/get-cart-detail");
    }

    public async Task<CartResponseDTO?> UpdateQuantityAsync(UpdateCartQuantityDTO dto)
    {
        return await _apiCaller.PutAsync<UpdateCartQuantityDTO, CartResponseDTO>("api/Cart/update-quantity", dto);
    }

    public async Task<CartResponseDTO?> RemoveItemAsync(Guid cartDetailId)
    {
        var ok = await _apiCaller.DeleteAsync($"api/Cart/remove-item/{cartDetailId}");
        return ok ? new CartResponseDTO { ResponseStatus = BaseStatus.Success } : new CartResponseDTO { ResponseStatus = BaseStatus.Error };
    }

    public async Task<CreateOrderResultDTO?> CheckoutAsync(CreateOrderDTO request)
    {
        return await _apiCaller.PostAsync<CreateOrderDTO, CreateOrderResultDTO>("api/Cart/checkout", request);
    }
}