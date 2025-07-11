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
        private readonly IProductsDetailRepository _productsDetailRepository;
        private readonly IInventoryTranSactionRepository _inventoryTransactionRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IIventoryBtachReposiory inventoryRepository, IOrderRepository orderRepository, IMapper mapper, IOrderDetailRepository orderDetailRepository, IInventoryTranSactionRepository inventoryTransactionRepository, IProductsDetailRepository productsDetailRepository, IUnitOfWork unitOfWork)
        {
            _inventoryRepository = inventoryRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
            _orderDetailRepository = orderDetailRepository;
            _inventoryTransactionRepository = inventoryTransactionRepository;
            _productsDetailRepository = productsDetailRepository;
            _unitOfWork = unitOfWork;
        }
        

        public async Task<PagingExtensions.PagedResult<OrderDTO>> GetListOrderAsync(GetListOrderRequestDTO request)
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
                metaDataValue.Confirmed = statusCounts.FirstOrDefault(s => s.Status == EOrderStatus.Confirmed)?.Count ?? 0;
                metaDataValue.InTransit = statusCounts.FirstOrDefault(s => s.Status == EOrderStatus.InTransit)?.Count ?? 0;
                metaDataValue.Canceled = statusCounts.FirstOrDefault(s => s.Status == EOrderStatus.Canceled)?.Count ?? 0;
                metaDataValue.Completed = statusCounts.FirstOrDefault(s => s.Status == EOrderStatus.Completed)?.Count ?? 0;
                
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
                var pagedOrders = await  _orderRepository.GetPagedAsync(query, request.PageIndex, request.PageSize);

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
                metaDataValue.Message =  ex.Message;
                return new PagingExtensions.PagedResult<OrderDTO,GetListOrderResponseDTO>
                {
                    TotalRecords = 0,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    Items = new List<OrderDTO>(),
                    Metadata = metaDataValue,
                };
            }
        }

        public async Task<UpdateStatusOrderResponseDTO> UpdateStatusOrderAsync(UpdateStatusOrderRequestDTO request)
        {
            try
            {
          
                    await _unitOfWork.BeginTransactionAsync();
                    var orders = await _orderRepository.Query().Where(c => request.OrderIds.Contains(c.Id)).ToListAsync();
                    if (!orders.Any())
                    {
                        return new UpdateStatusOrderResponseDTO()
                            { Message = "Không tìm thấy đơn hàng", ResponseStatus = BaseStatus.Error };
                    }

                    foreach (var order in orders)
                    {
                        if (!OrderValidator.CanTransition(order.Status, request.Status))
                        {
                            return new UpdateStatusOrderResponseDTO
                            {
                                Message = OrderValidator.GetErrorMessage(order.Status, request.Status),
                                ResponseStatus = BaseStatus.Error
                            };
                        }
                    }
                    var listOrderDetails = await _orderDetailRepository.Query().Where(c => request.OrderIds.Contains(c.OrderId)).ToListAsync();
                    var groupedByProduct = listOrderDetails
                        .GroupBy(od => od.ProductDetailId)
                        .Select(g => new {
                            ProductDetailId = g.Key,
                            TotalQuantity = g.Sum(od => od.Quantity)
                        }).ToList();
                    
                    var productDetailMap = await _productsDetailRepository.Query()
                        .Where(p => groupedByProduct.Select(g => g.ProductDetailId).Contains(p.Id))
                        .ToDictionaryAsync(p => p.Id, p => p.Sku);
                    var insufficientStocks = new Dictionary<string, int>();

                    if (request.Status == EOrderStatus.Confirmed)
                    {
                        foreach (var item in groupedByProduct)
                        {
                            int quantityToDeduct = item.TotalQuantity;

                            var inventoryBatches = await _inventoryRepository.Query()
                                .Where(b => b.ProductDetailId == item.ProductDetailId && b.Quantity > 0 &&b.Status==EInventoryBatchStatus.Aprroved)
                                .OrderBy(b => b.CreationTime)
                                .ToListAsync();

                            foreach (var batch in inventoryBatches)
                            {
                                if (quantityToDeduct == 0) break;

                                int deduct = Math.Min(batch.Quantity, quantityToDeduct);
                                batch.Quantity -= deduct;
                                quantityToDeduct -= deduct;

                                await _inventoryRepository.UpdateAsync(batch);

                                await _inventoryTransactionRepository.AddAsync(new InventoryTransaction
                                {
                                    InventoryBatchId = batch.Id,
                                    Quantity = deduct,
                                    CreationTime = DateTime.Now,
                                    Type = EInventoryTranctionType.Export
                                });
                            }

                            if (quantityToDeduct > 0)
                            {
                                var sku = productDetailMap.ContainsKey(item.ProductDetailId)
                                    ? productDetailMap[item.ProductDetailId]
                                    : $"ProductDetailId={item.ProductDetailId}";

                                if (insufficientStocks.ContainsKey(sku))
                                    insufficientStocks[sku] += quantityToDeduct;
                                else
                                    insufficientStocks[sku] = quantityToDeduct;
                            }
                        }
                        if (insufficientStocks.Any())
                        {
                            var message = "Không đủ tồn kho cho các sản phẩm: " +
                                          string.Join(", ", insufficientStocks.Select(kvp => $"{kvp.Key} thiếu {kvp.Value}"));
                            return new UpdateStatusOrderResponseDTO
                            {
                                Message = message,
                                ResponseStatus = BaseStatus.Error
                            };
                        }
       
                    }
                    else if (request.Status == EOrderStatus.Canceled)
                    {
                        
                    }
                    orders.ForEach(o =>
                    {
                        o.Status = request.Status;
                        o.LastModifiedTime = DateTime.Now;
                    });
                    await _orderRepository.UpdateRangeAsync(orders);
                    await _unitOfWork.CommitAsync();
                    return new UpdateStatusOrderResponseDTO()
                        { Message = "", ResponseStatus = BaseStatus.Success };
                    
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new UpdateStatusOrderResponseDTO
                {
                    Message = ex.Message,
                    ResponseStatus = BaseStatus.Error
                };
            }
        }

        public Task<BaseResponse> CreateOrderAsync(CreateOrderDTO request)
        {
            throw new NotImplementedException();
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