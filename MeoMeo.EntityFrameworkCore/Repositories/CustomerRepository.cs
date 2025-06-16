using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class CustomerRepository : BaseRepository<Customers>, ICustomerRepository
    {
        public CustomerRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<Customers> CreateCustomersAsync(Customers customers)
        {
            var customersAdded = await AddAsync(customers);
            return customersAdded;
        }

        public async Task<bool> DeleteCustomersAsync(Guid id)
        {
            await DeleteAsync(id);
            return true;
        }

        public async Task<List<Customers>> GetAllCustomersAsync()
        {
            var getAllCustomers = await GetAllAsync();
            return getAllCustomers.ToList();
        }

        public async Task<Customers> GetCustomersByIdAsync(Guid id)
        {
            var customers = await GetByIdAsync(id);
            return customers;
        }

        public async Task<Customers> UpdateCustomersAsync(Customers customers)
        {
            var customersUpdated = await UpdateAsync(customers);
            return customersUpdated;
        }
    }
}
