using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Order.Return;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Commons.Enums;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeoMeo.Application.Services
{
    public class OrderReturnService : IOrderReturnService
    {
        private readonly IOrderReturnRepository _orderReturnRepository;
        private readonly IOrderReturnItemRepository _orderReturnItemRepository;
        private readonly IOrderReturnFileRepository _orderReturnFileRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IOrderHistoryRepository _orderHistoryRepository;
        private readonly IIventoryBatchReposiory _inventoryBatchRepository;
        private readonly IInventoryTranSactionRepository _inventoryTransactionRepository;
        private readonly IProductsDetailRepository _productsDetailRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrderReturnService(
            IOrderReturnRepository orderReturnRepository,
            IOrderReturnItemRepository orderReturnItemRepository,
            IOrderReturnFileRepository orderReturnFileRepository,
            IOrderRepository orderRepository,
            IOrderDetailRepository orderDetailRepository,
            IOrderHistoryRepository orderHistoryRepository,
            IIventoryBatchReposiory inventoryBatchRepository,
            IInventoryTranSactionRepository inventoryTransactionRepository,
            IProductsDetailRepository productsDetailRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _orderReturnRepository = orderReturnRepository;
            _orderReturnItemRepository = orderReturnItemRepository;
            _orderReturnFileRepository = orderReturnFileRepository;
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _orderHistoryRepository = orderHistoryRepository;
            _inventoryBatchRepository = inventoryBatchRepository;
            _inventoryTransactionRepository = inventoryTransactionRepository;
            _productsDetailRepository = productsDetailRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> CreatePartialOrderReturnAsync(Guid customerId, CreatePartialOrderReturnDTO request)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Validate order
                var order = await _orderRepository.Query()
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.ProductDetail)
                    .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.CustomerId == customerId);

                if (order == null)
                {
                    return new BaseResponse
                    {
                        Message = "Không tìm thấy đơn hàng",
                        ResponseStatus = BaseStatus.Error
                    };
                }

                // Check if order can be returned
                if (!await CanOrderBeReturnedAsync(request.OrderId))
                {
                    return new BaseResponse
                    {
                        Message = "Đơn hàng này không thể hoàn trả",
                        ResponseStatus = BaseStatus.Error
                    };
                }

                // Validate return items
                var validationResult = await ValidateReturnItemsAsync(request.Items, order.OrderDetails.ToList());
                if (!validationResult.IsValid)
                {
                    return new BaseResponse
                    {
                        Message = validationResult.ErrorMessage,
                        ResponseStatus = BaseStatus.Error
                    };
                }

                // Generate return code
                var returnCode = await GenerateReturnCodeAsync();

                // Create order return
                var orderReturn = new OrderReturn
                {
                    Id = Guid.NewGuid(),
                    OrderId = request.OrderId,
                    CustomerId = customerId,
                    Code = returnCode,
                    Reason = request.Reason,
                    Status = EOrderReturnStatus.Pending,
                    RefundMethod = request.RefundMethod,
                    BankName = request.BankName,
                    BankAccountName = request.BankAccountName,
                    BankAccountNumber = request.BankAccountNumber,
                    ContactPhone = request.ContactPhone,
                    ContactName = request.ContactName,
                    CreationTime = DateTime.Now
                };

                await _orderReturnRepository.AddAsync(orderReturn);

                // Create return items
                foreach (var itemDto in request.Items)
                {
                    var returnItem = new OrderReturnItem
                    {
                        Id = Guid.NewGuid(),
                        OrderReturnId = orderReturn.Id,
                        OrderDetailId = itemDto.OrderDetailId,
                        Quantity = itemDto.Quantity,
                        Reason = itemDto.Reason
                    };

                    await _orderReturnItemRepository.AddAsync(returnItem);
                }

                // Create return files - skip for now, will be handled in overload method

                // Update order status to PendingReturn
                order.Status = EOrderStatus.PendingReturn;
                order.LastModifiedTime = DateTime.Now;
                await _orderRepository.UpdateAsync(order);

                // Add order history
                var productNames = string.Join(", ", request.Items.Select(item =>
                    order.OrderDetails.FirstOrDefault(od => od.Id == item.OrderDetailId)?.ProductName ?? "Unknown"));
                var historyContent = $"""
                    <p><strong>Yêu cầu hoàn trả:</strong> Khách hàng đã tạo yêu cầu hoàn trả</p>
                    """;
                await AddOrderHistoryAsync(request.OrderId, historyContent);

                await _unitOfWork.CommitAsync();

                return new BaseResponse
                {
                    Message = $"Tạo yêu cầu hoàn trả thành công. Mã hoàn trả: {returnCode}",
                    ResponseStatus = BaseStatus.Success
                };
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

        public async Task<BaseResponse> CreatePartialOrderReturnAsync(Guid customerId, CreatePartialOrderReturnDTO request, List<FileUploadResult> uploadedFiles)
        {
            try
            {
                // Validate order
                var order = await _orderRepository.Query()
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.ProductDetail)
                    .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.CustomerId == customerId);

                if (order == null)
                {
                    return new BaseResponse
                    {
                        Message = "Không tìm thấy đơn hàng",
                        ResponseStatus = BaseStatus.Error
                    };
                }

                // Check if order can be returned
                if (!await CanOrderBeReturnedAsync(request.OrderId))
                {
                    return new BaseResponse
                    {
                        Message = "Đơn hàng này không thể hoàn trả",
                        ResponseStatus = BaseStatus.Error
                    };
                }

                // Validate return items
                var validationResult = await ValidateReturnItemsAsync(request.Items, order.OrderDetails.ToList());
                if (!validationResult.IsValid)
                {
                    return new BaseResponse
                    {
                        Message = validationResult.ErrorMessage,
                        ResponseStatus = BaseStatus.Error
                    };
                }

                // Generate return code
                var returnCode = await GenerateReturnCodeAsync();

                // Create order return
                var orderReturn = new OrderReturn
                {
                    Id = Guid.NewGuid(),
                    OrderId = request.OrderId,
                    CustomerId = customerId,
                    Code = returnCode,
                    Reason = request.Reason,
                    Status = EOrderReturnStatus.Pending,
                    RefundMethod = request.RefundMethod,
                    BankName = request.BankName,
                    BankAccountName = request.BankAccountName,
                    BankAccountNumber = request.BankAccountNumber,
                    ContactPhone = request.ContactPhone,
                    ContactName = request.ContactName,
                    CreationTime = DateTime.Now
                };

                await _orderReturnRepository.AddAsync(orderReturn);

                // Create return items
                foreach (var itemDto in request.Items)
                {
                    var returnItem = new OrderReturnItem
                    {
                        Id = Guid.NewGuid(),
                        OrderReturnId = orderReturn.Id,
                        OrderDetailId = itemDto.OrderDetailId,
                        Quantity = itemDto.Quantity,
                        Reason = itemDto.Reason
                    };

                    await _orderReturnItemRepository.AddAsync(returnItem);
                }

                // Create return files from uploaded files
                foreach (var uploadedFile in uploadedFiles)
                {
                    var returnFile = new OrderReturnFile
                    {
                        Id = Guid.NewGuid(),
                        OrderReturnId = orderReturn.Id,
                        Name = uploadedFile.FileName,
                        Url = uploadedFile.RelativePath,
                        ContentType = GetContentTypeFromFileName(uploadedFile.FileName)
                    };

                    await _orderReturnFileRepository.AddAsync(returnFile);
                }

                // Update order status to PendingReturn
                order.Status = EOrderStatus.PendingReturn;
                order.LastModifiedTime = DateTime.Now;
                await _orderRepository.UpdateAsync(order);

                await _unitOfWork.CommitAsync();

                return new BaseResponse
                {
                    Message = $"Tạo yêu cầu hoàn trả thành công. Mã hoàn trả: {returnCode}",
                    ResponseStatus = BaseStatus.Success
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Message = $"Lỗi khi tạo yêu cầu hoàn trả: {ex.Message}",
                    ResponseStatus = BaseStatus.Error
                };
            }
        }

        public async Task<PagingExtensions.PagedResult<OrderReturnListDTO>> GetOrderReturnsAsync(GetOrderReturnRequestDTO request)
        {
            var query = _orderReturnRepository.Query()
                .Include(or => or.Order)
                .Include(or => or.Customer)
                .Include(or => or.Items)
                .ThenInclude(i => i.OrderDetail)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(request.CodeFilter))
                query = query.Where(or => or.Code.Contains(request.CodeFilter));

            if (!string.IsNullOrEmpty(request.OrderCodeFilter))
                query = query.Where(or => or.Order.Code.Contains(request.OrderCodeFilter));

            if (request.StatusFilter.HasValue)
                query = query.Where(or => or.Status == request.StatusFilter.Value);

            if (request.RefundMethodFilter.HasValue)
                query = query.Where(or => or.RefundMethod == request.RefundMethodFilter.Value);

            if (request.FromDateFilter.HasValue)
                query = query.Where(or => or.CreationTime >= request.FromDateFilter.Value);

            if (request.ToDateFilter.HasValue)
                query = query.Where(or => or.CreationTime <= request.ToDateFilter.Value);

            if (request.CustomerIdFilter.HasValue)
                query = query.Where(or => or.CustomerId == request.CustomerIdFilter.Value);

            // Order by creation time descending
            query = query.OrderByDescending(or => or.CreationTime);

            var totalRecords = await query.CountAsync();
            var items = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var resultItems = items.Select(or => new OrderReturnListDTO
            {
                Id = or.Id,
                Code = or.Code,
                OrderId = or.OrderId,
                OrderCode = or.Order.Code,
                Reason = or.Reason,
                Status = or.Status,
                RefundMethod = or.RefundMethod,
                CreationTime = or.CreationTime,
                LastModifiedTime = or.LastModifiedTime,
                PayBackAmount = or.PayBackAmount,
                PayBackDate = or.PayBackDate,
                CustomerName = or.Customer.Name,
                CustomerPhone = or.Customer.PhoneNumber,
                TotalItemCount = or.Items.Sum(i => i.Quantity),
                TotalRefundAmount = CalculateRefundAmount(or.Items),
                StatusDisplayName = GetStatusDisplayName(or.Status),
                RefundMethodDisplayName = GetRefundMethodDisplayName(or.RefundMethod)
            }).ToList();

            return new PagingExtensions.PagedResult<OrderReturnListDTO>
            {
                Items = resultItems,
                TotalRecords = totalRecords,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };
        }

        public async Task<PagingExtensions.PagedResult<OrderReturnListDTO>> GetMyOrderReturnsAsync(Guid customerId, GetOrderReturnRequestDTO request)
        {
            request.CustomerIdFilter = customerId;
            return await GetOrderReturnsAsync(request);
        }

        public async Task<OrderReturnDetailDTO?> GetOrderReturnByIdAsync(Guid id)
        {
            var orderReturn = await _orderReturnRepository.Query()
                .Include(or => or.Order)
                .Include(or => or.Customer)
                .Include(or => or.Items)
                .ThenInclude(i => i.OrderDetail)
                .Include(or => or.Files)
                .FirstOrDefaultAsync(or => or.Id == id);

            if (orderReturn == null)
                return null;

            return new OrderReturnDetailDTO
            {
                Id = orderReturn.Id,
                OrderId = orderReturn.OrderId,
                CustomerId = orderReturn.CustomerId,
                Code = orderReturn.Code,
                Reason = orderReturn.Reason,
                Status = orderReturn.Status,
                RefundMethod = orderReturn.RefundMethod,
                PayBackAmount = orderReturn.PayBackAmount,
                PayBackDate = orderReturn.PayBackDate,
                BankName = orderReturn.BankName,
                BankAccountName = orderReturn.BankAccountName,
                BankAccountNumber = orderReturn.BankAccountNumber,
                ContactPhone = orderReturn.ContactPhone,
                ContactName = orderReturn.ContactName,
                CreationTime = orderReturn.CreationTime,
                LastModifiedTime = orderReturn.LastModifiedTime,
                Order = new OrderReturnOrderInfo
                {
                    Id = orderReturn.Order?.Id ?? Guid.Empty,
                    Code = orderReturn.Order?.Code ?? "",
                    CustomerName = orderReturn.Order?.CustomerName ?? "",
                    CustomerPhone = orderReturn.Order?.CustomerPhoneNumber ?? "",
                    OrderDate = orderReturn.Order?.CreationTime ?? DateTime.MinValue,
                    TotalAmount = orderReturn.Order?.TotalPrice ?? 0,
                    Status = orderReturn.Order?.Status ?? Domain.Commons.Enums.EOrderStatus.Pending
                },
                Items = orderReturn.Items.Select(i => new OrderReturnItemDetailDTO
                {
                    Id = i.Id,
                    OrderDetailId = i.OrderDetailId,
                    Quantity = i.Quantity,
                    Reason = i.Reason,
                    ProductName = i.OrderDetail.ProductName,
                    Sku = i.OrderDetail.Sku,
                    Image = i.OrderDetail.Image,
                    UnitPrice = i.OrderDetail?.Price ?? 0f,
                    OriginalPrice = i.OrderDetail?.OriginalPrice ?? 0f,
                    Discount = i.OrderDetail?.Discount ?? 0f,
                    RefundAmount = (decimal)((i.OrderDetail?.Price ?? 0f) * i.Quantity)
                }).ToList(),
                Files = orderReturn.Files.Select(f => new OrderReturnFileDetailDTO
                {
                    Id = f.Id,
                    Name = f.Name,
                    Url = f.Url,
                    ContentType = f.ContentType
                }).ToList(),
                TotalRefundAmount = CalculateRefundAmount(orderReturn.Items),
                TotalItemCount = orderReturn.Items.Sum(i => i.Quantity),
                StatusDisplayName = GetStatusDisplayName(orderReturn.Status),
                RefundMethodDisplayName = GetRefundMethodDisplayName(orderReturn.RefundMethod)
            };
        }

        public async Task<BaseResponse> UpdateOrderReturnStatusAsync(Guid id, UpdateOrderReturnStatusRequestDTO request)
        {
            try
            {
                var orderReturn = await _orderReturnRepository.Query()
                    .Include(or => or.Order)
                    .FirstOrDefaultAsync(or => or.Id == id);

                if (orderReturn == null)
                {
                    return new BaseResponse
                    {
                        Message = "Không tìm thấy yêu cầu hoàn trả",
                        ResponseStatus = BaseStatus.Error
                    };
                }

                var oldStatus = orderReturn.Status;
                orderReturn.Status = request.Status;
                orderReturn.LastModifiedTime = DateTime.Now;

                // Update order status based on return status
                if (request.Status == EOrderReturnStatus.Approved)
                {
                    orderReturn.Order.Status = EOrderStatus.Returned;
                    orderReturn.Order.LastModifiedTime = DateTime.Now;

                    // Back inventory when return is approved
                    await BackInventoryForReturnAsync(orderReturn.Id);

                    // Giảm SellNumber khi hoàn hàng được duyệt
                    await DecreaseSellNumberForReturnAsync(orderReturn.Id);
                }
                else if (request.Status == EOrderReturnStatus.Rejected)
                {
                    // Set order status to RejectReturned when return is rejected
                    orderReturn.Order.Status = EOrderStatus.RejectReturned;
                    orderReturn.Order.LastModifiedTime = DateTime.Now;
                }

                await _orderReturnRepository.UpdateAsync(orderReturn);
                await _orderRepository.UpdateAsync(orderReturn.Order);

                // Add order history
                var historyContent = $"""
                    <p><strong>Cập nhật trạng thái hoàn trả:</strong> Từ 
                    <span class="status-old">{GetOrderReturnStatusDisplayName(oldStatus)}</span> 
                    => 
                    <span class="status-new">{GetOrderReturnStatusDisplayName(request.Status)}</span></p>
                    """;
                await AddOrderHistoryAsync(orderReturn.OrderId, historyContent);

                await _unitOfWork.SaveChangesAsync();

                return new BaseResponse
                {
                    Message = "Cập nhật trạng thái thành công",
                    ResponseStatus = BaseStatus.Success
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Message = ex.Message,
                    ResponseStatus = BaseStatus.Error
                };
            }
        }

        public async Task<BaseResponse> UpdatePayBackAmountAsync(Guid id, UpdatePayBackAmountDTO request)
        {
            try
            {
                var orderReturn = await _orderReturnRepository.Query()
                    .Include(or => or.Order)
                    .FirstOrDefaultAsync(or => or.Id == id);

                if (orderReturn == null)
                {
                    return new BaseResponse
                    {
                        Message = "Không tìm thấy yêu cầu hoàn trả",
                        ResponseStatus = BaseStatus.Error
                    };
                }

                // Only allow updating payback amount for approved or received returns
                if (orderReturn.Status != EOrderReturnStatus.Approved &&
                    orderReturn.Status != EOrderReturnStatus.Received)
                {
                    return new BaseResponse
                    {
                        Message = "Chỉ có thể cập nhật số tiền hoàn trả cho yêu cầu đã được duyệt hoặc đã nhận hàng",
                        ResponseStatus = BaseStatus.Error
                    };
                }

                orderReturn.PayBackAmount = request.PayBackAmount;
                orderReturn.PayBackDate = request.PayBackDate ?? DateTime.Now;
                orderReturn.Status = EOrderReturnStatus.Refunded; // Auto update status to refunded
                orderReturn.LastModifiedTime = DateTime.Now;

                // Update order status to Completed when refund is processed
                orderReturn.Order.Status = EOrderStatus.Completed;
                orderReturn.Order.LastModifiedTime = DateTime.Now;

                await _orderReturnRepository.UpdateAsync(orderReturn);
                await _orderRepository.UpdateAsync(orderReturn.Order);

                // Add order history
                var historyContent = $"""
                    <p><strong>Hoàn tiền thành công:</strong> Đã hoàn trả số tiền cho khách hàng</p>
                    """;
                await AddOrderHistoryAsync(orderReturn.OrderId, historyContent);

                await _unitOfWork.SaveChangesAsync();

                return new BaseResponse
                {
                    Message = "Cập nhật số tiền hoàn trả thành công",
                    ResponseStatus = BaseStatus.Success
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Message = ex.Message,
                    ResponseStatus = BaseStatus.Error
                };
            }
        }

        public async Task<BaseResponse> CancelOrderReturnAsync(Guid customerId, Guid orderReturnId)
        {
            try
            {
                var orderReturn = await _orderReturnRepository.Query()
                    .Include(or => or.Order)
                    .FirstOrDefaultAsync(or => or.Id == orderReturnId && or.CustomerId == customerId);

                if (orderReturn == null)
                {
                    return new BaseResponse
                    {
                        Message = "Không tìm thấy yêu cầu hoàn trả",
                        ResponseStatus = BaseStatus.Error
                    };
                }

                if (orderReturn.Status != EOrderReturnStatus.Pending)
                {
                    return new BaseResponse
                    {
                        Message = "Chỉ có thể hủy yêu cầu hoàn trả đang chờ duyệt",
                        ResponseStatus = BaseStatus.Error
                    };
                }

                orderReturn.Status = EOrderReturnStatus.Rejected;
                orderReturn.LastModifiedTime = DateTime.Now;

                // Set order status to RejectReturned when return is cancelled
                orderReturn.Order.Status = EOrderStatus.RejectReturned;
                orderReturn.Order.LastModifiedTime = DateTime.Now;

                await _orderReturnRepository.UpdateAsync(orderReturn);
                await _orderRepository.UpdateAsync(orderReturn.Order);

                // Add order history
                var historyContent = $"""
                    <p><strong>Hủy yêu cầu hoàn trả:</strong> Khách hàng đã hủy yêu cầu hoàn trả</p>
                    """;
                await AddOrderHistoryAsync(orderReturn.OrderId, historyContent);

                await _unitOfWork.SaveChangesAsync();

                return new BaseResponse
                {
                    Message = "Hủy yêu cầu hoàn trả thành công",
                    ResponseStatus = BaseStatus.Success
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Message = ex.Message,
                    ResponseStatus = BaseStatus.Error
                };
            }
        }

        public async Task<List<OrderReturnItemDetailDTO>> GetAvailableItemsForReturnAsync(Guid orderId)
        {
            var orderDetails = await _orderDetailRepository.Query()
                .Include(od => od.ProductDetail)
                    .ThenInclude(pd => pd.Size)
                .Include(od => od.ProductDetail)
                    .ThenInclude(pd => pd.Colour)
                .Where(od => od.OrderId == orderId)
                .ToListAsync();

            // Get already returned quantities
            var returnedQuantities = await _orderReturnItemRepository.Query()
                .Where(ri => ri.OrderReturn.OrderId == orderId &&
                           ri.OrderReturn.Status != EOrderReturnStatus.Rejected)
                .GroupBy(ri => ri.OrderDetailId)
                .Select(g => new { OrderDetailId = g.Key, ReturnedQty = g.Sum(x => x.Quantity) })
                .ToListAsync();

            var result = new List<OrderReturnItemDetailDTO>();

            foreach (var orderDetail in orderDetails)
            {
                // Check if product detail allows return
                if (orderDetail.ProductDetail?.AllowReturn != true)
                    continue;

                var returnedQty = returnedQuantities
                    .FirstOrDefault(r => r.OrderDetailId == orderDetail.Id)?.ReturnedQty ?? 0;

                var availableQty = orderDetail.Quantity - returnedQty;

                if (availableQty > 0)
                {
                    var size = orderDetail.ProductDetail?.Size?.Value ?? "N/A";
                    var color = orderDetail.ProductDetail?.Colour?.Name ?? "N/A";
                    var detailedProductName = $"{orderDetail.ProductName} - Size: {size}, Màu: {color}";

                    result.Add(new OrderReturnItemDetailDTO
                    {
                        OrderDetailId = orderDetail.Id,
                        ProductName = detailedProductName,
                        Sku = orderDetail.Sku,
                        Image = orderDetail.Image,
                        UnitPrice = orderDetail.Price,
                        OriginalPrice = orderDetail.OriginalPrice,
                        Discount = orderDetail.Discount ?? 0f,
                        AvailableQuantity = availableQty,
                        Quantity = 0, // Default selection
                        RefundAmount = 0 // Will be calculated when quantity is selected
                    });
                }
            }

            return result;
        }

        public async Task<bool> CanOrderBeReturnedAsync(Guid orderId)
        {
            var order = await _orderRepository.Query()
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProductDetail)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return false;

            // Only completed orders can be returned
            if (order.Status != EOrderStatus.Completed)
                return false;

            // Check if at least one product in the order allows return
            var hasReturnableProduct = order.OrderDetails.Any(od => od.ProductDetail?.AllowReturn == true);
            return hasReturnableProduct;
        }

        public async Task<(bool CanReturn, string Message, List<string> ReturnableProducts, List<string> NonReturnableProducts)> GetOrderReturnInfoAsync(Guid orderId)
        {
            var order = await _orderRepository.Query()
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProductDetail)
                        .ThenInclude(pd => pd.Size)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProductDetail)
                        .ThenInclude(pd => pd.Colour)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            var returnableProducts = new List<string>();
            var nonReturnableProducts = new List<string>();

            if (order == null)
            {
                return (false, "Không tìm thấy đơn hàng", returnableProducts, nonReturnableProducts);
            }

            // Check order status
            if (order.Status != EOrderStatus.Completed)
            {
                return (false, "Chỉ có thể hoàn trả đơn hàng đã hoàn thành", returnableProducts, nonReturnableProducts);
            }

            // Get already returned quantities
            var returnedQuantities = await _orderReturnItemRepository.Query()
                .Where(ri => ri.OrderReturn.OrderId == orderId &&
                           ri.OrderReturn.Status != EOrderReturnStatus.Rejected)
                .GroupBy(ri => ri.OrderDetailId)
                .Select(g => new { OrderDetailId = g.Key, ReturnedQty = g.Sum(x => x.Quantity) })
                .ToListAsync();

            foreach (var orderDetail in order.OrderDetails)
            {
                var productName = orderDetail.ProductName;
                var size = orderDetail.ProductDetail?.Size?.Value ?? "N/A";
                var color = orderDetail.ProductDetail?.Colour?.Name ?? "N/A";
                var sku = orderDetail.Sku;

                // Create detailed product description
                var productDescription = $"{productName} - Size: {size}, Màu: {color} (SKU: {sku})";

                if (orderDetail.ProductDetail?.AllowReturn == true)
                {
                    var returnedQty = returnedQuantities
                        .FirstOrDefault(r => r.OrderDetailId == orderDetail.Id)?.ReturnedQty ?? 0;
                    var availableQty = orderDetail.Quantity - returnedQty;

                    if (availableQty > 0)
                    {
                        returnableProducts.Add($"{productDescription} - Còn lại: {availableQty} sản phẩm");
                    }
                    else
                    {
                        nonReturnableProducts.Add($"{productDescription} - Đã hoàn trả hết");
                    }
                }
                else
                {
                    nonReturnableProducts.Add($"{productDescription} - Không cho phép hoàn trả");
                }
            }

            var canReturn = returnableProducts.Any();
            var message = canReturn
                ? $"Có {returnableProducts.Count} sản phẩm có thể hoàn trả"
                : "Không có sản phẩm nào có thể hoàn trả";

            return (canReturn, message, returnableProducts, nonReturnableProducts);
        }

        private async Task<(bool IsValid, string ErrorMessage)> ValidateReturnItemsAsync(
            List<OrderReturnItemDTO> returnItems, List<OrderDetail> orderDetails)
        {
            foreach (var returnItem in returnItems)
            {
                var orderDetail = orderDetails.FirstOrDefault(od => od.Id == returnItem.OrderDetailId);
                if (orderDetail == null)
                {
                    return (false, "Sản phẩm không tồn tại trong đơn hàng");
                }

                // Check if product detail allows return
                if (orderDetail.ProductDetail?.AllowReturn != true)
                {
                    return (false, $"Sản phẩm {orderDetail.ProductName} không cho phép hoàn trả");
                }

                // Check available quantity for return
                var alreadyReturnedQty = await _orderReturnItemRepository.Query()
                    .Where(ri => ri.OrderDetailId == returnItem.OrderDetailId &&
                               ri.OrderReturn.Status != EOrderReturnStatus.Rejected)
                    .SumAsync(ri => ri.Quantity);

                var availableQty = orderDetail.Quantity - alreadyReturnedQty;

                if (returnItem.Quantity > availableQty)
                {
                    return (false, $"Số lượng hoàn trả vượt quá số lượng có thể hoàn cho sản phẩm {orderDetail.ProductName}");
                }

                if (returnItem.Quantity <= 0)
                {
                    return (false, "Số lượng hoàn trả phải lớn hơn 0");
                }
            }

            return (true, string.Empty);
        }

        private async Task<string> GenerateReturnCodeAsync()
        {
            var date = DateTime.Now.ToString("yyyyMMdd");
            var count = await _orderReturnRepository.Query()
                .Where(or => or.CreationTime.Date == DateTime.Now.Date)
                .CountAsync();

            return $"RT{date}{(count + 1):D4}";
        }

        private decimal CalculateRefundAmount(ICollection<OrderReturnItem> items)
        {
            return items.Sum(i => (decimal)((i.OrderDetail?.Price ?? 0f) * i.Quantity));
        }

        private string GetContentTypeFromFileName(string fileName)
        {
            var extension = System.IO.Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".mp4" => "video/mp4",
                ".avi" => "video/avi",
                ".mov" => "video/quicktime",
                ".wmv" => "video/x-ms-wmv",
                _ => "application/octet-stream"
            };
        }

        private string GetStatusDisplayName(EOrderReturnStatus status)
        {
            return status switch
            {
                EOrderReturnStatus.Pending => "Chờ duyệt",
                EOrderReturnStatus.Approved => "Đã duyệt",
                EOrderReturnStatus.Rejected => "Từ chối",
                EOrderReturnStatus.Received => "Đã nhận hàng",
                EOrderReturnStatus.Refunded => "Đã hoàn tiền",
                _ => status.ToString()
            };
        }

        private string GetRefundMethodDisplayName(ERefundMethod method)
        {
            return method switch
            {
                ERefundMethod.BankTransfer => "Chuyển khoản",
                ERefundMethod.InStore => "Nhận tại cửa hàng",
                _ => method.ToString()
            };
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

        private async Task AddOrderHistoryAsync(Guid orderId, string content, EHistoryType type = EHistoryType.Update)
        {
            var orderHistory = new OrderHistory
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                Content = content,
                Type = type,
                CreationTime = DateTime.Now,
                CreatedBy = Guid.Empty // System generated
            };

            await _orderHistoryRepository.AddAsync(orderHistory);
        }

        private async Task BackInventoryForReturnAsync(Guid orderReturnId)
        {
            // Get return items
            var returnItems = await _orderReturnItemRepository.Query()
                .Where(ri => ri.OrderReturnId == orderReturnId)
                .Include(ri => ri.OrderDetail)
                .ToListAsync();

            foreach (var returnItem in returnItems)
            {
                // Find the first available batch for this product detail
                var availableBatch = await _inventoryBatchRepository.Query()
                    .Where(ib => ib.ProductDetailId == returnItem.OrderDetail.ProductDetailId &&
                                 ib.Status == EInventoryBatchStatus.Approved)
                    .OrderBy(ib => ib.CreationTime)
                    .FirstOrDefaultAsync();

                if (availableBatch != null)
                {
                    // Add quantity back to inventory
                    availableBatch.Quantity += returnItem.Quantity;
                    await _inventoryBatchRepository.UpdateAsync(availableBatch);

                    // Create inventory transaction
                    await _inventoryTransactionRepository.AddAsync(new InventoryTransaction
                    {
                        Id = Guid.NewGuid(),
                        InventoryBatchId = availableBatch.Id,
                        Quantity = returnItem.Quantity,
                        CreationTime = DateTime.Now,
                        Type = EInventoryTranctionType.Import
                    });
                }
            }
        }

        private async Task DecreaseSellNumberForReturnAsync(Guid orderReturnId)
        {
            // Get return items
            var returnItems = await _orderReturnItemRepository.Query()
                .Where(ri => ri.OrderReturnId == orderReturnId)
                .Include(ri => ri.OrderDetail)
                .ToListAsync();

            // Group by ProductDetailId to get total quantity per product
            var productDetailGroups = returnItems
                .GroupBy(ri => ri.OrderDetail.ProductDetailId)
                .Select(g => new { ProductDetailId = g.Key, TotalQuantity = g.Sum(ri => ri.Quantity) })
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
}