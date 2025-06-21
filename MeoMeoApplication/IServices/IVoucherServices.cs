using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IVoucherServices
    {
        Task<List<Voucher>> GetAllVoucherAsync();
        Task<CreateOrUpdateVoucherResponseDTO> GetVoucherByIdAsync(Guid id);
        Task<Voucher> CreateVoucherAsync(CreateOrUpdateVoucherDTO voucher);
        Task<CreateOrUpdateVoucherResponseDTO> UpdateVoucherAsync(CreateOrUpdateVoucherDTO voucher);
        Task<bool> DeleteVoucherAsync(Guid id);
    }
}
