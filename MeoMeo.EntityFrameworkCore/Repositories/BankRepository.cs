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
    public class BankRepository : BaseRepository<Bank>, IBankRepository
    {
        public BankRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<Bank> CreateBankAsync(Bank bank)
        {
            var bankAdded = await AddAsync(bank);
            return bankAdded;
        }

        public async Task<bool> DeleteBankAsync(Guid id)
        {
            await DeleteAsync(id);
            return true;
        }

        public async Task<List<Bank>> GetAllBankAsync()
        {
            var getAllBank = await GetAllAsync();
            return getAllBank.ToList();
        }

        public async Task<Bank> GetBankByIdAsync(Guid id)
        {
            var bank = await GetByIdAsync(id);
            return bank;
        }

        public async Task<Bank> UpdateBankAsync(Bank bank)
        {
            var bankUpdated = await UpdateAsync(bank);
            return bankUpdated;
        }
    }
}
