using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;

namespace MeoMeo.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<Order> CreateOrderAsync(CreateOrUpdateOrderDTO order)
        {
            var mappedOrder = _mapper.Map<Order>(order);
            mappedOrder.Id = Guid.NewGuid();
            var result = await _orderRepository.CreateOrderAsync(mappedOrder);
            return result;
        }



        public async Task<bool> DeleteOrderAsync(Guid id)
        {
            var checkOrder = await _orderRepository.GetOrderByIdAsync(id);
            if (checkOrder.Id != id)
            {
                return false;
            }
            else
            {
                await _orderRepository.DeleteOrderAsync(checkOrder);
                return true;
            }
        }

        public Task<IEnumerable<Order>> GetAllAsync()
        {
            return _orderRepository.GetAllAsync();
        }

        public async Task<Order> GetOrderByIdAsync(Guid id)
        {

            var result = await _orderRepository.GetOrderByIdAsync(id);

            return result;
        }

        public async Task<Order> UpdateOrderAsync(CreateOrUpdateOrderDTO order)
        {
            Order itemOrder = new Order();
            itemOrder = _mapper.Map<Order>(order);

            var result = await _orderRepository.UpdateOrderAsync(itemOrder);
            return result;


        }
    }
}
