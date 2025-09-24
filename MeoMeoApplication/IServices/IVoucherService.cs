using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IVoucherService
    {
        Task<PagingExtensions.PagedResult<VoucherDTO>> GetAllVoucherAsync(GetListVoucherRequestDTO request);
        Task<CreateOrUpdateVoucherResponseDTO> GetVoucherByIdAsync(Guid id);
        Task<CreateOrUpdateVoucherResponseDTO> CreateVoucherAsync(CreateOrUpdateVoucherDTO voucher);
        Task<CreateOrUpdateVoucherResponseDTO> UpdateVoucherAsync(CreateOrUpdateVoucherDTO voucher);
        Task<bool> DeleteVoucherAsync(Guid id);
        Task<CheckVoucherResponseDTO> CheckVoucherAsync(CheckVoucherRequestDTO request);
        Task<List<AvailableVoucherDTO>> GetAvailableVouchersAsync(GetAvailableVouchersRequestDTO request);
        Task<string> GenerateUniqueVoucherCodeAsync();
    }
}
