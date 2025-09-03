using Humanizer;
using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        //
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _cartService.GetAllCartsAsync();
            return Ok(result);
        }
        //
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _cartService.GetCartByIdAsync(id);
            var customerId = _httpContextAccessor.HttpContext.GetCurrentCustomerId();
            if (customerId == Guid.Empty)
            {
                return new CartResponseDTO()
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng"
                };
            }
            request.CustomerId ??= customerId;
            var result = await _cartService.AddProductToCartAsync(request);
            if (result.ResponseStatus == BaseStatus.Error)
            {
                return new CartResponseDTO()
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = result.Message
                };
            }

            return result;
        }

        // update quantity of a cart detail
        [HttpPut("update-quantity")]
        public async Task<CartResponseDTO> UpdateQuantity([FromBody] UpdateCartQuantityDTO request)
        {
            var customerId = _httpContextAccessor.HttpContext.GetCurrentCustomerId();
            if (customerId == Guid.Empty)
            {
                return new CartResponseDTO()
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng"
                };
            }
            request.CustomerId ??= customerId;
            var result = await _cartService.UpdateCartQuantityAsync(request);
            if (result.ResponseStatus == BaseStatus.Error)
            {
                return new CartResponseDTO()
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = result.Message
                };
            }

            return result;
        }

        // checkout selected cart items -> create order from cart
        [HttpPost("checkout")]
        public async Task<BaseResponse> Checkout([FromBody] CreateOrderDTO request)
        {
            var customerId = _httpContextAccessor.HttpContext.GetCurrentCustomerId();
            if (customerId == Guid.Empty)
            {
                return new BaseResponse()
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Vui lòng đăng nhập để thanh toán đơn hàng"
                };
            }
            var userId = _httpContextAccessor.HttpContext.GetCurrentUserId();
            var result = await _orderService.CreateOrderAsync(customerId, userId, request);
            return result;
        }

        // remove a cart detail item
        [HttpDelete("remove-item/{id}")]
        public async Task<IActionResult> RemoveItem(Guid id)
        {
            var userId = _httpContextAccessor.HttpContext.GetCurrentUserId();
            if (userId == Guid.Empty) return BadRequest();
            var result = await _cartService.RemoveCartItemAsync(id, userId);
            return Ok(result);
        }
        //add
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CartDTO cartDTO)
        {
            var result = await _cartService.CreateCartAsync(cartDTO);
            return Ok(result);
        }
        //
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CartDTO cartDto)
        {
            var result = await _cartService.UpdateCartAsync(cartDto);
            return Ok(result);
        }
    }
}
