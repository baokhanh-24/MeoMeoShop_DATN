using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using Microsoft.AspNetCore.Http;

namespace MeoMeo.Shared.IServices;

public interface ICustomerClientService
{
    Task<PagingExtensions.PagedResult<CustomerDTO>> GetAllCustomersAsync(GetListCustomerRequestDTO request);
    Task<CustomerDTO> GetCustomersByIdAsync(Guid id);
    Task<CreateOrUpdateCustomerResponse> CreateCustomersAsync(CreateOrUpdateCustomerDTO customer);
    Task<CreateOrUpdateCustomerResponse> UpdateCustomersAsync(CreateOrUpdateCustomerDTO customer);
    Task<BaseResponse> DeleteCustomersAsync(Guid id);
    Task<BaseResponse> UploadAvatarAsync(IFormFile file);
    Task<BaseResponse> ChangePasswordAsync(ChangePasswordDTO model);
    Task<CreateOrUpdateCustomerResponse> UpdateProfileAsync(CreateOrUpdateCustomerDTO createOrUpdateCustomerDto);
    Task<QuickCustomerResponseDTO> CreateQuickCustomerAsync(CreateQuickCustomerDTO request);

}