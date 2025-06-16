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
    public class CustomersBankRepository : BaseRepository<CustomersBank>, ICustomersBankRepository
    {
        public CustomersBankRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<CustomersBank> CreateCustomersBankAsync(CustomersBank customersBank)
        {
            var customersBankAdded = await AddAsync(customersBank);
            return customersBankAdded;
        }

        public async Task<bool> DeleteCustomersBankAsync(Guid id)
        {
            await DeleteAsync(id);
            return true;
        }

        public async Task<List<CustomersBank>> GetAllCustomersBankAsync()
        {
            var getAllCustomersBank = await GetAllAsync();
            return getAllCustomersBank.ToList();
        }

        public async Task<CustomersBank> GetCustomersBankByIdAsync(Guid id)
        {
            var customersBank = await GetByIdAsync(id);
            return customersBank;
        }

        public async Task<CustomersBank> UpdateCustomersBankAsync(CustomersBank customersBank)
        {
            var customersBankUpdated = await UpdateAsync(customersBank);
            return customersBankUpdated;
        }
    }
}
