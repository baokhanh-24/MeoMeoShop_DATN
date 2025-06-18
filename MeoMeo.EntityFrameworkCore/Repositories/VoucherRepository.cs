using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class VoucherRepository : BaseRepository<Voucher>, IVoucherRepository
    {
        public VoucherRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<Voucher> CreateVoucherAsync(Voucher voucher)
        {
            var voucherAdded = await AddAsync(voucher);
            return voucherAdded;    
        }

        public async Task<bool> DeleteVoucherAsync(Guid id)
        {
            await DeleteAsync(id);
            return true;
        }

        public async Task<List<Voucher>> GetAllVoucherAsync()
        {
            var getAllVoucher = await GetAllAsync();
            return getAllVoucher.ToList();
        }

        public async Task<Voucher> GetVoucherByIdAsync(Guid id)
        {
            var voucher = await GetByIdAsync(id);
            return voucher;
        }

        public async Task<Voucher> UpdateVoucherAsync(Voucher voucher)
        {
            var voucherUpdated = await UpdateAsync(voucher);
            return voucherUpdated;
        }
    }
}
