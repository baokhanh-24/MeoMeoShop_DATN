using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
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

        public async Task<CreateOrUpdateOrderResponse> GetOrderByIdAsync(Guid id)
        {
            CreateOrUpdateOrderResponse response = new CreateOrUpdateOrderResponse();
            var result = await _orderRepository.GetOrderByIdAsync(id);
            if (result == null)
            {
                response.Message = "Không tìm thấy đơn hàng này";
                response.ResponseStatus = BaseStatus.Error;
                return response;
            }
            response = _mapper.Map<CreateOrUpdateOrderResponse>(result);
            response.Message = "";
            response.ResponseStatus = BaseStatus.Success;
            return response;
        }

        public async Task<CreateOrUpdateOrderResponse> UpdateOrderAsync(CreateOrUpdateOrderDTO order)
        {
            CreateOrUpdateOrderResponse response = new CreateOrUpdateOrderResponse();
            Order itemOrder = new Order();

            var check = await _orderRepository.GetOrderByIdAsync(Guid.Parse(order.Id.ToString()));
            if (check == null)
            {
                response.Message = "Không tìm thấy Order";
                response.ResponseStatus = BaseStatus.Error;
                return response;
                //throw new Exception("Không tìm thấy Order");
            }
            itemOrder = _mapper.Map(order, check);
            var result = await _orderRepository.UpdateOrderAsync(itemOrder);

            response = _mapper.Map<CreateOrUpdateOrderResponse>(result);

            response.Message = "";
            response.ResponseStatus = BaseStatus.Success;
            return response;
        }
    }
}
