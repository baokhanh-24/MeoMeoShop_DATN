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
using MeoMeo.Shared.IServices;
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
        private readonly IEmailService _emailService;
        private readonly ISizeRepository _sizeRepository;
        private readonly IColourRepository _colourRepository;
        private readonly IOrderReturnRepository _orderReturnRepository;

        public OrderService(IIventoryBatchReposiory inventoryRepository, IOrderRepository orderRepository,
            IMapper mapper, IOrderDetailRepository orderDetailRepository,
            IInventoryTranSactionRepository inventoryTransactionRepository,
            IProductsDetailRepository productsDetailRepository, IUnitOfWork unitOfWork,
            IOrderDetailInventoryBatchRepository orderDetailInventoryBatchRepository,
            ICartDetaillRepository cartDetailRepository, ICartRepository cartRepository,
            IOrderHistoryRepository orderHistoryRepository, IUserRepository userRepository,
            IDeliveryAddressRepository deliveryAddressRepository, IEmployeeRepository employeeRepository,
            ICustomerRepository customerRepository, IProductRepository productRepository,
            IPromotionDetailRepository promotionDetailRepository, IVoucherRepository voucherRepository,
            IEmailService emailService, ISizeRepository sizeRepository, IColourRepository colourRepository,
            IOrderReturnRepository orderReturnRepository)
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
            _emailService = emailService;
            _sizeRepository = sizeRepository;
            _colourRepository = colourRepository;
            _orderReturnRepository = orderReturnRepository;
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
                metaDataValue.PendingReturn =
                    statusCounts.FirstOrDefault(s => s.Status == EOrderStatus.PendingReturn)?.Count ?? 0;
                metaDataValue.Returned =
                    statusCounts.FirstOrDefault(s => s.Status == EOrderStatus.Returned)?.Count ?? 0;
                metaDataValue.RejectReturned =
                    statusCounts.FirstOrDefault(s => s.Status == EOrderStatus.RejectReturned)?.Count ?? 0;

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
                await EnrichOrderDataAsync(dtoItems);

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
                metaDataValue.PendingReturn =
                    statusCounts.FirstOrDefault(s => s.Status == EOrderStatus.PendingReturn)?.Count ?? 0;
                metaDataValue.Returned =
                    statusCounts.FirstOrDefault(s => s.Status == EOrderStatus.Returned)?.Count ?? 0;
                metaDataValue.RejectReturned =
                    statusCounts.FirstOrDefault(s => s.Status == EOrderStatus.RejectReturned)?.Count ?? 0;

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
                await EnrichOrderDataAsync(dtoItems);

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
                // Lấy Order với Include các bảng cần thiết
                var order = await _orderRepository.Query()
                    .Include(o => o.Customers)
                    .Include(o => o.Employee)
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                    return null;
                var orderDetailsWithInfo = await (from od in _orderDetailRepository.Query()
                                                  join pd in _productsDetailRepository.Query() on od.ProductDetailId equals pd.Id
                                                  join s in _sizeRepository.Query() on pd.SizeId equals s.Id
                                                  join col in _colourRepository.Query() on pd.ColourId equals col.Id
                                                  where od.OrderId == orderId
                                                  select new OrderDetailDTO
                                                  {
                                                      Id = od.Id,
                                                      OrderId = od.OrderId,
                                                      ProductDetailId = od.ProductDetailId,
                                                      PromotionDetailId = od.PromotionDetailId ?? Guid.Empty,
                                                      Sku = od.Sku ?? string.Empty,
                                                      Price = od.Price,
                                                      OriginalPrice = od.OriginalPrice,
                                                      Quantity = od.Quantity,
                                                      GrandTotal = od.Price * od.Quantity,
                                                      ProductName = od.ProductName ?? string.Empty,
                                                      Discount = od.Discount ?? 0,
                                                      Image = od.Image ?? string.Empty,
                                                      SizeName = s.Value,
                                                      ColourName = col.Name
                                                  }).ToListAsync();

                // Map Order sang OrderDTO
                var orderDto = new OrderDTO
                {
                    Id = order.Id,
                    CustomerId = order.CustomerId,
                    EmployeeId = order.EmployeeId ?? Guid.Empty,
                    VoucherId = order.VoucherId,
                    DeliveryAddressId = null, // Không có trong Order entity
                    Code = order.Code,
                    EmployeeName = order.Employee?.Name ?? string.Empty,
                    CustomerName = order.Customers?.Name ?? string.Empty,
                    EmployeePhoneNumber = order.Employee?.PhoneNumber ?? string.Empty,
                    CustomerPhoneNumber = order.Customers?.PhoneNumber ?? string.Empty,
                    CustomerCode = order.Customers?.Code ?? string.Empty,
                    EmployeeEmail = order.Employee?.User?.Email ?? string.Empty,
                    CustomerEmail = order.Customers?.User?.Email ?? string.Empty,
                    TotalPrice = order.TotalPrice,
                    DiscountPrice = order.DiscountPrice,
                    ShippingFee = order.ShippingFee,
                    PaymentMethod = order.PaymentMethod,
                    DeliveryAddress = order.DeliveryAddress,
                    DeliveryDate = order.DeliveryDate,
                    ReceiveDate = order.ReceiveDate,
                    ExpectReceiveDate = order.ExpectReceiveDate,
                    Type = order.Type,
                    CreationTime = order.CreationTime,
                    LastModifiedTime = order.LastModifiedTime,
                    Note = order.Note,
                    CancelDate = order.CancelDate,
                    Reason = order.Reason,
                    Status = order.Status,
                    OrderDetails = orderDetailsWithInfo
                };

                return orderDto;
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
                else if (request.Status == EOrderStatus.Completed)
                {
                    // Cập nhật SellNumber khi đơn hàng hoàn thành
                    var productDetailGroups = listOrderDetails
                        .GroupBy(od => od.ProductDetailId)
                        .Select(g => new { ProductDetailId = g.Key, TotalQuantity = g.Sum(od => od.Quantity) })
                        .ToList();

                    foreach (var group in productDetailGroups)
                    {
                        var productDetail = await _productsDetailRepository.GetByIdAsync(group.ProductDetailId);
                        if (productDetail != null)
                        {
                            productDetail.SellNumber = (productDetail.SellNumber ?? 0) + group.TotalQuantity;
                            await _productsDetailRepository.UpdateAsync(productDetail);
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

                    // Giảm SellNumber khi hủy đơn hàng (nếu đơn hàng đã hoàn thành trước đó)
                    if (currentStatus == EOrderStatus.Completed)
                    {
                        var productDetailGroups = listOrderDetails
                            .GroupBy(od => od.ProductDetailId)
                            .Select(g => new { ProductDetailId = g.Key, TotalQuantity = g.Sum(od => od.Quantity) })
                            .ToList();

                        foreach (var group in productDetailGroups)
                        {
                            var productDetail = await _productsDetailRepository.GetByIdAsync(group.ProductDetailId);
                            if (productDetail != null)
                            {
                                productDetail.SellNumber = Math.Max(0, (productDetail.SellNumber ?? 0) - group.TotalQuantity);
                                await _productsDetailRepository.UpdateAsync(productDetail);
                            }
                        }
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
                    // Lấy thông tin chi tiết sản phẩm để hiển thị trong thông báo lỗi
                    var productInfo = await _productsDetailRepository.Query()
                        .Include(pd => pd.Product)
                        .Include(pd => pd.Size)
                        .Include(pd => pd.Colour)
                        .Where(pd => pd.Id == g.ProductDetailId)
                        .Select(pd => new
                        {
                            ProductName = pd.Product.Name,
                            SizeValue = pd.Size.Value,
                            ColourName = pd.Colour.Name
                        })
                        .FirstOrDefaultAsync();

                    // Chỉ tính các lô đã duyệt
                    var available = await _inventoryRepository.Query()
                        .Where(x => x.ProductDetailId == g.ProductDetailId &&
                                    x.Status == EInventoryBatchStatus.Approved)
                        .SumAsync(x => (int?)x.Quantity) ?? 0;
                    if (g.TotalQty > available)
                    {
                        var productDisplayName = productInfo != null
                            ? $"\"{productInfo.ProductName}\" ({productInfo.ColourName}, Size {productInfo.SizeValue})"
                            : $"biến thể {g.ProductDetailId}";

                        return new CreateOrderResultDTO
                        {
                            Message = $"Không đủ tồn kho cho sản phẩm {productDisplayName}. Cần {g.TotalQty}, chỉ còn {available}",
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

                    // Check MaxTotalUse limit - chỉ đếm các đơn hàng đã hoàn thành
                    if (validVoucher.MaxTotalUse.HasValue)
                    {
                        var totalUsageCount = validVoucher.Orders?.Count(o => o.Status == EOrderStatus.Completed) ?? 0;
                        if (totalUsageCount >= validVoucher.MaxTotalUse.Value)
                        {
                            return new CreateOrderResultDTO
                            {
                                Message = "Voucher này đã hết lượt sử dụng",
                                ResponseStatus = BaseStatus.Error
                            };
                        }
                    }

                    // Check MaxTotalUsePerCustomer limit - chỉ đếm các đơn hàng đã hoàn thành của khách hàng này
                    if (validVoucher.MaxTotalUsePerCustomer.HasValue)
                    {
                        var customerUsageCount = validVoucher.Orders?.Count(o => o.CustomerId == customerId && o.Status == EOrderStatus.Completed) ?? 0;
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
                    Note = request.Note,
                    PaymentMethod = request.PaymentMethod,
                    Type = EOrderType.Online,
                    Status = EOrderStatus.Pending,
                    ShippingFee = request.ShippingFee,
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
                order.TotalPrice = Math.Max(0, totalPrice - voucherDiscountAmount);
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

                // Add order history for order creation
                var orderHistory = new OrderHistory
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    Type = EHistoryType.Create,
                    CreationTime = DateTime.Now,
                    CreatedBy = Guid.Empty, // System generated
                    Content = $"""
                        <p><strong>Tạo đơn hàng:</strong> Khách hàng đã tạo đơn hàng online</p>
                        """
                };
                await _orderHistoryRepository.AddAsync(orderHistory);

                await _unitOfWork.CommitAsync();

                // Gửi email xác nhận đơn hàng
                try
                {
                    await _emailService.SendOrderConfirmationEmailAsync(
                        order.CustomerEmail,
                        order.CustomerName,
                        order.Code,
                        order.TotalPrice
                    );
                }
                catch (Exception emailEx)
                {
                    // Log lỗi email nhưng không làm fail transaction
                    Console.WriteLine($"Failed to send order confirmation email: {emailEx.Message}");
                }

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

        public async Task<CreatePosOrderResultDTO> CreatePosOrderAsync(Guid employeeId, CreatePosOrderDTO request)
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
                // Validate stock for both Completed and Pending orders
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
                        // Get product name for better error message
                        var productDetail = await _productsDetailRepository.Query()
                            .Include(pd => pd.Product)
                            .FirstOrDefaultAsync(pd => pd.Id == g.ProductDetailId);

                        var productName = productDetail?.Product?.Name ?? "Sản phẩm";
                        var sku = productDetail?.Sku ?? g.ProductDetailId.ToString();

                        return new CreatePosOrderResultDTO
                        {
                            Message = $"Không đủ tồn kho cho sản phẩm '{productName}' (SKU: {sku}). Cần {g.TotalQty}, còn {available}",
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

                    // Check MaxTotalUse limit - chỉ đếm các đơn hàng đã hoàn thành
                    if (validVoucher.MaxTotalUse.HasValue)
                    {
                        var totalUsageCount = validVoucher.Orders?.Count(o => o.Status == EOrderStatus.Completed) ?? 0;
                        if (totalUsageCount >= validVoucher.MaxTotalUse.Value)
                        {
                            return new CreatePosOrderResultDTO
                            {
                                Message = "Voucher này đã hết lượt sử dụng",
                                ResponseStatus = BaseStatus.Error
                            };
                        }
                    }

                    // Check MaxTotalUsePerCustomer limit - chỉ đếm các đơn hàng đã hoàn thành của khách hàng này
                    if (validVoucher.MaxTotalUsePerCustomer.HasValue)
                    {
                        var customerUsageCount = validVoucher.Orders?.Count(o => o.CustomerId == request.CustomerId && o.Status == EOrderStatus.Completed) ?? 0;
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
                var employee = await _employeeRepository.Query().FirstOrDefaultAsync(c => c.Id == employeeId);

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
                    Note = request.Note,
                    EmployeeId = employeeId,
                    EmployeeName = employee?.Name ?? "",
                    EmployeePhoneNumber = employee?.PhoneNumber ?? "",
                    PaymentMethod = request.PaymentMethod,
                    Type = request.Type,
                    Status = request.Status,
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
                    // Sử dụng thông tin từ DTO thay vì query database
                    var lineTotal = (decimal)item.Price * item.Quantity;
                    totalPrice += lineTotal;

                    var orderDetail = new OrderDetail
                    {
                        Id = item.Id == Guid.Empty ? Guid.NewGuid() : item.Id,
                        OrderId = order.Id,
                        ProductDetailId = item.ProductDetailId,
                        PromotionDetailId = item.PromotionDetailId,
                        Sku = item.Sku,
                        Price = item.Price,
                        OriginalPrice = item.OriginalPrice,
                        Quantity = item.Quantity,
                        ProductName = item.ProductName,
                        Discount = item.Discount > 0 ? item.Discount : null,
                        Image = item.Image
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
                order.TotalPrice = Math.Max(0, totalPrice - voucherDiscountAmount);
                await _orderRepository.UpdateAsync(order);

                // Only deduct inventory if order is completed (not draft/pending)
                if (request.Status == EOrderStatus.Completed)
                {
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

                    // Cập nhật SellNumber cho đơn POS hoàn thành
                    var productDetailGroups = request.Items
                        .GroupBy(item => item.ProductDetailId)
                        .Select(g => new { ProductDetailId = g.Key, TotalQuantity = g.Sum(item => item.Quantity) })
                        .ToList();

                    foreach (var group in productDetailGroups)
                    {
                        var productDetail = await _productsDetailRepository.GetByIdAsync(group.ProductDetailId);
                        if (productDetail != null)
                        {
                            productDetail.SellNumber = (productDetail.SellNumber ?? 0) + group.TotalQuantity;
                            await _productsDetailRepository.UpdateAsync(productDetail);
                        }
                    }
                }

                // Add order history for POS order creation
                var orderHistory = new OrderHistory
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    Type = EHistoryType.Create,
                    CreationTime = DateTime.Now,
                    CreatedBy = Guid.Empty,
                    Content = $"""
                        <p><strong>Tạo đơn hàng:</strong> Nhân viên đã tạo đơn hàng tại quầy</p>
                        """
                };
                await _orderHistoryRepository.AddAsync(orderHistory);

                await _unitOfWork.CommitAsync();

                // Lấy thông tin đầy đủ để trả về với join Size và Colour
                var orderDetails = await _orderDetailRepository.Query()
                    .Where(od => od.OrderId == order.Id)
                    .Join(_productsDetailRepository.Query(),
                        od => od.ProductDetailId,
                        pd => pd.Id,
                        (od, pd) => new { OrderDetail = od, ProductDetail = pd })
                    .Join(_sizeRepository.Query(),
                        x => x.ProductDetail.SizeId,
                        s => s.Id,
                        (x, s) => new { x.OrderDetail, x.ProductDetail, Size = s })
                    .Join(_colourRepository.Query(),
                        x => x.ProductDetail.ColourId,
                        c => c.Id,
                        (x, c) => new { x.OrderDetail, x.ProductDetail, x.Size, Colour = c })
                    .Select(x => new MeoMeo.Contract.DTOs.OrderDetail.OrderDetailDTO
                    {
                        Id = x.OrderDetail.Id,
                        OrderId = x.OrderDetail.OrderId,
                        ProductDetailId = x.OrderDetail.ProductDetailId,
                        PromotionDetailId = x.OrderDetail.PromotionDetailId ?? Guid.Empty,
                        Sku = x.OrderDetail.Sku,
                        Price = x.OrderDetail.Price,
                        OriginalPrice = x.OrderDetail.OriginalPrice,
                        Quantity = x.OrderDetail.Quantity,
                        ProductName = x.OrderDetail.ProductName,
                        Discount = x.OrderDetail.Discount ?? 0,
                        Image = x.OrderDetail.Image,
                        SizeName = x.Size.Value,
                        ColourName = x.Colour.Name
                    })
                    .ToListAsync();

                return new CreatePosOrderResultDTO
                {
                    ResponseStatus = BaseStatus.Success,
                    Message = "Tạo đơn POS thành công",
                    OrderId = order.Id,
                    Code = order.Code,

                    // Thông tin đầy đủ để in hóa đơn
                    CreationTime = order.CreationTime,
                    Type = order.Type,
                    Status = order.Status,
                    PaymentMethod = order.PaymentMethod,
                    TotalPrice = (float)order.TotalPrice,
                    ShippingFee = (float)(order.ShippingFee ?? 0),
                    Note = order.Note,

                    // Thông tin khách hàng
                    CustomerName = order.CustomerName,
                    CustomerPhoneNumber = order.CustomerPhoneNumber,
                    CustomerEmail = "", // Customer model không có Email field

                    // Thông tin nhân viên
                    EmployeeName = order.EmployeeName,
                    EmployeePhoneNumber = order.EmployeePhoneNumber,

                    // Thông tin giao hàng
                    Delivery = request.Delivery,

                    // Chi tiết sản phẩm
                    OrderDetails = orderDetails
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

        // Pending Orders methods
        public async Task<GetPendingOrdersResponseDTO> GetPendingOrdersAsync(GetPendingOrdersRequestDTO request)
        {
            try
            {
                var query = _orderRepository.Query()
                    .Include(o => o.Customers)
                    .Include(o => o.Employee)
                    .Include(o => o.OrderDetails)
                    .Where(o => (o.Status == EOrderStatus.Pending ||
                               o.Status == EOrderStatus.Confirmed) &&
                               o.Type == EOrderType.Store); // Only POS orders

                // Filter by employee if specified
                if (request.EmployeeId.HasValue)
                {
                    query = query.Where(o => o.EmployeeId == request.EmployeeId.Value);
                }

                // Filter by date range
                if (request.FromDate.HasValue)
                {
                    query = query.Where(o => o.CreationTime >= request.FromDate.Value);
                }
                if (request.ToDate.HasValue)
                {
                    query = query.Where(o => o.CreationTime <= request.ToDate.Value);
                }

                // Order by creation time descending
                query = query.OrderByDescending(o => o.CreationTime);

                // Get total counts for metadata
                var totalPendingCount = await query.CountAsync();
                var totalDraftCount = await query.CountAsync(o => o.Status == EOrderStatus.Pending);
                var totalPendingAmount = await query.SumAsync(o => o.TotalPrice);

                // Apply paging
                var pagedResult = await query
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                // Map to DTOs
                var items = pagedResult.Select(o => new PendingOrderDTO
                {
                    Id = o.Id,
                    Code = o.Code,
                    CustomerName = o.Customers?.Name ?? "Khách lẻ",
                    CustomerPhone = o.Customers?.PhoneNumber ?? "",
                    TotalAmount = o.TotalPrice,
                    CreationTime = o.CreationTime,
                    LastModifiedTime = o.LastModifiedTime,
                    Status = o.Status,
                    Type = o.Type,
                    PaymentMethod = o.PaymentMethod,
                    Note = o.Note,
                    ItemCount = o.OrderDetails?.Count ?? 0,
                    IsDraft = o.Status == EOrderStatus.Pending
                }).ToList();

                return new GetPendingOrdersResponseDTO
                {
                    Items = items,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    TotalPendingCount = totalPendingCount,
                    TotalDraftCount = totalDraftCount,
                    TotalPendingAmount = totalPendingAmount
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting pending orders: {ex.Message}", ex);
            }
        }

        public async Task<BaseResponse> DeletePendingOrderAsync(Guid orderId)
        {
            try
            {
                var order = await _orderRepository.Query()
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    return new BaseResponse
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Không tìm thấy đơn hàng"
                    };
                }

                // Only allow deletion of pending or confirmed orders
                if (order.Status != EOrderStatus.Pending && order.Status != EOrderStatus.Confirmed)
                {
                    return new BaseResponse
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Chỉ có thể xóa đơn hàng ở trạng thái chờ xử lý hoặc đã xác nhận"
                    };
                }

                // Delete order details first
                if (order.OrderDetails?.Any() == true)
                {
                    foreach (var detail in order.OrderDetails)
                    {
                        await _orderDetailRepository.DeleteAsync(detail.Id);
                    }
                }

                // Delete the order
                await _orderRepository.DeleteAsync(orderId);
                await _unitOfWork.SaveChangesAsync();

                return new BaseResponse
                {
                    ResponseStatus = BaseStatus.Success,
                    Message = "Xóa đơn hàng thành công"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Lỗi khi xóa đơn hàng: {ex.Message}"
                };
            }
        }

        public async Task<CreatePosOrderResultDTO> UpdatePosOrderAsync(Guid orderId, Guid employeeId, CreatePosOrderDTO request)
        {
            try
            {
                // Find existing order - only POS orders (Store type)
                var existingOrder = await _orderRepository.Query()
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.Id == orderId && o.Type == EOrderType.Store);

                if (existingOrder == null)
                {
                    return new CreatePosOrderResultDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Không tìm thấy đơn hàng POS hoặc đơn hàng không phải loại tại quầy"
                    };
                }

                // Only allow update of pending or confirmed POS orders
                if (existingOrder.Status != EOrderStatus.Pending && existingOrder.Status != EOrderStatus.Confirmed)
                {
                    return new CreatePosOrderResultDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Chỉ có thể cập nhật đơn hàng POS ở trạng thái chờ xử lý hoặc đã xác nhận"
                    };
                }

                // Validate inventory for both Pending and Completed orders
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
                        // Get product name for better error message
                        var productDetail = await _productsDetailRepository.Query()
                            .Include(pd => pd.Product)
                            .FirstOrDefaultAsync(pd => pd.Id == g.ProductDetailId);

                        var productName = productDetail?.Product?.Name ?? "Sản phẩm";
                        var sku = productDetail?.Sku ?? g.ProductDetailId.ToString();

                        return new CreatePosOrderResultDTO
                        {
                            ResponseStatus = BaseStatus.Error,
                            Message = $"Không đủ tồn kho cho sản phẩm '{productName}' (SKU: {sku}). Cần {g.TotalQty}, còn {available}"
                        };
                    }
                }
                existingOrder.Type = EOrderType.Store;
                existingOrder.PaymentMethod = request.PaymentMethod;
                existingOrder.Note = request.Note;
                existingOrder.ShippingFee = request.ShippingFee;
                existingOrder.Status = request.Status;
                existingOrder.CustomerId = request.CustomerId;
                existingOrder.LastModifiedTime = DateTime.UtcNow;
                existingOrder.DeliveryAddress = null;
                if (existingOrder.OrderDetails?.Any() == true)
                {
                    foreach (var detail in existingOrder.OrderDetails)
                    {
                        await _orderDetailRepository.DeleteAsync(detail.Id);
                    }
                }
                if (request.Items?.Any() == true)
                {
                    // Sử dụng thông tin từ DTO thay vì query database
                    foreach (var item in request.Items)
                    {
                        var orderDetail = new OrderDetail
                        {
                            Id = item.Id == Guid.Empty ? Guid.NewGuid() : item.Id, // Tạo mới hoặc giữ nguyên ID
                            OrderId = orderId,
                            ProductDetailId = item.ProductDetailId,
                            PromotionDetailId = item.PromotionDetailId,
                            Sku = item.Sku,
                            Price = item.Price,
                            OriginalPrice = item.OriginalPrice,
                            Quantity = item.Quantity,
                            ProductName = item.ProductName,
                            Discount = item.Discount > 0 ? item.Discount : null,
                            Image = item.Image
                        };
                        await _orderDetailRepository.AddAsync(orderDetail);
                    }
                }

                // Only deduct inventory if order status changes to completed
                if (request.Status == EOrderStatus.Completed && existingOrder.Status != EOrderStatus.Completed)
                {
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

                    // Cập nhật SellNumber khi đơn POS chuyển sang hoàn thành
                    var productDetailGroups = request.Items
                        .GroupBy(item => item.ProductDetailId)
                        .Select(g => new { ProductDetailId = g.Key, TotalQuantity = g.Sum(item => item.Quantity) })
                        .ToList();

                    foreach (var group in productDetailGroups)
                    {
                        var productDetail = await _productsDetailRepository.GetByIdAsync(group.ProductDetailId);
                        if (productDetail != null)
                        {
                            productDetail.SellNumber = (productDetail.SellNumber ?? 0) + group.TotalQuantity;
                            await _productsDetailRepository.UpdateAsync(productDetail);
                        }
                    }
                }

                // Add order history for POS order update
                var oldStatus = existingOrder.Status;
                var orderHistory = new OrderHistory
                {
                    Id = Guid.NewGuid(),
                    OrderId = existingOrder.Id,
                    Type = EHistoryType.Update,
                    CreationTime = DateTime.Now,
                    CreatedBy = Guid.Empty, // System generated
                    Content = $"""
                        <p><strong>Cập nhật đơn hàng:</strong> Nhân viên đã cập nhật đơn hàng tại quầy</p>
                        """
                };
                await _orderHistoryRepository.AddAsync(orderHistory);

                // Update order
                await _orderRepository.UpdateAsync(existingOrder);
                await _unitOfWork.SaveChangesAsync();

                // Lấy thông tin đầy đủ để trả về với join Size và Colour
                var orderDetails = await _orderDetailRepository.Query()
                    .Where(od => od.OrderId == existingOrder.Id)
                    .Join(_productsDetailRepository.Query(),
                        od => od.ProductDetailId,
                        pd => pd.Id,
                        (od, pd) => new { OrderDetail = od, ProductDetail = pd })
                    .Join(_sizeRepository.Query(),
                        x => x.ProductDetail.SizeId,
                        s => s.Id,
                        (x, s) => new { x.OrderDetail, x.ProductDetail, Size = s })
                    .Join(_colourRepository.Query(),
                        x => x.ProductDetail.ColourId,
                        c => c.Id,
                        (x, c) => new { x.OrderDetail, x.ProductDetail, x.Size, Colour = c })
                    .Select(x => new MeoMeo.Contract.DTOs.OrderDetail.OrderDetailDTO
                    {
                        Id = x.OrderDetail.Id,
                        OrderId = x.OrderDetail.OrderId,
                        ProductDetailId = x.OrderDetail.ProductDetailId,
                        PromotionDetailId = x.OrderDetail.PromotionDetailId ?? Guid.Empty,
                        Sku = x.OrderDetail.Sku,
                        Price = x.OrderDetail.Price,
                        OriginalPrice = x.OrderDetail.OriginalPrice,
                        Quantity = x.OrderDetail.Quantity,
                        ProductName = x.OrderDetail.ProductName,
                        Discount = x.OrderDetail.Discount ?? 0,
                        Image = x.OrderDetail.Image,
                        SizeName = x.Size.Value,
                        ColourName = x.Colour.Name
                    })
                    .ToListAsync();

                return new CreatePosOrderResultDTO
                {
                    ResponseStatus = BaseStatus.Success,
                    Message = "Cập nhật đơn hàng thành công",
                    OrderId = orderId,
                    Code = existingOrder.Code,

                    // Thông tin đầy đủ để in hóa đơn
                    CreationTime = existingOrder.CreationTime,
                    Type = existingOrder.Type,
                    Status = existingOrder.Status,
                    PaymentMethod = existingOrder.PaymentMethod,
                    TotalPrice = (float)existingOrder.TotalPrice,
                    ShippingFee = (float)(existingOrder.ShippingFee ?? 0),
                    Note = existingOrder.Note,

                    // Thông tin khách hàng
                    CustomerName = existingOrder.CustomerName,
                    CustomerPhoneNumber = existingOrder.CustomerPhoneNumber,
                    CustomerEmail = "", // Cần lấy từ customer table

                    // Thông tin nhân viên
                    EmployeeName = existingOrder.EmployeeName,
                    EmployeePhoneNumber = existingOrder.EmployeePhoneNumber,

                    // Thông tin giao hàng
                    Delivery = request.Delivery,

                    // Chi tiết sản phẩm
                    OrderDetails = orderDetails
                };
            }
            catch (Exception ex)
            {
                return new CreatePosOrderResultDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Lỗi khi cập nhật đơn hàng: {ex.Message}"
                };
            }
        }

        private string GetOrderReturnStatusDisplayName(EOrderReturnStatus status)
        {
            return status switch
            {
                EOrderReturnStatus.Pending => "Chờ duyệt hoàn hàng",
                EOrderReturnStatus.Approved => "Đã duyệt hoàn hàng",
                EOrderReturnStatus.Rejected => "Từ chối hoàn hàng",
                EOrderReturnStatus.Received => "Đã nhận hàng hoàn",
                EOrderReturnStatus.Refunded => "Hoàn tiền xong",
                _ => status.ToString()
            };
        }

        private string GetRefundMethodDisplayName(ERefundMethod method)
        {
            return method switch
            {
                ERefundMethod.BankTransfer => "Chuyển khoản",
                ERefundMethod.ViaShipper => "Nhận qua ship",
                ERefundMethod.InStore => "Nhận tại cửa hàng",
                _ => method.ToString()
            };
        }

        private string GetPaymentMethodDisplayName(EOrderPaymentMethod method)
        {
            return method switch
            {
                EOrderPaymentMethod.Cash => "Tiền mặt",
                EOrderPaymentMethod.Transfer => "Chuyển khoản",
                _ => method.ToString()
            };
        }

        private string GetOrderStatusDisplayName(EOrderStatus status)
        {
            return status switch
            {
                EOrderStatus.Pending => "Chờ xác nhận",
                EOrderStatus.Confirmed => "Đã xác nhận",
                EOrderStatus.InTransit => "Đang giao",
                EOrderStatus.Canceled => "Đã hủy",
                EOrderStatus.Completed => "Hoàn thành",
                EOrderStatus.PendingReturn => "Chờ xác nhận hoàn hàng",
                EOrderStatus.Returned => "Đã hoàn hàng",
                EOrderStatus.RejectReturned => "Từ chối cho phép hoàn hàng",
                _ => status.ToString()
            };
        }

        private async Task EnrichOrderDataAsync(List<OrderDTO> orders)
        {
            var listOrderIds = orders.Select(c => c.Id).ToList();

            // Get Order Details with Size and Colour information
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

            // Get Order Return information
            var orderReturns = await _orderReturnRepository.Query()
                .Include(or => or.Files)
                .Include(or => or.Items)
                    .ThenInclude(item => item.OrderDetail)
                        .ThenInclude(od => od.ProductDetail)
                            .ThenInclude(pd => pd.Size)
                .Include(or => or.Items)
                    .ThenInclude(item => item.OrderDetail)
                        .ThenInclude(od => od.ProductDetail)
                            .ThenInclude(pd => pd.Colour)
                .Include(or => or.Items)
                    .ThenInclude(item => item.OrderDetail)
                        .ThenInclude(od => od.ProductDetail)
                            .ThenInclude(pd => pd.Product)
                .Where(or => listOrderIds.Contains(or.OrderId))
                .ToListAsync();
            var groupedOrderReturns = orderReturns
                .GroupBy(or => or.OrderId)
                .ToDictionary(g => g.Key, g => g.FirstOrDefault());

            foreach (var order in orders)
            {
                // Set Order Details
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

                // Set Order Return information
                if (groupedOrderReturns.TryGetValue(order.Id, out var orderReturn) && orderReturn != null)
                {
                    order.OrderReturn = new OrderReturnSummaryDTO
                    {
                        Id = orderReturn.Id,
                        Code = orderReturn.Code,
                        Reason = orderReturn.Reason,
                        Status = orderReturn.Status,
                        RefundMethod = orderReturn.RefundMethod,
                        PayBackAmount = orderReturn.PayBackAmount,
                        PayBackDate = orderReturn.PayBackDate,
                        CreationTime = orderReturn.CreationTime,
                        LastModifiedTime = orderReturn.LastModifiedTime,
                        TotalItemCount = orderReturn.Items?.Sum(i => i.Quantity) ?? 0,
                        TotalRefundAmount = orderReturn.Items?.Sum(i => (decimal)(i.OrderDetail?.Price ?? 0f) * i.Quantity) ?? 0,
                        Files = orderReturn.Files?.Select(f => new OrderReturnFileSummaryDTO
                        {
                            Id = f.Id,
                            Name = f.Name,
                            Url = f.Url,
                            ContentType = f.ContentType
                        }).ToList() ?? new List<OrderReturnFileSummaryDTO>(),
                        Items = orderReturn.Items?.Select(item => new OrderReturnItemSummaryDTO
                        {
                            Id = item.Id,
                            OrderDetailId = item.OrderDetailId,
                            ProductName = item.OrderDetail?.ProductDetail?.Product?.Name ?? "N/A",
                            SizeName = item.OrderDetail?.ProductDetail?.Size?.Value ?? "N/A",
                            ColourName = item.OrderDetail?.ProductDetail?.Colour?.Name ?? "N/A",
                            Sku = item.OrderDetail?.Sku ?? "N/A",
                            Quantity = item.Quantity,
                            UnitPrice = (decimal)(item.OrderDetail?.Price ?? 0f),
                            TotalPrice = (decimal)(item.OrderDetail?.Price ?? 0f) * item.Quantity,
                            ImageUrl = item.OrderDetail?.Image ?? string.Empty
                        }).ToList() ?? new List<OrderReturnItemSummaryDTO>(),
                        BankInfo = !string.IsNullOrEmpty(orderReturn.BankName) ? new OrderReturnBankInfoDTO
                        {
                            BankName = orderReturn.BankName,
                            AccountNumber = orderReturn.BankAccountNumber ?? string.Empty,
                            AccountHolderName = orderReturn.BankAccountName ?? string.Empty,
                        } : null,
                        ContactName = orderReturn.ContactName,
                        ContactPhone = orderReturn.ContactPhone,
                        StatusDisplayName = GetOrderReturnStatusDisplayName(orderReturn.Status),
                        RefundMethodDisplayName = GetRefundMethodDisplayName(orderReturn.RefundMethod)
                    };
                }
            }
        }

    }
}