using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;

namespace MeoMeo.CMS.IServices;

public interface ICustomerClientService
{
    Task<PagingExtensions.PagedResult<CustomerDTO>> GetAllCustomersAsync(GetListCustomerRequestDTO request);
    Task<CustomerDTO> GetCustomersByIdAsync(Guid id);
    Task<CreateOrUpdateCustomerResponse> CreateCustomersAsync(CreateOrUpdateCustomerDTO customer);
    Task<CreateOrUpdateCustomerResponse> UpdateCustomersAsync(CreateOrUpdateCustomerDTO customer);
    Task<bool> DeleteCustomersAsync(Guid id);
}