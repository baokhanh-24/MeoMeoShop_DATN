using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons.Enums;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserServices(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<User> CreateUserAsync(CreateOrUpdateUserDTO user)
        {
            var mappedUser = _mapper.Map<User>(user);
            mappedUser.Id = Guid.NewGuid();
            return await _repository.CreateUserAsync(mappedUser);
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var userToDelete = await _repository.GetUserByIdAsync(id);

            if (userToDelete == null)
            {
                return false;
            }

            await _repository.DeleteUserAsync(userToDelete.Id);
            return true;
        }

        public async Task<List<User>> GetAllUserAsync()
        {
            return await _repository.GetAllUserAsync();
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            return await _repository.GetUserByIdAsync(id);
        }

        public async Task<User> UpdateUserAsync(CreateOrUpdateUserDTO user)
        {
            User userCheck = new User();

            userCheck = _mapper.Map<User>(user);

            var result = await _repository.UpdateUserAsync(userCheck);

            return result;
        }
    }
}
