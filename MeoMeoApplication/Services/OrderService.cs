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
using System.Security.Cryptography;
using Microsoft.Identity.Client;

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
        private readonly IDeliveryAddressRepository _deliveryAddressRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPromotionDetailRepository _promotionDetailRepository;
        private readonly IVoucherRepository _voucherRepository;

        public OrderService(IIventoryBatchReposiory inventoryRepository, IOrderRepository orderRepository,
            IMapper mapper, IOrderDetailRepository orderDetailRepository,
            IInventoryTranSactionRepository inventoryTransactionRepository,
            IProductsDetailRepository productsDetailRepository, IUnitOfWork unitOfWork,
            IOrderDetailInventoryBatchRepository orderDetailInventoryBatchRepository,
            ICartDetaillRepository cartDetailRepository, ICartRepository cartRepository,
            IOrderHistoryRepository orderHistoryRepository, IUserRepository userRepository,
            IDeliveryAddressRepository deliveryAddressRepository, IEmployeeRepository employeeRepository,
            ICustomerRepository customerRepository, IProductRepository productRepository,
            IPromotionDetailRepository promotionDetailRepository, IVoucherRepository voucherRepository)
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
            this._userRepository = userRepository;
            _userRepository = userRepository;
            _deliveryAddressRepository = deliveryAddressRepository;
            _employeeRepository = employeeRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _promotionDetailRepository = promotionDetailRepository;
            _voucherRepository = voucherRepository;
        }


        public async Task<PagingExtensions.PagedResult<OrderDTO, GetListOrderResponseDTO>> GetListOrderAsync(
            GetListOrderRequestDTO request)
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
                    .Include(c => c.ProductDetail)
                        .ThenInclude(pd => pd.Size)
                    .Include(c => c.ProductDetail)
                        .ThenInclude(pd => pd.Colour)
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

        public async Task<PagingExtensions.PagedResult<OrderDTO, GetListOrderResponseDTO>> GetListOrderByCustomerAsync(
            GetListOrderRequestDTO request, Guid customerId)
        {
            var metaDataValue = new GetListOrderResponseDTO();
            try
            {
                var query = _orderRepository.Query().Where(o => o.CustomerId == customerId);

                var statusCounts = await _orderRepository.Query()
                    .Where(o => o.CustomerId == customerId)
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
                    .Include(c => c.ProductDetail)
                        .ThenInclude(pd => pd.Size)
                    .Include(c => c.ProductDetail)
                        .ThenInclude(pd => pd.Colour)
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

        public async Task<OrderDTO?> GetOrderByIdAsync(Guid orderId)
        {
            try
            {
                var order = await _orderRepository.Query()
                    .Include(o => o.OrderDetails)
                    .Include(o => o.Customers)
                    .Include(o => o.Employee)
                    .Include(o => o.Voucher)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                    return null;

                return _mapper.Map<OrderDTO>(order);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting order by id: {ex.Message}");
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

                var currentStatus = orders.First().Status;
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
                            .Where(x => x.ProductDetailId == orderDetail.ProductDetailId && x.Quantity > 0 &&
                                        x.Status == EInventoryBatchStatus.Approved)
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
                else if ((currentStatus == EOrderStatus.Confirmed || currentStatus == EOrderStatus.InTransit ||
                          currentStatus == EOrderStatus.Completed) && request.Status == EOrderStatus.Canceled)
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
                        content +=
                            $"""<p><strong>Lý do từ chối:</strong> <span class="status-new">{request.Reason}</span></p>""";
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
                                              Actor = String.IsNullOrEmpty(u.UserName) ? "Admin hệ thống" : u.UserName
                                          }).OrderByDescending(oh => oh.CreationTime).ToListAsync();
                response.Items = orderHistory;
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

        public async Task<CreateOrderResultDTO> CreateOrderAsync(Guid customerId, Guid userId, CreateOrderDTO request)
        {
            try
            {
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
                    .Where(c => c.CustomerId == customerId)
                    .Select(c => new { c.Id })
                    .FirstOrDefaultAsync();
                if (cartOfCustomer == null)
                {
                    return new CreateOrderResultDTO
                    { Message = "Không tìm thấy giỏ hàng của khách", ResponseStatus = BaseStatus.Error };
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
                        .Where(x => x.ProductDetailId == g.ProductDetailId &&
                                    x.Status == EInventoryBatchStatus.Approved)
                        .SumAsync(x => (int?)x.Quantity) ?? 0;
                    if (g.TotalQty > available)
                    {
                        return new CreateOrderResultDTO
                        {
                            Message =
                                $"Không đủ tồn kho cho biến thể {g.ProductDetailId}. Cần {g.TotalQty}, còn {available}",
                            ResponseStatus = BaseStatus.Error
                        };
                    }
                }

                // 4) Tạo đơn hàng (Pending). Tổng tiền sẽ tính theo các dòng giỏ (giá sau chiết khấu)
                var orderCode = await GenerateUniqueOrderCodeAsync();
                var deliInfor = await _deliveryAddressRepository.Query()
                    .FirstOrDefaultAsync(c => c.Id == request.DeliveryAddressId);
                if (deliInfor == null)
                {
                    return new CreateOrderResultDTO
                    {
                        Message = $"Không tìm thấy địa chỉ giao hàng",
                        ResponseStatus = BaseStatus.Error
                    };
                }

                // Validate voucher if provided
                Voucher? validVoucher = null;
                if (request.VoucherId.HasValue)
                {
                    validVoucher = await _voucherRepository.Query()
                        .Include(v => v.Orders)
                        .Where(v => v.Id == request.VoucherId.Value &&
                                   v.StartDate <= DateTime.Now &&
                                   v.EndDate >= DateTime.Now)
                        .FirstOrDefaultAsync();

                    if (validVoucher == null)
                    {
                        return new CreateOrderResultDTO
                        {
                            Message = "Voucher không hợp lệ hoặc đã hết hạn",
                            ResponseStatus = BaseStatus.Error
                        };
                    }

                    // Check MaxTotalUse limit
                    if (validVoucher.MaxTotalUse.HasValue)
                    {
                        var totalUsageCount = validVoucher.Orders?.Count ?? 0;
                        if (totalUsageCount >= validVoucher.MaxTotalUse.Value)
                        {
                            return new CreateOrderResultDTO
                            {
                                Message = "Voucher này đã hết lượt sử dụng",
                                ResponseStatus = BaseStatus.Error
                            };
                        }
                    }

                    // Check MaxTotalUsePerCustomer limit
                    if (validVoucher.MaxTotalUsePerCustomer.HasValue)
                    {
                        var customerUsageCount = validVoucher.Orders?.Count(o => o.CustomerId == customerId) ?? 0;
                        if (customerUsageCount >= validVoucher.MaxTotalUsePerCustomer.Value)
                        {
                            return new CreateOrderResultDTO
                            {
                                Message = "Bạn đã sử dụng hết lượt áp dụng voucher này",
                                ResponseStatus = BaseStatus.Error
                            };
                        }
                    }
                }

                var customerUser = await _userRepository.Query().FirstOrDefaultAsync(c => c.Id == userId);
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    Code = orderCode,
                    CustomerId = customerId,
                    DeliveryAddress = deliInfor.FullAddress,
                    CustomerName = deliInfor.Name,
                    CustomerEmail = customerUser.Email ?? "",
                    CustomerPhoneNumber = deliInfor.PhoneNumber,
                    VoucherId = validVoucher?.Id,
                    DeliveryAddressId = request.DeliveryAddressId,
                    Note = request.Note,
                    PaymentMethod = request.PaymentMethod,
                    Type = EOrderType.Online,
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
                    // Lưu giá gốc và %/tiền giảm để tiện theo dõi, tính toán
                    var productDetailInfor = await _productsDetailRepository.Query()
                        .FirstOrDefaultAsync(c => c.Id == cart.ProductDetailId);
                    var productInfor = await _productRepository.Query()
                        .FirstOrDefaultAsync(c => c.Id == productDetailInfor.ProductId);
                    var orderDetail = new OrderDetail
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductName = productInfor.Name,
                        Sku = productDetailInfor.Sku,
                        ProductDetailId = cart.ProductDetailId,
                        Image = productInfor.Thumbnail,
                        Quantity = cart.Quantity,
                        Price = cart.Price,
                        OriginalPrice = cart.Price,
                        Discount = cart.Discount,
                        PromotionDetailId = cart.PromotionDetailId == Guid.Empty ? (Guid?)null : cart.PromotionDetailId
                    };
                    await _orderDetailRepository.AddAsync(orderDetail);
                }

                // Apply voucher discount if available
                decimal voucherDiscountAmount = 0m;
                if (validVoucher != null)
                {
                    // Check minimum order requirement
                    if (totalPrice >= validVoucher.MinOrder)
                    {
                        if (validVoucher.Type == EVoucherType.byPercentage)
                        {
                            // Percentage discount
                            voucherDiscountAmount = totalPrice * (decimal)validVoucher.Discount / 100m;
                            // Apply max discount limit if set
                            if (validVoucher.MaxDiscount > 0 && voucherDiscountAmount > (decimal)validVoucher.MaxDiscount)
                            {
                                voucherDiscountAmount = (decimal)validVoucher.MaxDiscount;
                            }
                        }
                        else
                        {
                            // Fixed amount discount
                            voucherDiscountAmount = (decimal)validVoucher.Discount;
                            // Ensure discount doesn't exceed order total
                            if (voucherDiscountAmount > totalPrice)
                            {
                                voucherDiscountAmount = totalPrice;
                            }
                        }
                    }
                    else
                    {
                        return new CreateOrderResultDTO
                        {
                            Message = $"Đơn hàng cần tối thiểu {validVoucher.MinOrder:N0} đ để áp dụng voucher này",
                            ResponseStatus = BaseStatus.Error
                        };
                    }
                }

                // Update order with discount information
                order.DiscountPrice = voucherDiscountAmount > 0 ? voucherDiscountAmount : null;
                order.TotalPrice = totalPrice - voucherDiscountAmount + (order.ShippingFee ?? 0);
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
                return new CreateOrderResultDTO
                {
                    ResponseStatus = BaseStatus.Success,
                    Message = "Tạo đơn hàng thành công. Mã đơn hàng của bạn là " + orderCode,
                    OrderId = order.Id,
                    Code = order.Code,
                    DeliveryAddress = order.DeliveryAddress,
                    Note = order.Note ?? "",
                };
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

        public async Task<CreatePosOrderResultDTO> CreatePosOrderAsync(CreatePosOrderDTO request)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Validate items
                if (request.Items == null || !request.Items.Any())
                {
                    return new CreatePosOrderResultDTO
                    {
                        Message = "Không có sản phẩm trong đơn",
                        ResponseStatus = BaseStatus.Error
                    };
                }
                // Validate inventory
                var groupedByVariant = request.Items
                    .GroupBy(x => x.ProductDetailId)
                    .Select(g => new { ProductDetailId = g.Key, TotalQty = g.Sum(x => x.Quantity) })
                    .ToList();

                foreach (var g in groupedByVariant)
                {
                    var available = await _inventoryRepository.Query()
                        .Where(x => x.ProductDetailId == g.ProductDetailId && x.Status == EInventoryBatchStatus.Approved)
                        .SumAsync(x => (int?)x.Quantity) ?? 0;
                    if (g.TotalQty > available)
                    {
                        return new CreatePosOrderResultDTO
                        {
                            Message = $"Không đủ tồn kho cho biến thể {g.ProductDetailId}. Cần {g.TotalQty}, còn {available}",
                            ResponseStatus = BaseStatus.Error
                        };
                    }
                }
                var customer = await _customerRepository.Query().FirstOrDefaultAsync(c => c.Id == request.CustomerId);

                Voucher? validVoucher = null;
                if (!string.IsNullOrEmpty(request.DiscountCode))
                {
                    validVoucher = await _voucherRepository.Query()
                        .Include(v => v.Orders)
                        .Where(v => v.Code == request.DiscountCode &&
                                   v.StartDate <= DateTime.Now &&
                                   v.EndDate >= DateTime.Now)
                        .FirstOrDefaultAsync();

                    if (validVoucher == null)
                    {
                        return new CreatePosOrderResultDTO
                        {
                            Message = "Mã voucher không hợp lệ hoặc đã hết hạn",
                            ResponseStatus = BaseStatus.Error
                        };
                    }

                    // Check MaxTotalUse limit
                    if (validVoucher.MaxTotalUse.HasValue)
                    {
                        var totalUsageCount = validVoucher.Orders?.Count ?? 0;
                        if (totalUsageCount >= validVoucher.MaxTotalUse.Value)
                        {
                            return new CreatePosOrderResultDTO
                            {
                                Message = "Voucher này đã hết lượt sử dụng",
                                ResponseStatus = BaseStatus.Error
                            };
                        }
                    }

                    // Check MaxTotalUsePerCustomer limit
                    if (validVoucher.MaxTotalUsePerCustomer.HasValue)
                    {
                        var customerUsageCount = validVoucher.Orders?.Count(o => o.CustomerId == request.CustomerId) ?? 0;
                        if (customerUsageCount >= validVoucher.MaxTotalUsePerCustomer.Value)
                        {
                            return new CreatePosOrderResultDTO
                            {
                                Message = "Bạn đã sử dụng hết lượt áp dụng voucher này",
                                ResponseStatus = BaseStatus.Error
                            };
                        }
                    }
                }

                // Create order
                var orderCode = await GenerateUniqueOrderCodeAsync();
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    Code = orderCode,
                    CustomerId = customer.Id,
                    CustomerName = customer?.Name ?? string.Empty,
                    CustomerPhoneNumber = customer?.PhoneNumber ?? string.Empty,
                    VoucherId = validVoucher?.Id,
                    DeliveryAddressId = null,
                    Note = request.Note,
                    PaymentMethod = request.PaymentMethod,
                    Type = request.Type,
                    Status = EOrderStatus.Completed,
                    ShippingFee = request.ShippingFee,
                    CreationTime = DateTime.Now,
                };
                if (request.Type == EOrderType.Online && request.Delivery != null)
                {
                    order.DeliveryAddress = request.Delivery.ConsigneeAddress;
                }
                await _orderRepository.AddAsync(order);

                decimal totalPrice = 0m;
                var now = DateTime.Now;

                foreach (var item in request.Items)
                {
                    var productDetail = await _productsDetailRepository.Query().FirstOrDefaultAsync(c => c.Id == item.ProductDetailId);
                    if (productDetail == null) continue;
                    var product = await _productRepository.Query().FirstOrDefaultAsync(c => c.Id == productDetail.ProductId);

                    // Check for active promotion
                    var activePromotion = await _promotionDetailRepository.Query()
                        .Include(pd => pd.Promotion)
                        .Where(pd => pd.ProductDetailId == item.ProductDetailId &&
                                    pd.Promotion.StartDate <= now &&
                                    pd.Promotion.EndDate >= now)
                        .OrderByDescending(pd => pd.Discount)
                        .FirstOrDefaultAsync();

                    // Calculate discount and final price
                    float discount = activePromotion?.Discount ?? 0f;
                    var originalPrice = item.UnitPrice;
                    var discountedPrice = discount > 0 ? originalPrice * (1 - (decimal)discount / 100m) : originalPrice;
                    var lineTotal = discountedPrice * item.Quantity;
                    totalPrice += lineTotal;

                    var orderDetail = new OrderDetail
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductName = product?.Name ?? string.Empty,
                        Sku = productDetail.Sku,
                        ProductDetailId = item.ProductDetailId,
                        Image = product?.Thumbnail ?? string.Empty,
                        Quantity = item.Quantity,
                        Price = (float)discountedPrice,
                        OriginalPrice = (float)originalPrice,
                        Discount = discount,
                        PromotionDetailId = activePromotion?.Id
                    };
                    await _orderDetailRepository.AddAsync(orderDetail);
                }

                // Apply voucher discount if available
                decimal voucherDiscountAmount = 0m;
                if (validVoucher != null)
                {
                    // Check minimum order requirement
                    if (totalPrice >= validVoucher.MinOrder)
                    {
                        if (validVoucher.Type == EVoucherType.byPercentage)
                        {
                            // Percentage discount
                            voucherDiscountAmount = totalPrice * (decimal)validVoucher.Discount / 100m;
                            // Apply max discount limit if set
                            if (validVoucher.MaxDiscount > 0 && voucherDiscountAmount > (decimal)validVoucher.MaxDiscount)
                            {
                                voucherDiscountAmount = (decimal)validVoucher.MaxDiscount;
                            }
                        }
                        else
                        {
                            // Fixed amount discount
                            voucherDiscountAmount = (decimal)validVoucher.Discount;
                            // Ensure discount doesn't exceed order total
                            if (voucherDiscountAmount > totalPrice)
                            {
                                voucherDiscountAmount = totalPrice;
                            }
                        }
                    }
                    else
                    {
                        return new CreatePosOrderResultDTO
                        {
                            Message = $"Đơn hàng cần tối thiểu {validVoucher.MinOrder:N0} đ để áp dụng voucher này",
                            ResponseStatus = BaseStatus.Error
                        };
                    }
                }

                // Update order with discount information
                order.DiscountPrice = voucherDiscountAmount > 0 ? voucherDiscountAmount : null;
                order.TotalPrice = totalPrice - voucherDiscountAmount + (order.ShippingFee ?? 0);
                await _orderRepository.UpdateAsync(order);

                // Deduct inventory immediately (POS confirm)
                foreach (var item in request.Items)
                {
                    var requiredQuantity = item.Quantity;
                    var availableBatches = await _inventoryRepository.Query()
                        .Where(x => x.ProductDetailId == item.ProductDetailId && x.Quantity > 0 && x.Status == EInventoryBatchStatus.Approved)
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
                    }
                }

                await _unitOfWork.CommitAsync();
                return new CreatePosOrderResultDTO
                {
                    ResponseStatus = BaseStatus.Success,
                    Message = "Tạo đơn POS thành công",
                    OrderId = order.Id,
                    Code = order.Code
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new CreatePosOrderResultDTO
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

        private async Task<string> GenerateUniqueOrderCodeAsync()
        {
            // Format: MEO-yyyymmdd-XXXXXX (Base36 uppercase), max length 20
            // Ensure uniqueness by checking existing codes; retry a few times to avoid rare collisions
            for (int attempt = 0; attempt < 5; attempt++)
            {
                var candidate = GenerateOrderCode();
                var exists = await _orderRepository.Query().AnyAsync(o => o.Code == candidate);
                if (!exists)
                {
                    return candidate;
                }
            }

            // Fallback with extra random chars if repeated collisions (extremely unlikely)
            return GenerateOrderCode(8);
        }

        private static string GenerateOrderCode(int randomChars = 6)
        {
            var date = DateTime.Now.ToString("yyyyMMdd");
            using var rng = RandomNumberGenerator.Create();
            Span<byte> bytes = stackalloc byte[6];
            rng.GetBytes(bytes);
            ulong num = BitConverter.ToUInt64(
                new byte[] { bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5], 0, 0 }, 0);
            const string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var chars = new char[randomChars];
            for (int i = 0; i < randomChars; i++)
            {
                chars[i] = alphabet[(int)(num % 36)];
                num /= 36;
            }

            Array.Reverse(chars);
            var code = $"MEO-{date}-{new string(chars)}";
            return code.Length > 20 ? code.Substring(0, 20) : code; // respect max length
        }


    }
}