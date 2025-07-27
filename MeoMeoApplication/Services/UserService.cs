using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
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

        public async Task<CreateOrUpdateUserResponseDTO> ChangePasswordAsync(ChangePasswordRequestDTO request)
        {
            var user = await _repository.GetUserByIdAsync(request.UserId);
            if (user == null)
            {
                return new CreateOrUpdateUserResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Tài khỏan không tồn tại."
                };
            } await _repository.UpdateUserAsync(user);

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
