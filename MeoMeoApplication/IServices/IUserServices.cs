using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Employees;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IUserServices
    {
        Task<List<User>> GetAllUserAsync();
        Task<CreateOrUpdateUserResponseDTO> GetUserByIdAsync(Guid id);
        Task<User> CreateUserAsync(CreateOrUpdateUserDTO user);
        Task<CreateOrUpdateUserResponseDTO> UpdateUserAsync(CreateOrUpdateUserDTO user);
        Task<bool> DeleteUserAsync(Guid id);
        Task<CreateOrUpdateUserResponseDTO> ChangePasswordAsync(ChangePasswordRequestDTO request);
    }
}
