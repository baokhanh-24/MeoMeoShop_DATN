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
    }
}
