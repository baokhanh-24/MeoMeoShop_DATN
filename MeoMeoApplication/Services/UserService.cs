using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Auth;
using MeoMeo.Contract.DTOs.Employees;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;

namespace MeoMeo.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        
        

        public async Task<CreateOrUpdateUserResponseDTO> ChangePasswordAsync(Guid userId, ChangePasswordDTO changePasswordDto)
        {
            var user = await _repository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return new CreateOrUpdateUserResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Tài khoản không tồn tại."
                };
            }

            // Kiểm tra mật khẩu hiện tại (có thể thêm logic hash password)
            if (user.PasswordHash != changePasswordDto.CurrentPassword)
            {
                return new CreateOrUpdateUserResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Mật khẩu hiện tại không đúng."
                };
            }

            user.PasswordHash = changePasswordDto.NewPassword;
            await _repository.UpdateUserAsync(user);

            return new CreateOrUpdateUserResponseDTO
            {
                ResponseStatus = BaseStatus.Success,
                Message = "Đổi mật khẩu thành công."
            };
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

        public async Task<UserDTO> GetUserByIdAsync(Guid id)
        {
            UserDTO responseDTO = new UserDTO();
            var check = await _repository.GetUserByIdAsync(id);
            responseDTO = _mapper.Map<UserDTO>(check);
            return responseDTO;
        }

        public async Task<CreateOrUpdateUserResponseDTO> UpdateUserAsync(CreateOrUpdateUserDTO user)
        {
            var existingMaterial = await _repository.GetUserByIdAsync(user.Id.Value);
            if (existingMaterial == null)
            {
                return new CreateOrUpdateUserResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không tìm thấy user này."
                };
            }

            _mapper.Map(user, existingMaterial);
            await _repository.UpdateUserAsync(existingMaterial);

            var response = _mapper.Map<CreateOrUpdateUserResponseDTO>(existingMaterial);
            response.Message = "Update tài khoản thành công.";
            response.ResponseStatus = BaseStatus.Success;
            return response;
        }
    }
}
