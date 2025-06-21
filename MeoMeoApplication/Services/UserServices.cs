using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
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

        public async Task<CreateOrUpdateUserResponseDTO> GetUserByIdAsync(Guid id)
        {
            CreateOrUpdateUserResponseDTO responseDTO = new CreateOrUpdateUserResponseDTO();

            var check = await _repository.GetUserByIdAsync(id);
            if (check == null)
            {
                responseDTO.ResponseStatus = BaseStatus.Error;
                responseDTO.Message = "Không tìm thấy User";
                return responseDTO;
            }

            responseDTO = _mapper.Map<CreateOrUpdateUserResponseDTO>(check);
            responseDTO.ResponseStatus = BaseStatus.Success;
            responseDTO.Message = "";
            return responseDTO;
        }

        public async Task<CreateOrUpdateUserResponseDTO> UpdateUserAsync(CreateOrUpdateUserDTO user)
        {
            var itemUser = await _repository.GetUserByIdAsync(Guid.Parse(user.Id.ToString()));
            if (itemUser == null)
            {
                return new CreateOrUpdateUserResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy user" };
            }
            _mapper.Map(user, itemUser);

            await _repository.UpdateUserAsync(itemUser);
            return new CreateOrUpdateUserResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Cập nhật thành công" };
        }
    }
}
