using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Order.Return;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Commons.Enums;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace MeoMeo.Application.Services
{
    public class OrderReturnService : IOrderReturnService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderReturnRepository _orderReturnRepository;
        private readonly IOrderReturnItemRepository _orderReturnItemRepository;
        private readonly IOrderReturnFileRepository _orderReturnFileRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrderReturnService(IOrderRepository orderRepository,
            IOrderReturnRepository orderReturnRepository,
            IOrderReturnItemRepository orderReturnItemRepository,
            IOrderReturnFileRepository orderReturnFileRepository,
            IOrderDetailRepository orderDetailRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _orderReturnRepository = orderReturnRepository;
            _orderReturnItemRepository = orderReturnItemRepository;
            _orderReturnFileRepository = orderReturnFileRepository;
            _orderDetailRepository = orderDetailRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateOrderReturnResponseDTO> CreateAsync(CreateOrderReturnRequestDTO request, Guid currentCustomerId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var order = await _orderRepository.GetByIdAsync(request.OrderId);
                if (order == null)
                {
                    return new CreateOrderReturnResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy đơn hàng" };
                }
                if (order.Status != EOrderStatus.Completed || order.ReceiveDate == null)
                {
                    return new CreateOrderReturnResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Đơn hàng chưa hoàn thành hoặc chưa xác nhận ngày nhận" };
                }
                if ((DateTime.Now - order.ReceiveDate.Value).TotalDays > 5)
                {
                    return new CreateOrderReturnResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Quá thời hạn 5 ngày để hoàn hàng" };
                }
                var existingPending = await _orderReturnRepository.Query()
                    .Where(x => x.OrderId == order.Id && x.Status == EOrderReturnStatus.Pending)
                    .FirstOrDefaultAsync();
                if (existingPending != null)
                {
                    return new CreateOrderReturnResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Đơn hàng đã có yêu cầu hoàn đang chờ duyệt" };
                }

                var code = $"RTN-{DateTime.Now:yyyyMMddHHmmss}";
                var orderReturn = new OrderReturn
                {
                    OrderId = order.Id,
                    CustomerId = order.CustomerId,
                    Code = code,
                    Reason = request.Reason,
                    Status = EOrderReturnStatus.Pending,
                    RefundMethod = request.RefundMethod,
                    BankName = request.BankName,
                    BankAccountName = request.BankAccountName,
                    BankAccountNumber = request.BankAccountNumber,
                    ContactName = request.ContactName,
                    ContactPhone = request.ContactPhone,
                    CreationTime = DateTime.Now
                };
                orderReturn = await _orderReturnRepository.AddAsync(orderReturn);

                var items = request.Items.Select(i => new OrderReturnItem
                {
                    OrderReturnId = orderReturn.Id,
                    OrderDetailId = i.OrderDetailId,
                    Quantity = i.Quantity,
                    Reason = i.Reason
                }).ToList();
                if (items.Any())
                {
                    await _orderReturnItemRepository.AddRangeAsync(items);
                }
                var files = request.Files.Select(f => new OrderReturnFile
                {
                    OrderReturnId = orderReturn.Id,
                    Url = f.Url,
                    Name = f.Name,
                    ContentType = f.ContentType
                }).ToList();
                if (files.Any())
                {
                    await _orderReturnFileRepository.AddRangeAsync(files);
                }

                await _unitOfWork.CommitAsync();
                return new CreateOrderReturnResponseDTO { ResponseStatus = BaseStatus.Success, OrderReturnId = orderReturn.Id, Code = orderReturn.Code };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new CreateOrderReturnResponseDTO { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<UpdateOrderReturnStatusResponseDTO> UpdateStatusAsync(UpdateOrderReturnStatusRequestDTO request)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var orderReturn = await _orderReturnRepository.GetByIdAsync(request.OrderReturnId);
                if (orderReturn == null)
                {
                    return new UpdateOrderReturnStatusResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy yêu cầu hoàn hàng" };
                }
                orderReturn.Status = request.Status;
                orderReturn.LastModifiedTime = DateTime.Now;
                await _orderReturnRepository.UpdateAsync(orderReturn);
                await _unitOfWork.CommitAsync();
                return new UpdateOrderReturnStatusResponseDTO { ResponseStatus = BaseStatus.Success };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new UpdateOrderReturnStatusResponseDTO { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<OrderReturnViewDTO?> GetByIdAsync(Guid id)
        {
            var entity = await _orderReturnRepository.GetByIdAsync(id);
            if (entity == null) return null;
            var items = await _orderReturnItemRepository.Query().Where(x => x.OrderReturnId == id).ToListAsync();
            var files = await _orderReturnFileRepository.Query().Where(x => x.OrderReturnId == id).ToListAsync();
            return new OrderReturnViewDTO
            {
                Id = entity.Id,
                OrderId = entity.OrderId,
                CustomerId = entity.CustomerId,
                Code = entity.Code,
                Reason = entity.Reason,
                Status = entity.Status,
                RefundMethod = entity.RefundMethod,
                BankName = entity.BankName,
                BankAccountName = entity.BankAccountName,
                BankAccountNumber = entity.BankAccountNumber,
                ContactName = entity.ContactName,
                ContactPhone = entity.ContactPhone,
                CreationTime = entity.CreationTime,
                LastModifiedTime = entity.LastModifiedTime,
                Items = items.Select(i => new OrderReturnItemViewDTO
                {
                    Id = i.Id,
                    OrderDetailId = i.OrderDetailId,
                    Quantity = i.Quantity,
                    Reason = i.Reason
                }).ToList(),
                Files = files.Select(f => new OrderReturnFileViewDTO
                {
                    Id = f.Id,
                    Url = f.Url,
                    Name = f.Name,
                    ContentType = f.ContentType
                }).ToList()
            };
        }

        public async Task<List<OrderReturnViewDTO>> GetByOrderIdAsync(Guid orderId)
        {
            var list = await _orderReturnRepository.Query().Where(x => x.OrderId == orderId).OrderByDescending(x => x.CreationTime).ToListAsync();
            var result = new List<OrderReturnViewDTO>();
            foreach (var entity in list)
            {
                var items = await _orderReturnItemRepository.Query().Where(x => x.OrderReturnId == entity.Id).ToListAsync();
                var files = await _orderReturnFileRepository.Query().Where(x => x.OrderReturnId == entity.Id).ToListAsync();
                result.Add(new OrderReturnViewDTO
                {
                    Id = entity.Id,
                    OrderId = entity.OrderId,
                    CustomerId = entity.CustomerId,
                    Code = entity.Code,
                    Reason = entity.Reason,
                    Status = entity.Status,
                    RefundMethod = entity.RefundMethod,
                    BankName = entity.BankName,
                    BankAccountName = entity.BankAccountName,
                    BankAccountNumber = entity.BankAccountNumber,
                    ContactName = entity.ContactName,
                    ContactPhone = entity.ContactPhone,
                    CreationTime = entity.CreationTime,
                    LastModifiedTime = entity.LastModifiedTime,
                    Items = items.Select(i => new OrderReturnItemViewDTO
                    {
                        Id = i.Id,
                        OrderDetailId = i.OrderDetailId,
                        Quantity = i.Quantity,
                        Reason = i.Reason
                    }).ToList(),
                    Files = files.Select(f => new OrderReturnFileViewDTO
                    {
                        Id = f.Id,
                        Url = f.Url,
                        Name = f.Name,
                        ContentType = f.ContentType
                    }).ToList()
                });
            }
            return result;
        }
    }
}


