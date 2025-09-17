using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Commons.Enums;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MeoMeo.Application.Services
{
    public class VoucherServices : IVoucherService
    {
        private readonly IVoucherRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public VoucherServices(IVoucherRepository repository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateOrUpdateVoucherResponseDTO> CreateVoucherAsync(CreateOrUpdateVoucherDTO voucher)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var entity = _mapper.Map<Voucher>(voucher);
                entity.Id = Guid.NewGuid();
                entity.CreationTime = DateTime.Now;

                await _repository.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return _mapper.Map<CreateOrUpdateVoucherResponseDTO>(entity);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new CreateOrUpdateVoucherResponseDTO
                {
                    Message = ex.Message,
                    ResponseStatus = BaseStatus.Error
                };
            }
        }

        public async Task<bool> DeleteVoucherAsync(Guid id)
        {
            var voucher = await _repository.GetVoucherByIdAsync(id);
            if (voucher == null)
            {
                return false;
            }
            await _repository.DeleteAsync(id);

            return true;
        }

        public async Task<PagingExtensions.PagedResult<VoucherDTO>> GetAllVoucherAsync(GetListVoucherRequestDTO request)
        {
            var query = _repository.Query();

            // Filter theo Code
            if (!string.IsNullOrEmpty(request.CodeFilter))
            {
                query = query.Where(v => EF.Functions.Like(v.Code, $"%{request.CodeFilter}%"));
            }

            // Filter theo Type
            if (request.TypeFilter != null)
            {
                query = query.Where(v => v.Type == request.TypeFilter);
            }

            // Filter theo khoảng thời gian bắt đầu
            if (request.StartDateFromFilter != null)
            {
                query = query.Where(v => v.StartDate >= request.StartDateFromFilter);
            }
            if (request.StartDateToFilter != null)
            {
                query = query.Where(v => v.StartDate <= request.StartDateToFilter);
            }

            // Filter theo khoảng thời gian kết thúc
            if (request.EndDateFromFilter != null)
            {
                query = query.Where(v => v.EndDate >= request.EndDateFromFilter);
            }
            if (request.EndDateToFilter != null)
            {
                query = query.Where(v => v.EndDate <= request.EndDateToFilter);
            }

            // Filter theo trạng thái (Status)
            if (request.Status != null)
            {
                var now = DateTime.Now;
                switch (request.Status)
                {
                    case EVoucherStatus.Upcoming:
                        query = query.Where(v => v.StartDate > now);
                        break;
                    case EVoucherStatus.Active:
                        query = query.Where(v => v.StartDate <= now && v.EndDate >= now);
                        break;
                    case EVoucherStatus.Expired:
                        query = query.Where(v => v.EndDate < now);
                        break;
                    case EVoucherStatus.All:
                    default:
                        // Không lọc gì thêm nếu là All hoặc null
                        break;
                }
            }

            // Sắp xếp giảm dần theo thời gian tạo
            query = query.OrderByDescending(v => v.CreationTime);

            var pagedResult = await _repository.GetPagedAsync(query, request.PageIndex, request.PageSize);

            var dtoList = _mapper.Map<List<VoucherDTO>>(pagedResult.Items);

            return new PagingExtensions.PagedResult<VoucherDTO>
            {
                TotalRecords = pagedResult.TotalRecords,
                PageIndex = pagedResult.PageIndex,
                PageSize = pagedResult.PageSize,
                Items = dtoList
            };

        }



        public async Task<CreateOrUpdateVoucherResponseDTO> GetVoucherByIdAsync(Guid id)
        {
            var voucher = await _repository.GetByIdAsync(id);

            return _mapper.Map<CreateOrUpdateVoucherResponseDTO>(voucher);
        }

        public async Task<CreateOrUpdateVoucherResponseDTO> UpdateVoucherAsync(CreateOrUpdateVoucherDTO voucher)
        {
            var existing = await _repository.GetByIdAsync(voucher.Id.Value);
            if (existing == null)
            {
                return new CreateOrUpdateVoucherResponseDTO
                {
                    Message = "Voucher không tồn tại",
                    ResponseStatus = BaseStatus.Error,
                };
            }

            _mapper.Map(voucher, existing);
            existing.LastModificationTime = DateTime.Now;

            var updated = await _repository.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CreateOrUpdateVoucherResponseDTO>(updated);
        }

        public async Task<CheckVoucherResponseDTO> CheckVoucherAsync(CheckVoucherRequestDTO request)
        {
            try
            {
                var voucher = await _repository.Query()
                    .Include(v => v.Orders)
                    .FirstOrDefaultAsync(v => v.Code == request.Code);

                if (voucher == null)
                {
                    return new CheckVoucherResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Mã voucher không tồn tại"
                    };
                }

                var now = DateTime.Now;
                if (now < voucher.StartDate)
                {
                    return new CheckVoucherResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Voucher chưa có hiệu lực"
                    };
                }

                if (now > voucher.EndDate)
                {
                    return new CheckVoucherResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Voucher đã hết hạn"
                    };
                }

                if (request.OrderAmount < voucher.MinOrder)
                {
                    return new CheckVoucherResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = $"Đơn hàng cần tối thiểu {voucher.MinOrder:N0} đ để áp dụng voucher này"
                    };
                }

                // Check MaxTotalUse limit
                if (voucher.MaxTotalUse.HasValue)
                {
                    var totalUsageCount = voucher.Orders?.Count ?? 0;
                    if (totalUsageCount >= voucher.MaxTotalUse.Value)
                    {
                        return new CheckVoucherResponseDTO
                        {
                            ResponseStatus = BaseStatus.Error,
                            Message = "Voucher này đã hết lượt sử dụng"
                        };
                    }
                }

                // Calculate discount amount
                decimal discountAmount = 0;
                if (voucher.Type == EVoucherType.byPercentage)
                {
                    discountAmount = request.OrderAmount * (decimal)voucher.Discount / 100m;
                    if (voucher.MaxDiscount > 0 && discountAmount > (decimal)voucher.MaxDiscount)
                    {
                        discountAmount = (decimal)voucher.MaxDiscount;
                    }
                }
                else
                {
                    discountAmount = (decimal)voucher.Discount;
                    if (discountAmount > request.OrderAmount)
                    {
                        discountAmount = request.OrderAmount;
                    }
                }

                return new CheckVoucherResponseDTO
                {
                    ResponseStatus = BaseStatus.Success,
                    Message = "Voucher hợp lệ",
                    Id = voucher.Id,
                    Code = voucher.Code,
                    DiscountAmount = discountAmount,
                    Discount = voucher.Discount,
                    MinOrder = voucher.MinOrder,
                    MaxDiscount = voucher.MaxDiscount,
                    DiscountPercent = (decimal)(voucher.Type == EVoucherType.byPercentage ? voucher.Discount : 0),
                    MinOrderAmount = voucher.MinOrder,
                    ExpiryDate = voucher.EndDate
                };
            }
            catch (Exception ex)
            {
                return new CheckVoucherResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Lỗi khi kiểm tra voucher: {ex.Message}"
                };
            }
        }

        public async Task<List<AvailableVoucherDTO>> GetAvailableVouchersAsync(GetAvailableVouchersRequestDTO request)
        {
            try
            {
                var now = DateTime.Now;
                var vouchers = await _repository.Query()
                    .Include(v => v.Orders)
                    .Where(v => v.StartDate <= now && v.EndDate >= now)
                    .ToListAsync();

                var availableVouchers = new List<AvailableVoucherDTO>();

                foreach (var voucher in vouchers)
                {
                    var canUse = true;
                    string? reason = null;

                    // Check minimum order amount
                    if (request.OrderAmount < voucher.MinOrder)
                    {
                        canUse = false;
                        reason = $"Đơn hàng cần tối thiểu {voucher.MinOrder:N0} đ";
                    }

                    // Check MaxTotalUse limit
                    if (canUse && voucher.MaxTotalUse.HasValue)
                    {
                        var totalUsageCount = voucher.Orders?.Count ?? 0;
                        if (totalUsageCount >= voucher.MaxTotalUse.Value)
                        {
                            canUse = false;
                            reason = "Voucher đã hết lượt sử dụng";
                        }
                    }

                    // Check MaxTotalUsePerCustomer limit
                    if (canUse && voucher.MaxTotalUsePerCustomer.HasValue)
                    {
                        var customerUsageCount = voucher.Orders?.Count(o => o.CustomerId == request.CustomerId) ?? 0;
                        if (customerUsageCount >= voucher.MaxTotalUsePerCustomer.Value)
                        {
                            canUse = false;
                            reason = "Bạn đã sử dụng hết lượt áp dụng voucher này";
                        }
                    }

                    // Calculate discount amount
                    decimal calculatedDiscountAmount = 0;
                    if (canUse)
                    {
                        if (voucher.Type == EVoucherType.byPercentage)
                        {
                            calculatedDiscountAmount = request.OrderAmount * (decimal)voucher.Discount / 100m;
                            if (voucher.MaxDiscount > 0 && calculatedDiscountAmount > (decimal)voucher.MaxDiscount)
                            {
                                calculatedDiscountAmount = (decimal)voucher.MaxDiscount;
                            }
                        }
                        else
                        {
                            calculatedDiscountAmount = (decimal)voucher.Discount;
                            if (calculatedDiscountAmount > request.OrderAmount)
                            {
                                calculatedDiscountAmount = request.OrderAmount;
                            }
                        }
                    }

                    availableVouchers.Add(new AvailableVoucherDTO
                    {
                        Id = voucher.Id,
                        Code = voucher.Code,
                        Name = voucher.Code, // Có thể thêm field Name vào Voucher entity
                        Discount = voucher.Discount,
                        MinOrder = voucher.MinOrder,
                        MaxDiscount = voucher.MaxDiscount,
                        Type = voucher.Type,
                        StartDate = voucher.StartDate,
                        EndDate = voucher.EndDate,
                        MaxTotalUse = voucher.MaxTotalUse,
                        MaxTotalUsePerCustomer = voucher.MaxTotalUsePerCustomer,
                        CalculatedDiscountAmount = calculatedDiscountAmount,
                        CanUse = canUse,
                        Reason = reason
                    });
                }

                // Sort by discount amount descending (highest discount first)
                return availableVouchers.OrderByDescending(v => v.CalculatedDiscountAmount).ToList();
            }
            catch (Exception ex)
            {
                return new List<AvailableVoucherDTO>();
            }
        }
    }
}
