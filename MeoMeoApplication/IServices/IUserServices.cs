using MeoMeo.Contract.DTOs;
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
        Task<User> GetUserByIdAsync(Guid id);
        Task<User> CreateUserAsync(CreateOrUpdateUserDTO user);
        Task<User> UpdateUserAsync(CreateOrUpdateUserDTO user);
        Task<bool> DeleteUserAsync(Guid id);
    }
}
