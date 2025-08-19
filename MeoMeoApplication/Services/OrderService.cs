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
        private readonly IIventoryBatchReposiory _inventoryRepository;
        private readonly ICartDetaillRepository _cartDetailRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IOrderHistoryRepository _orderHistoryRepository;
        private readonly IProductsDetailRepository _productsDetailRepository;
        private readonly IInventoryTranSactionRepository _inventoryTransactionRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailInventoryBatchRepository _orderDetailInventoryBatchRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IIventoryBatchReposiory inventoryRepository, IOrderRepository orderRepository,
            IMapper mapper, IOrderDetailRepository orderDetailRepository,
            IInventoryTranSactionRepository inventoryTransactionRepository,
            IProductsDetailRepository productsDetailRepository, IUnitOfWork unitOfWork,
            IOrderDetailInventoryBatchRepository orderDetailInventoryBatchRepository, ICartDetaillRepository cartDetailRepository, ICartRepository cartRepository, IOrderHistoryRepository orderHistoryRepository, IUserRepository userRepository)
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
            _orderHistoryRepository = orderHistoryRepository;
            _userRepository = userRepository;
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

                if (!string.IsNullOrEmpty(request.CustomerNameFilter))
                {
                    query = query.Where(o => EF.Functions.Like(o.CustomerName, $"%{request.CustomerNameFilter}%"));
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
                if (request.ShippingDateStartFilter.HasValue)
                {
                    query = query.Where(o => o.DeliveryDate >= request.ShippingDateStartFilter.Value);
                }

                if (request.ShippingDateEndFilter.HasValue)
                {
                    query = query.Where(o => o.DeliveryDate <= request.ShippingDateEndFilter.Value);
                }

                if (request.PaymentMethodFilter.HasValue)
                {
                    query = query.Where(o => o.PaymentMethod == request.PaymentMethodFilter.Value);
                }

                if (request.OrderStatusFilter.HasValue)
                {
                    query = query.Where(o => o.Status == request.OrderStatusFilter.Value);
                }

                query = query.OrderByDescending(o => o.LastModifiedTime ?? o.CreationTime);
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
                var currentStatus= orders.First().Status;
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
                var orderDetailIds = listOrderDetails.Select(x => x.Id).ToList();
                if (request.Status == EOrderStatus.Confirmed)
                {
                    var inventoryErrors = await OrderValidator.ValidateInventoryAsync(
                        listOrderDetails,
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
                    foreach (var orderDetail in listOrderDetails)
                    {
                        var requiredQuantity = orderDetail.Quantity;
                        var availableBatches = await _inventoryRepository.Query()
                            .Where(x => x.ProductDetailId == orderDetail.ProductDetailId && x.Quantity > 0 && x.Status == EInventoryBatchStatus.Aprroved)
                            .OrderBy(x => x.CreationTime) 
                            .ToListAsync();
                        foreach (var batch in availableBatches)
                        {
                            if (requiredQuantity <= 0) break;

                            var deductedQuantity = Math.Min(batch.Quantity, requiredQuantity);
                            batch.Quantity -= deductedQuantity;
                            requiredQuantity -= deductedQuantity;

                            await _inventoryRepository.UpdateAsync(batch);
                            await _inventoryTransactionRepository.AddAsync(new InventoryTransaction
                            {
                                InventoryBatchId = batch.Id,
                                Quantity = deductedQuantity,
                                CreationTime = DateTime.Now,
                                Type = EInventoryTranctionType.Export,
                            });
                            await _orderDetailInventoryBatchRepository.AddAsync(new OrderDetailInventoryBatch
                            {
                                OrderDetailId = orderDetail.Id,
                                InventoryBatchId = batch.Id,
                                Quantity = deductedQuantity,
                            });
                        }
                    }
                }
                else if ((currentStatus == EOrderStatus.Confirmed || currentStatus== EOrderStatus.InTransit || currentStatus == EOrderStatus.Completed) && request.Status == EOrderStatus.Canceled)
                {
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
                }
                orders.ForEach(o =>
                {
                    o.Status = request.Status;
                    o.LastModifiedTime = DateTime.Now;
                    o.Reason = request.Reason;

                });
                var lstHistory = orders.Select(o =>
                {
                    var content = $"""
                                   <p><strong>Trạng thái đơn hàng:</strong> Từ 
                                   <span class="status-old">{MapStatus(currentStatus)}</span> 
                                   => 
                                   <span class="status-new">{MapStatus(request.Status)}</span></p>
                                   """;
                    if (request.Status == EOrderStatus.Canceled && !string.IsNullOrWhiteSpace(request.Reason))
                    {
                        content += $"""<p><strong>Lý do từ chối:</strong> <span class="status-new">{request.Reason}</span></p>""";
                    }
                    return new OrderHistory
                    {
                        OrderId = o.Id,
                        Type = EHistoryType.Update,
                        CreationTime = DateTime.Now,
                        CreatedBy = Guid.Empty,
                        Content = content
                    };
                }).ToList();
                await _orderHistoryRepository.AddRangeAsync(lstHistory);
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

        public async Task<GetListOrderHistoryResponseDTO> GetListOrderHistoryAsync(Guid orderId)
        {
            try
            {
                GetListOrderHistoryResponseDTO response = new GetListOrderHistoryResponseDTO();
                var orderHistory = await (from oh in _orderHistoryRepository.Query()
                    join u in _userRepository.Query() on oh.CreatedBy equals u.Id into userJoin
                    from u in userJoin.DefaultIfEmpty()
                    where oh.OrderId == orderId
                    select new OrderHistoryDTO
                    {
                      Id = oh.Id,
                      CreationTime = oh.CreationTime,
                      Content = oh.Content,
                      Type = oh.Type,
                      OrderId = oh.OrderId,
                      Actor = String.IsNullOrEmpty(u.UserName) ? "Admin hệ thống":u.UserName
                    }).OrderByDescending(oh => oh.CreationTime).ToListAsync();
                response.Items= orderHistory;
                return response;
            }
            catch (Exception ex)
            {
                return new GetListOrderHistoryResponseDTO()
                {
                    Items = new List<OrderHistoryDTO>(),
                    ResponseStatus = BaseStatus.Error,
                    Message = ex.Message
                };
               
            }
        }

        private static string MapStatus(EOrderStatus status)
        {
            return status switch
            {
                EOrderStatus.Pending => "Chờ xác nhận",
                EOrderStatus.Confirmed => "Đã xác nhận",
                EOrderStatus.InTransit => "Đang giao",
                EOrderStatus.Canceled => "Đã huỷ",
                EOrderStatus.Completed => "Hoàn thành",
                _ => status.ToString()
            };
        }
        public async Task<CreateOrderResultDTO> CreateOrderAsync(CreateOrderDTO request)
        {
            // Checkout từ giỏ hàng sang đơn hàng: kiểm tra dữ liệu, check tồn kho, tạo đơn + chi tiết, xoá dòng giỏ đã mua, cập nhật tổng tiền giỏ
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // 1) Lấy các dòng giỏ hàng theo danh sách CartItems
                var cartDetailItems = await _cartDetailRepository.Query()
                    .Where(c => request.CartItems.Contains(c.Id))
                    .ToListAsync();

                if (!cartDetailItems.Any())
                {
                    return new CreateOrderResultDTO
                    {
                        Message = "Không tìm thấy sản phẩm trong giỏ hàng",
                        ResponseStatus = BaseStatus.Error
                    };
                }

                // 2) Xác thực các dòng giỏ thuộc về giỏ của đúng Customer
                var cartOfCustomer = await _cartRepository.Query()
                    .Where(c => c.CustomerId == request.CustomerId)
                    .Select(c => new { c.Id })
                    .FirstOrDefaultAsync();
                if (cartOfCustomer == null)
                {
                    return new CreateOrderResultDTO { Message = "Không tìm thấy giỏ hàng của khách", ResponseStatus = BaseStatus.Error };
                }
                if (cartDetailItems.Any(cd => cd.CartId != cartOfCustomer.Id))
                {
                    return new CreateOrderResultDTO { Message = "Dòng giỏ hàng không hợp lệ", ResponseStatus = BaseStatus.Error };
                }

                // 3) Kiểm tra tồn kho theo từng biến thể (gom nhóm theo ProductDetailId)
                var groupedByVariant = cartDetailItems
                    .GroupBy(x => x.ProductDetailId)
                    .Select(g => new { ProductDetailId = g.Key, TotalQty = g.Sum(x => x.Quantity) })
                    .ToList();

                foreach (var g in groupedByVariant)
                {
                    // Chỉ tính các lô đã duyệt
                    var available = await _inventoryRepository.Query()
                        .Where(x => x.ProductDetailId == g.ProductDetailId && x.Status == EInventoryBatchStatus.Aprroved)
                        .SumAsync(x => (int?)x.Quantity) ?? 0;
                    if (g.TotalQty > available)
                    {
                        return new CreateOrderResultDTO
                        {
                            Message = $"Không đủ tồn kho cho biến thể {g.ProductDetailId}. Cần {g.TotalQty}, còn {available}",
                            ResponseStatus = BaseStatus.Error
                        };
                    }
                }

                // 4) Tạo đơn hàng (Pending). Tổng tiền sẽ tính theo các dòng giỏ (giá sau chiết khấu)
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
                    Status = EOrderStatus.Pending,
                    CreationTime = DateTime.Now
                };
                await _orderRepository.AddAsync(order);

                decimal totalPrice = 0m;
                // 5) Tạo các dòng chi tiết đơn từ các dòng giỏ
                foreach (var cart in cartDetailItems)
                {
                    // Giá cuối cùng dựa trên discount %/số tiền trong giỏ
                    float discount = cart.Discount;
                    var linePriceAfterDiscount = discount > 100
                        ? cart.Price - discount
                        : cart.Price * (1 - discount / 100f);
                    totalPrice += (decimal)(linePriceAfterDiscount * cart.Quantity);

                    var orderDetail = new OrderDetail
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductDetailId = cart.ProductDetailId,
                        Quantity = cart.Quantity,
                        // Lưu giá gốc và %/tiền giảm để tiện theo dõi, tính toán
                        Price = cart.Price,
                        OriginalPrice = cart.Price,
                        Discount = cart.Discount,
                        PromotionDetailId = cart.PromotionDetailId == Guid.Empty ? (Guid?)null : cart.PromotionDetailId
                    };
                    await _orderDetailRepository.AddAsync(orderDetail);
                }
                // Cập nhật tổng tiền đơn
                order.TotalPrice = totalPrice;
                await _orderRepository.UpdateAsync(order);

                // 6) Xoá các dòng giỏ đã checkout và cập nhật lại tổng tiền giỏ còn lại
                foreach (var cd in cartDetailItems)
                {
                    await _cartDetailRepository.Delete(cd.Id);
                }
                var newCartTotal = await _cartDetailRepository.Query()
                    .Where(d => d.CartId == cartOfCustomer.Id)
                    .SumAsync(i => (decimal)((i.Price - (i.Price * i.Discount / 100f)) * i.Quantity));
                var cartEntity = await _cartRepository.GetCartById(cartOfCustomer.Id);
                if (cartEntity != null)
                {
                    cartEntity.TotalPrice = newCartTotal;
                    await _cartRepository.Update(cartEntity);
                }

                await _unitOfWork.CommitAsync();
                return new CreateOrderResultDTO { ResponseStatus = BaseStatus.Success, Message = "Tạo đơn hàng thành công", OrderId = order.Id, Amount = totalPrice };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new CreateOrderResultDTO
                {
                    Message = ex.Message,
                    ResponseStatus = BaseStatus.Error
                };
            }
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