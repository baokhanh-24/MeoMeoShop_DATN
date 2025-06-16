using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface ICustomerServices
    {
        Task<List<Customers>> GetAllCustomersAsync();
        Task<Customers> GetCustomersByIdAsync(Guid id);
        Task<Customers> CreateCustomersAsync(CreateOrUpdateCustomerDTO customer);
        Task<Customers> UpdateCustomersAsync(CreateOrUpdateCustomerDTO customer);
        Task<bool> DeleteCustomersAsync(Guid id);
    }
}
