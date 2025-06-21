using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        public CartService(ICartRepository cartRepository, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<Cart>> GetAllCartsAsync()
        {
            var cart = await _cartRepository.GetAllCart();
            return cart;
        }

        public async Task<CartResponseDTO> GetCartByIdAsync(Guid id)
        {
            var cart = await _cartRepository.GetCartById(id);
            if(cart == null)
            {
                return new CartResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Không tìm thấy giỏ hàng với ID: {id}"
                };
            }
            return new CartResponseDTO 
            { 
                Id = cart.Id,
                CustomersId = cart.CustomerId,
                createBy = cart.CreatedBy,
                NgayTao = cart.CreationTime,
                lastModificationTime = cart.LastModificationTime,
                TongTien = cart.TotalPrice,
                ResponseStatus = BaseStatus.Success,
                Message = $""
            };
        }

        public async Task<CartResponseDTO> CreateCartAsync(CartDTO cartDto)
        {
            var cart = new Cart
            {
                Id = Guid.NewGuid(), // hoặc để Db tự tạo
                CustomerId = cartDto.CustomersId,
                CreatedBy = cartDto.createBy,
                CreationTime = cartDto.NgayTao,
                LastModificationTime = cartDto.lastModificationTime,
                TotalPrice = cartDto.TongTien
                // gán thêm các trường khác nếu có
            };
           await _cartRepository.Create(cart);
            return new CartResponseDTO
            {
                ResponseStatus = BaseStatus.Success,
                Message = "Thêm giỏ hàng thành công"
            };
        }

        public async Task<CartResponseDTO> UpdateCartAsync(CartDTO cartDto)
        {
            var cart = await _cartRepository.GetCartById(cartDto.Id);
            if (cart == null)
            {
                return new CartResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy Id trên để cập nhật" };
            }
            _mapper.Map(cartDto, cart);
            await _cartRepository.Update(cart);
            return new CartResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Cập nhật thành công" };
        }

        public async Task SaveChangesAsync()
        {
            await _cartRepository.Savechanges();
        }
    }
}
