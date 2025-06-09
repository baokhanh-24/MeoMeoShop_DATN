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

        public async Task<bool> DeleteVoucherAsync(Voucher voucher)
        {
            try
            {
                var voucherDeleted = _context.vouchers.Remove(voucher);

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
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
            var voucherUpdated = _context.vouchers.Update(voucher);

            await _context.SaveChangesAsync();

            return voucher;
        }
    }
}
