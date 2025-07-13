using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.BusinessRules;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Order;
using MeoMeo.Contract.DTOs.OrderDetail;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Commons.Enums;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace MeoMeo.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IIventoryBtachReposiory _inventoryRepository;
        private readonly ICartDetaillRepository _cartDetailRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductsDetailRepository _productsDetailRepository;
        private readonly IInventoryTranSactionRepository _inventoryTransactionRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailInventoryBatchRepository _orderDetailInventoryBatchRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IIventoryBtachReposiory inventoryRepository, IOrderRepository orderRepository,
            IMapper mapper, IOrderDetailRepository orderDetailRepository,
            IInventoryTranSactionRepository inventoryTransactionRepository,
            IProductsDetailRepository productsDetailRepository, IUnitOfWork unitOfWork,
            IOrderDetailInventoryBatchRepository orderDetailInventoryBatchRepository, ICartDetaillRepository cartDetailRepository, ICartRepository cartRepository)
        {
            _inventoryRepository = inventoryRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
            _orderDetailRepository = orderDetailRepository;
            _inventoryTransactionRepository = inventoryTransactionRepository;
            _productsDetailRepository = productsDetailRepository;
            _unitOfWork = unitOfWork;
            _orderDetailInventoryBatchRepository = orderDetailInventoryBatchRepository;
            _cartDetailRepository = cartDetailRepository;
            _cartRepository = cartRepository;
        }


        public async Task<PagingExtensions.PagedResult<OrderDTO,GetListOrderResponseDTO>> GetListOrderAsync(GetListOrderRequestDTO request)
        {
            var metaDataValue = new GetListOrderResponseDTO();
            try
            {
                var query = _orderRepository.Query();
                var statusCounts = await _orderRepository.Query()
                    .GroupBy(o => o.Status)
                    .Select(g => new
                    {
                        Status = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();
                metaDataValue.TotalAll = statusCounts.Sum(s => s.Count);
                metaDataValue.Pending = statusCounts.FirstOrDefault(s => s.Status == EOrderStatus.Pending)?.Count ?? 0;
                metaDataValue.Confirmed =
                    statusCounts.FirstOrDefault(s => s.Status == EOrderStatus.Confirmed)?.Count ?? 0;
                metaDataValue.InTransit =
                    statusCounts.FirstOrDefault(s => s.Status == EOrderStatus.InTransit)?.Count ?? 0;
                metaDataValue.Canceled =
                    statusCounts.FirstOrDefault(s => s.Status == EOrderStatus.Canceled)?.Count ?? 0;
                metaDataValue.Completed =
                    statusCounts.FirstOrDefault(s => s.Status == EOrderStatus.Completed)?.Count ?? 0;

                if (!string.IsNullOrEmpty(request.CodeFilter))
                {
                    query = query.Where(o => EF.Functions.Like(o.Code, $"%{request.CodeFilter}%"));
                }

                if (request.CustomerNameFilter.HasValue)
                {
                    query = query.Where(o => o.CustomerId == request.CustomerNameFilter.Value);
                }

                if (!string.IsNullOrEmpty(request.CustomerPhoneNumberFilter))
                {
                    query = query.Where(o =>
                        EF.Functions.Like(o.CustomerPhoneNumber, $"%{request.CustomerPhoneNumberFilter}%"));
                }

                if (!string.IsNullOrEmpty(request.CustomerEmailFilter))
                {
                    query = query.Where(o => EF.Functions.Like(o.CustomerEmail, $"%{request.CustomerEmailFilter}%"));
                }

                if (request.CreationDateStartFilter.HasValue)
                {
                    query = query.Where(o => o.CreationTime >= request.CreationDateStartFilter.Value);
                }

                if (request.CreationDateEndFilter.HasValue)
                {
                    query = query.Where(o => o.CreationTime <= request.CreationDateEndFilter.Value);
                }

                if (request.PaymentMethodFilter.HasValue)
                {
                    query = query.Where(o => o.PaymentMethod == request.PaymentMethodFilter.Value);
                }

                if (request.OrderStatusFilter.HasValue)
                {
                    query = query.Where(o => o.Status == request.OrderStatusFilter.Value);
                }

                query = query.OrderByDescending(o => o.CreationTime);
                var pagedOrders = await _orderRepository.GetPagedAsync(query, request.PageIndex, request.PageSize);

                var dtoItems = _mapper.Map<List<OrderDTO>>(pagedOrders.Items);
                var listOrderIds = dtoItems.Select(c => c.Id).ToList();

                var listOrderDetails = await _orderDetailRepository.Query()
                    .Where(c => listOrderIds.Contains(c.OrderId))
                    .ToListAsync();
                var groupedOrderDetails = listOrderDetails
                    .GroupBy(d => d.OrderId)
                    .ToDictionary(g => g.Key, g => _mapper.Map<List<OrderDetailDTO>>(g.ToList()));
                foreach (var order in dtoItems)
                {
                    if (groupedOrderDetails.TryGetValue(order.Id, out var details))
                    {
                        order.OrderDetails = details;
                        foreach (var detail in order.OrderDetails)
                        {
                            var finalPrice = detail.Discount > 100
                                ? detail.Price - detail.Discount
                                : detail.Price * (1 - detail.Discount / 100);
                            detail.GrandTotal = finalPrice * detail.Quantity;
                        }
                    }
                    else
                    {
                        order.OrderDetails = new List<OrderDetailDTO>();
                    }
                }

                metaDataValue.ResponseStatus = BaseStatus.Success;
                return new PagingExtensions.PagedResult<OrderDTO, GetListOrderResponseDTO>
                {
                    TotalRecords = pagedOrders.TotalRecords,
                    PageIndex = pagedOrders.PageIndex,
                    PageSize = pagedOrders.PageSize,
                    Items = dtoItems,
                    Metadata = metaDataValue,
                };
            }
            catch (Exception ex)
            {
                metaDataValue.ResponseStatus = BaseStatus.Error;
                metaDataValue.Message = ex.Message;
                return new PagingExtensions.PagedResult<OrderDTO, GetListOrderResponseDTO>
                {
                    TotalRecords = 0,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    Items = new List<OrderDTO>(),
                    Metadata = metaDataValue,
                };
            }
        }

        public async Task<BaseResponse> UpdateStatusOrderAsync(UpdateStatusOrderRequestDTO request)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var orders = await _orderRepository.Query().Where(c => request.OrderIds.Contains(c.Id)).ToListAsync();
                if (!orders.Any())
                {
                    return new BaseResponse()
                        { Message = "Không tìm thấy đơn hàng", ResponseStatus = BaseStatus.Error };
                }

                foreach (var order in orders)
                {
                    if (!OrderValidator.CanTransition(order.Status, request.Status))
                    {
                        return new BaseResponse
                        {
                            Message = OrderValidator.GetErrorMessage(order.Status, request.Status),
                            ResponseStatus = BaseStatus.Error
                        };
                    }
                }

                var listOrderDetails = await _orderDetailRepository.Query()
                    .Where(c => request.OrderIds.Contains(c.OrderId)).ToListAsync();
                var groupedByProduct = listOrderDetails
                    .GroupBy(od => od.ProductDetailId)
                    .Select(g => new
                    {
                        ProductDetailId = g.Key,
                        TotalQuantity = g.Sum(od => od.Quantity)
                    }).ToList();

                var productDetailMap = await _productsDetailRepository.Query()
                    .Where(p => groupedByProduct.Select(g => g.ProductDetailId).Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id, p => p.Sku);
                var insufficientStocks = new Dictionary<string, int>();

                if (request.Status == EOrderStatus.Confirmed)
                {
                    var inventoryErrors = await OrderValidator.ValidateInventoryAsync(
                        listOrderDetails, // danh sách các OrderDetail chuẩn bị insert
                        async productDetailId => await _inventoryRepository.Query()
                            .Where(x => x.ProductDetailId == productDetailId)
                            .ToListAsync(),
                        async productDetailId => await _productsDetailRepository.Query()
                                .Where(x => x.Id == productDetailId)
                                .Select(x => x.Sku)
                                .FirstOrDefaultAsync() ?? $"ProductDetailId={productDetailId}"
                    );

                    if (inventoryErrors.Any())
                    {
                        var message = "Không đủ tồn kho cho các sản phẩm: " +
                                      string.Join(", ", inventoryErrors.Select(kvp => $"{kvp.Key} thiếu {kvp.Value}"));
                        return new BaseResponse()
                        {
                            Message = message,
                            ResponseStatus = BaseStatus.Error,
                        };
                    }
                }
                else if (request.Status == EOrderStatus.Canceled)
                {
                    var orderDetailIds = listOrderDetails.Select(x => x.Id).ToList();

                    var orderDetailBatchMappings = await _orderDetailInventoryBatchRepository.Query()
                        .Where(x => orderDetailIds.Contains(x.OrderDetailId))
                        .ToListAsync();

                    foreach (var mapping in orderDetailBatchMappings)
                    {
                        var batch = await _inventoryRepository.GetByIdAsync(mapping.InventoryBatchId);
                        if (batch == null) continue;

                        batch.Quantity += mapping.Quantity;


                        await _inventoryRepository.UpdateAsync(batch);

                        await _inventoryTransactionRepository.AddAsync(new InventoryTransaction
                        {
                            InventoryBatchId = batch.Id,
                            Quantity = mapping.Quantity,
                            CreationTime = DateTime.Now,
                            Type = EInventoryTranctionType.Import,
                        });
                    }

                    await _inventoryRepository.SaveChangesAsync();
                    await _inventoryTransactionRepository.SaveChangesAsync();
                }

                orders.ForEach(o =>
                {
                    o.Status = request.Status;
                    o.LastModifiedTime = DateTime.Now;
                });
                await _orderRepository.UpdateRangeAsync(orders);
                await _unitOfWork.CommitAsync();
                return new BaseResponse()
                    { Message = "", ResponseStatus = BaseStatus.Success };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new BaseResponse
                {
                    Message = ex.Message,
                    ResponseStatus = BaseStatus.Error
                };
            }
        }

        public async Task<BaseResponse> CreateOrderAsync(CreateOrderDTO request)
        {
            var cartDetailItems = await _cartDetailRepository.Query()
                .Where(c => request.CartItems.Contains(c.Id))
                .ToListAsync();

            if (!cartDetailItems.Any())
            {
                return new BaseResponse
                {
                    Message = "Không tìm thấy sản phẩm trong giỏ hàng",
                    ResponseStatus = BaseStatus.Error
                };
            }

            // 1. Tạo Order
            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                EmployeeId = request.EmployeeId,
                VoucherId = request.VoucherId,
                DeliveryAddressId = request.DeliveryAddressId,
                DeliveryDate = request.DeliveryDate,
                ReceiveDate = request.ReceiveDate,
                ExpectReceiveDate = request.ExpectReceiveDate,
                Note = request.Note,
                Reason = request.Reason,
                PaymentMethod = request.PaymentMethod,
                Type = request.Type,
                Status = EOrderStatus.Pending, // Mặc định luôn là Pending
                CreationTime = DateTime.Now
            };
            await _orderRepository.AddAsync(order);

            foreach (var cart in cartDetailItems)
            {
                var orderDetail = new OrderDetail
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductDetailId = cart.ProductDetailId,
                    Quantity = cart.Quantity,
                    Price = cart.Price 
                };

                await _orderDetailRepository.AddAsync(orderDetail);
            }

            return new BaseResponse();

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

        // public async Task<CreateOrUpdateOrderResponse> GetOrderByIdAsync(Guid id)
        // {
        //     CreateOrUpdateOrderResponse response = new CreateOrUpdateOrderResponse();
        //     var result = await _orderRepository.GetOrderByIdAsync(id);
        //     if (result == null)
        //     {
        //         response.Message = "Không tìm thấy đơn hàng này";
        //         response.ResponseStatus = BaseStatus.Error;
        //         return response;
        //     }
        //     response = _mapper.Map<CreateOrUpdateOrderResponse>(result);
        //     response.Message = "";
        //     response.ResponseStatus = BaseStatus.Success;
        //     return response;
        // }
        //
        // public async Task<CreateOrUpdateOrderResponse> UpdateOrderAsync(CreateOrUpdateOrderDTO order)
        // {
        //     CreateOrUpdateOrderResponse response = new CreateOrUpdateOrderResponse();
        //     Order itemOrder = new Order();
        //
        //     var check = await _orderRepository.GetOrderByIdAsync(Guid.Parse(order.Id.ToString()));
        //     if (check == null)
        //     {
        //         response.Message = "Không tìm thấy Order";
        //         response.ResponseStatus = BaseStatus.Error;
        //         return response;
        //         //throw new Exception("Không tìm thấy Order");
        //     }
        //     itemOrder = _mapper.Map(order, check);
        //     var result = await _orderRepository.UpdateOrderAsync(itemOrder);
        //
        //     response = _mapper.Map<CreateOrUpdateOrderResponse>(result);
        //
        //     response.Message = "";
        //     response.ResponseStatus = BaseStatus.Success;
        //     return response;
        // }
    }
}