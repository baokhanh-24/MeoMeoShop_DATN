using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface ICustomersBankRepository
    {
        Task<CustomersBank> CreateCustomersBankAsync(CustomersBank customersBank);
        Task<List<CustomersBank>> GetAllCustomersBankAsync();
        Task<CustomersBank> GetCustomersBankByIdAsync(Guid id);
        Task<CustomersBank> UpdateCustomersBankAsync(CustomersBank customersBank);
        Task<bool> DeleteCustomersBankAsync(Guid id);
    }
}
