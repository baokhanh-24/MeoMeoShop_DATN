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
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrderReturnService(
            IOrderReturnRepository orderReturnRepository,
            IOrderReturnItemRepository orderReturnItemRepository,
            IOrderReturnFileRepository orderReturnFileRepository,
            IOrderRepository orderRepository,
            IOrderDetailRepository orderDetailRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _orderReturnRepository = orderReturnRepository;
            _orderReturnItemRepository = orderReturnItemRepository;
            _orderReturnFileRepository = orderReturnFileRepository;
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
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
                var orderReturn = await _orderReturnRepository.GetByIdAsync(id);
                if (orderReturn == null)
                {
                    return new BaseResponse
                    {
                        Message = "Không tìm thấy yêu cầu hoàn trả",
                        ResponseStatus = BaseStatus.Error
                    };
                }

                orderReturn.Status = request.Status;
                orderReturn.LastModifiedTime = DateTime.Now;

                await _orderReturnRepository.UpdateAsync(orderReturn);
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
                var orderReturn = await _orderReturnRepository.GetByIdAsync(id);
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

                await _orderReturnRepository.UpdateAsync(orderReturn);
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

                await _orderReturnRepository.UpdateAsync(orderReturn);
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
                    result.Add(new OrderReturnItemDetailDTO
                    {
                        OrderDetailId = orderDetail.Id,
                        ProductName = orderDetail.ProductName,
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

            // Check if within return period (e.g., 7 days)
            var returnPeriodDays = 7;
            if (order.ReceiveDate.HasValue)
            {
                var daysSinceReceived = (DateTime.Now - order.ReceiveDate.Value).Days;
                if (daysSinceReceived > returnPeriodDays)
                    return false;
            }
            else
            {
                // If no receive date, check creation time
                var daysSinceCreated = (DateTime.Now - order.CreationTime).Days;
                if (daysSinceCreated > returnPeriodDays)
                    return false;
            }

            // Check if at least one product in the order allows return
            var hasReturnableProduct = order.OrderDetails.Any(od => od.ProductDetail?.AllowReturn == true);
            return hasReturnableProduct;
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
                ERefundMethod.ViaShipper => "Nhận qua ship",
                ERefundMethod.InStore => "Nhận tại cửa hàng",
                _ => method.ToString()
            };
        }
    }
}