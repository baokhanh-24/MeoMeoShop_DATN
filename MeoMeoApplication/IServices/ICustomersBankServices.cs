using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface ICustomersBankServices
    {
        Task<List<CustomersBank>> GetAllCustomersBankAsync();
        Task<CustomersBank> GetCustomersBankByIdAsync(Guid id);
        Task<CustomersBank> CreateCustomersBankAsync(CreateOrUpdateCustomersBankDTO customersBank);
        Task<CustomersBank> UpdateCustomersBankAsync(CreateOrUpdateCustomersBankDTO customersBank);
        Task<bool> DeleteCustomersBankAsync(Guid id);
    }
}
