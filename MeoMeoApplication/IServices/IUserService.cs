using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Employees;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeoMeo.Contract.DTOs.Auth;

namespace MeoMeo.Application.IServices
{
    public interface IUserService
    {
        Task<List<User>> GetAllUserAsync();
        Task<UserDTO> GetUserByIdAsync(Guid id);
        Task<User> CreateUserAsync(CreateOrUpdateUserDTO user);
        Task<CreateOrUpdateUserResponseDTO> UpdateUserAsync(CreateOrUpdateUserDTO user);
        Task<bool> DeleteUserAsync(Guid id);
    }
}
