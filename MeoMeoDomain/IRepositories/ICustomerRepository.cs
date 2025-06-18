using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface ICustomerRepository : IBaseRepository<Customers>
    {
        Task<Customers> CreateCustomersAsync(Customers customers);
        Task<List<Customers>> GetAllCustomersAsync();
        Task<Customers> GetCustomersByIdAsync(Guid id);
        Task<Customers> UpdateCustomersAsync(Customers customers);
        Task<bool> DeleteCustomersAsync(Guid id);
    }
}
