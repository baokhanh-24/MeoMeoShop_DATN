using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;

namespace MeoMeo.Shared.IServices
{
    public interface IVoucherClientService
    {
        Task<PagingExtensions.PagedResult<VoucherDTO>> GetAllVoucherAsync(GetListVoucherRequestDTO request);
        Task<CreateOrUpdateVoucherResponseDTO> GetVoucherByIdAsync(Guid id);
        Task<CreateOrUpdateVoucherResponseDTO> CreateVoucherAsync(CreateOrUpdateVoucherDTO voucher);
        Task<CreateOrUpdateVoucherResponseDTO> UpdateVoucherAsync(CreateOrUpdateVoucherDTO voucher);
        Task<bool> DeleteVoucherAsync(Guid id);
        Task<CheckVoucherResponseDTO> CheckVoucherAsync(CheckVoucherRequestDTO request);
        Task<List<AvailableVoucherDTO>> GetAvailableVouchersAsync(GetAvailableVouchersRequestDTO request);
    }
}
