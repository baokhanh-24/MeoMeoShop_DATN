using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons;

namespace MeoMeo.Application.IServices
{
    public interface ICustomerServices
    {
        Task<PagingExtensions.PagedResult<CustomerDTO>> GetAllCustomersAsync(GetListCustomerRequestDTO request);
        Task<CustomerDTO> GetCustomersByIdAsync(Guid id);
        Task<CreateOrUpdateCustomerResponse> CreateCustomersAsync(CreateOrUpdateCustomerDTO customer);
        Task<CreateOrUpdateCustomerResponse> UpdateCustomersAsync(CreateOrUpdateCustomerDTO customer);
        Task<bool> DeleteCustomersAsync(Guid id);
        Task<BaseResponse> UploadAvatarAsync(Guid userId, FileUploadResult file);
        Task<BaseResponse> ChangePasswordAsync(Guid userId, ChangePasswordDTO request);
        Task<string> GetOldUrlAvatar(Guid userId);
        Task<QuickCustomerResponseDTO> CreateQuickCustomerAsync(CreateQuickCustomerDTO request);
    }
}
