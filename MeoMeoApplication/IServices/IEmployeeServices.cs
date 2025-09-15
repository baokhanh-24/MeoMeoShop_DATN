using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MeoMeo.Application.IServices
{
    public interface IEmployeeServices
    {
        Task<PagingExtensions.PagedResult<CreateOrUpdateEmployeeDTO>> GetAllEmployeeAsync(GetlistEmployeesRequestDTO requestDTO);
        Task<CreateOrUpdateEmployeeResponseDTO> GetEmployeeByIdAsync(Guid id);
        Task<CreateOrUpdateEmployeeResponseDTO> CreateEmployeeAsync(CreateOrUpdateEmployeeDTO employee);
        Task<CreateOrUpdateEmployeeResponseDTO> UpdateEmployeeAsync(CreateOrUpdateEmployeeDTO employee);
        Task<bool> DeleteEmployeeAsync(Guid id);
        Task<BaseResponse> UploadAvatarAsync(Guid userId, FileUploadResult uploadResult);
        Task<string> GetOldUrlAvatar(Guid userId);
        Task<BaseResponse> ChangePasswordAsync(Guid userId, ChangePasswordDTO changePasswordDTO);
        Task<CreateOrUpdateEmployeeResponseDTO> UpdateProfileAsync(CreateOrUpdateEmployeeDTO employee);
    }
}
