using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<User> CreateUserAsync(User user)
        {
            var userAdded = await AddAsync(user);
            return userAdded;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            await DeleteAsync(id);
            return true;
        }

        public async Task<List<User>> GetAllUserAsync()
        {
            var getAllUser = await GetAllAsync();
            return getAllUser.ToList();
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            var userUpdated = await UpdateAsync(user);
            return userUpdated;
        }
    }
}
