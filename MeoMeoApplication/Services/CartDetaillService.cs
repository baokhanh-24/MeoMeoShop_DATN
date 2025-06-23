using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
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

        public async Task<CartDetailResponseDTO> CreateCartDetaillAsync(CartDetailDTO cartDetailDTO)
        {
            var entity = new CartDetail
            {
                Id = Guid.NewGuid(),
                CartId = cartDetailDTO.CartId,
                ProductDetailId = cartDetailDTO.ProductId,
                PromotionDetailId = cartDetailDTO.PonmotionId,
                Discount = cartDetailDTO.Discount,
                Price = cartDetailDTO.Price,
                Quantity = cartDetailDTO.Quantity,
                Status = cartDetailDTO.Status,
            };

            await _cartDetaillRepository.Create(entity);

            return new CartDetailResponseDTO
            {
                ResponseStatus = BaseStatus.Success,
                Message = "Thêm chi tiết giỏ hàng thành công"
            };
        }

        public async Task<CartDetailResponseDTO> DeleteCartDetaillAsync(Guid id)
        {
            var entity = await _cartDetaillRepository.GetCartDetailById(id);
            if (entity == null)
            {
                return new CartDetailResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy Id" };
            }

            await _cartDetaillRepository.Delete(id);
            return new CartDetailResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Xóa thành công" };
        }

        public async Task<IEnumerable<CartDetail>> GetAllCartDetaillAsync()
        {
            var cartDetail = await _cartDetaillRepository.GetAllCartDetail();
            return cartDetail;
        }

        public async Task<CartDetailResponseDTO> GetCartDetaillByIdAsync(Guid id)
        {
            var cartDetaill = await _cartDetaillRepository.GetCartDetailById(id);
            if(cartDetaill == null)
            {
                return new CartDetailResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Không tìm thấy giỏ hàng chi tiết với ID: {id}"
                };
            }
            return new CartDetailResponseDTO 
            { 
                Id = cartDetaill.Id,
                CartId = cartDetaill.CartId,
                ProductId = cartDetaill.ProductDetailId,
                PonmotionId = cartDetaill.PromotionDetailId,
                Discount = cartDetaill.Discount,
                Price = cartDetaill.Price,
                Quantity = cartDetaill.Quantity,
                Status = cartDetaill.Status,
                ResponseStatus = BaseStatus.Success,
                Message = $""
            };
        }

        public async Task<CartDetailResponseDTO> UpdataCartDetaillAsync(CartDetailDTO cartDetailDTO)
        {
            var entity = await _cartDetaillRepository.GetCartDetailById(cartDetailDTO.Id.Value);
            if (entity == null)
            {
                return new CartDetailResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy Id trên để cập nhật" };
            }
            _mapper.Map(cartDetailDTO, entity);
            await _cartDetaillRepository.Update(entity);

            return new CartDetailResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Cập nhật thành công" };
        }
    }
}
