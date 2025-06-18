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
    public class CartDetaillService : ICartDetaillService
    {
        private readonly ICartDetaillRepository _cartDetaillRepository;
        private readonly IMapper _mapper;
        public CartDetaillService(ICartDetaillRepository cartDetaillRepository, IMapper mapper)
        {
            _cartDetaillRepository = cartDetaillRepository;
            _mapper = mapper;
        }

        public async Task<CartDetail> CreateCartDetaillAsync(CartDetailDTO cartDetailDTO)
        {
            var cartDetail = new CartDetail
            {
                Id = Guid.NewGuid(), // hoặc để Db tự tạo
                ProductDetailId = cartDetailDTO.ProductId,
                PromotionDetailId = cartDetailDTO.PonmotionId,
                Discount = cartDetailDTO.Discount,
                Price = cartDetailDTO.Price,
                Quantity = cartDetailDTO.Quantity,

                // gán thêm các trường khác nếu có
            };
            //var image = _mapper.Map<Image>(imageDto);
            var updated = await _cartDetaillRepository.Create(cartDetail);
            return updated;
        }

        public async Task<bool> DeleteCartDetaillAsync(Guid id)
        {
            var cartDetail = await _cartDetaillRepository.GetCartDetailById(id);
            if (cartDetail == null)
                throw new ArgumentException("Không tìm thấy ảnh với ID đã cung cấp");

            await _cartDetaillRepository.Delete(id);
            return true;
        }

        public async Task<IEnumerable<CartDetail>> GetAllCartDetaillAsync()
        {
            var cartDetail = await _cartDetaillRepository.GetAllCartDetail();
            return cartDetail;
        }

        public async Task<CartDetail> GetCartDetaillByIdAsync(Guid id)
        {
            var cartDetaill = await _cartDetaillRepository.GetCartDetailById(id);
            return cartDetaill;
        }

        public async Task<CartDetail> UpdataCartDetaillAsync(CartDetailDTO cartDetailDTO)
        {
            var cartDetail = await _cartDetaillRepository.GetCartDetailById(cartDetailDTO.Id.Value);
            if (cartDetail == null)
                throw new Exception("Image not found");

            // Ánh xạ các giá trị từ DTO vào entity đang tồn tại
            _mapper.Map(cartDetailDTO, cartDetail);

            await _cartDetaillRepository.Update(cartDetail);
            return cartDetail;
        }
    }
}
