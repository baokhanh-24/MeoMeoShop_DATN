using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
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
            var images = await _cartRepository.GetAllCart();
            return images;
        }

        public async Task<Cart> GetCartByIdAsync(Guid id)
        {
            var image = await _cartRepository.GetCartById(id);
            return image;
        }

        public async Task<Cart> CreateCartAsync(CartDTO cartDto)
        {
            var image = new Cart
            {
                Id = Guid.NewGuid(), // hoặc để Db tự tạo
                CustomerId = cartDto.CustomersId,
                CreatedBy = cartDto.createBy,
                CreationTime = cartDto.NgayTao,
                LastModificationTime = cartDto.lastModificationTime,
                TotalPrice = cartDto.TongTien
                // gán thêm các trường khác nếu có
            };
            var updated = await _cartRepository.Create(image);
            return updated;
        }

        public async Task<Cart> UpdateCartAsync(CartDTO cartDto)
        {
            var image = await _cartRepository.GetCartById(cartDto.Id);
            if (image == null)
                throw new Exception("Image not found");

            // Ánh xạ các giá trị từ DTO vào entity đang tồn tại
            image.CustomerId = cartDto.CustomersId;
            image.TotalPrice = cartDto.TongTien;
            image.CreationTime = cartDto.NgayTao;
            image.CreatedBy = cartDto.createBy;
            image.LastModificationTime = DateTime.Now;

            await _cartRepository.Update(image);
            return image;
        }

        public async Task SaveChangesAsync()
        {
            await _cartRepository.Savechanges();
        }
    }
}
