using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface IVoucherRepository : IBaseRepository<Voucher>
    {
        Task<List<Voucher>> GetAllVoucherAsync();
        Task<Voucher> GetVoucherByIdAsync(Guid id);
        Task<Voucher> UpdateVoucherAsync(Voucher voucher);
        Task<bool> DeleteVoucherAsync(Voucher voucher);
    }
}
