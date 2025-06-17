using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;

namespace MeoMeo.Application.Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IMapper _mapper;

        public OrderDetailService(IOrderDetailRepository orderDetailRepository, IMapper mapper)
        {
            _orderDetailRepository = orderDetailRepository;
            _mapper = mapper;
        }

        public async Task<OrderDetail> CreateOrderDetailAsync(CreateOrUpdateOrderDetailDTO order)
        {
            var mappedOrderDetail = _mapper.Map<OrderDetail>(order);
            mappedOrderDetail.Id = Guid.NewGuid();
            var result = await _orderDetailRepository.CreateOrderDetailAsync(mappedOrderDetail);
            return result;
        }

        public async Task<bool> DeleteOrderOrderDetailAsync(Guid id)
        {
            var checkOrderDetail = await _orderDetailRepository.GetOrderDetailByIdAsync(id);
            if (checkOrderDetail.Id != id)
            {
                return false;
            }
            else
            {
                await _orderDetailRepository.DeleteOrderDetailAsync(checkOrderDetail);
                return true;
            }
        }

        public Task<IEnumerable<OrderDetail>> GetAllDetailAsync()
        {
            return _orderDetailRepository.GetAllOrderDetailAsync();
        }

        public async Task<OrderDetail> GetOrderDetailByIdAsync(Guid id)
        {
            var result = await _orderDetailRepository.GetOrderDetailByIdAsync(id);
            return result;
        }

        public async Task<OrderDetail> UpdateOrderDetailAsync(CreateOrUpdateOrderDetailDTO order)
        {
            OrderDetail itemOrderDetail = new OrderDetail();
            itemOrderDetail = _mapper.Map<OrderDetail>(order);
            var result = await _orderDetailRepository.UpdateOrderDetailAsync(itemOrderDetail);
            return result;
        }
    }
}
