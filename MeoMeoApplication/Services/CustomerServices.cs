using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace MeoMeo.Application.Services
{
    public class CustomerServices : ICustomerServices
    {
        private readonly ICustomerRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerServices(ICustomerRepository repository, IMapper mapper, IUserRepository userRepository, 
            IRoleRepository roleRepository, IUserRoleRepository userRoleRepository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _mapper = mapper;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateOrUpdateCustomerResponse> CreateCustomersAsync(CreateOrUpdateCustomerDTO customer)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                
                // Tạo tài khoản user mới
                var userId = Guid.NewGuid();
                var userToAdd = new User()
                {
                    Id = userId,
                    PasswordHash = FunctionHelper.ComputerSha256Hash(customer.Password ?? "Ab@12345"), 
                    Avatar =  customer.Avatar,
                    LastLogin = DateTime.Now,
                    CreationTime = DateTime.Now,
                    Email = customer.Email ?? "customer@meomeo.com",
                    UserName = customer.Email ?? "customer@meomeo.com",
                    Status = 1,
                    IsLocked = false
                };
                await _userRepository.AddAsync(userToAdd);
                
                // Tìm role Customer và gán cho user
                var customerRole = await _roleRepository.GetRoleByName("Customer");
                if (customerRole != null)
                {
                    var userRole = new UserRole
                    {
                        UserId = userId,
                        RoleId = customerRole.Id
                    };
                    await _userRoleRepository.AddUserRole(userRole);
                }
                
                // Tạo customer
                var mappedCustomer = _mapper.Map<Customers>(customer);
                mappedCustomer.Id = Guid.NewGuid();
                mappedCustomer.UserId = userId;
                var response = await _repository.CreateCustomersAsync(mappedCustomer);
                
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return _mapper.Map<CreateOrUpdateCustomerResponse>(response);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                Console.WriteLine($"Transaction failed: {ex.Message}");
                return new CreateOrUpdateCustomerResponse()
                {
                    Message = ex.Message,
                    ResponseStatus = BaseStatus.Error
                };
            }
        }

        public async Task<bool> DeleteCustomersAsync(Guid id)
        {
            var customerToDelete = await _repository.GetCustomersByIdAsync(id);

            if (customerToDelete == null)
            {
                return false;
            }

            await _repository.DeleteCustomersAsync(customerToDelete.Id);
            return true;
        }

        public async Task<string> GetOldUrlAvatar(Guid userId)
        {
            var user= await _userRepository.Query().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return "";
            }
            return user.Avatar;
        }
        public async Task<BaseResponse> UploadAvatarAsync(Guid userId, FileUploadResult file)
        {
          var user= await _userRepository.Query().FirstOrDefaultAsync(u => u.Id == userId);
          if (user == null)
          {
              return new BaseResponse()
              {
                  ResponseStatus = BaseStatus.Error,
                  Message = "User not found"
              };
          }
          user.Avatar = file.RelativePath;
          await _userRepository.UpdateAsync(user);
          return new BaseResponse()
          {
              ResponseStatus = BaseStatus.Success,
              Message = "Cập nhật avatar thành công"
          };
        }

        public async Task<BaseResponse> ChangePasswordAsync(Guid userId,ChangePasswordDTO request)
        {
            var user= await _userRepository.Query().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return new BaseResponse()
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "User not found"
                };
            }
            var currentPassword=  FunctionHelper.ComputerSha256Hash(request.CurrentPassword);
            if (user.PasswordHash != currentPassword)
            {
                return new BaseResponse()
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Mật khẩu hiện tại không chính xác"
                };
            }
            var newPassword=  FunctionHelper.ComputerSha256Hash(request.CurrentPassword);
            user.PasswordHash = newPassword;
            await _userRepository.UpdateAsync(user);
            return new BaseResponse()
            {
                ResponseStatus = BaseStatus.Success,
                Message = "Thay đổi mật khẩu thành công"
            };
        }

        public async Task<List<Customers>> GetAllCustomersAsync()
        {
            return await _repository.GetAllCustomersAsync();
        }
        public async Task<PagingExtensions.PagedResult<CustomerDTO>> GetAllCustomersAsync(GetListCustomerRequestDTO request)
        {
            // where dùng để tìm kiếm theo nhu cầu
            try
            {
                var query = _repository.Query();

                if (!string.IsNullOrEmpty(request.FullNameFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.Name, $"%{request.FullNameFilter}%"));
                }

                if (!string.IsNullOrEmpty(request.CodeFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.Code, $"%{request.CodeFilter}%"));
                }

                if (!string.IsNullOrEmpty(request.TaxCodeFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.TaxCode, $"%{request.TaxCodeFilter}%"));
                }

                if (!string.IsNullOrEmpty(request.AddressFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.Address, $"%{request.AddressFilter}%"));
                }

                if (!string.IsNullOrEmpty(request.PhoneNumberFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.PhoneNumber, $"%{request.PhoneNumberFilter}%"));
                }

                if (request.StatusFilter != null)
                {
                    query = query.Where(c => c.Status == request.StatusFilter);
                }

                if (request.DateOfBirthFilter != null)
                {
                    query = query.Where(c => c.DateOfBirth.HasValue && DateOnly.FromDateTime(c.DateOfBirth.Value) == request.DateOfBirthFilter.Value);
                }
                query = query.OrderByDescending(c => c.CreationTime);
                var filteredCustomers = await _repository.GetPagedAsync(query, request.PageIndex, request.PageSize);
                var dtoItems = _mapper.Map<List<CustomerDTO>>(filteredCustomers.Items);

                return new PagingExtensions.PagedResult<CustomerDTO>
                {
                    TotalRecords = filteredCustomers.TotalRecords,
                    PageIndex = filteredCustomers.PageIndex,
                    PageSize = filteredCustomers.PageSize,
                    Items = dtoItems
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public async Task<CustomerDTO> GetCustomersByIdAsync(Guid id)
        {
            var customer = await _repository.GetCustomersByIdAsync(id);
            return _mapper.Map<CustomerDTO>(customer);
        }

        public async Task<CreateOrUpdateCustomerResponse> UpdateCustomersAsync(CreateOrUpdateCustomerDTO customer)
        {

            var isExistedPhoneNumber = await _repository.AnyAsync(c => c.Id != customer.Id && c.PhoneNumber == customer.PhoneNumber);
            if (isExistedPhoneNumber)
            {
                return new CreateOrUpdateCustomerResponse()
                {
                    Message = $"Số điện thoại đã tồn tại",
                    ResponseStatus = BaseStatus.Error
                };
            }
            var customerToUpdate = await _repository.GetCustomersByIdAsync(customer.Id.Value);
            _mapper.Map(customer, customerToUpdate);

            var result = await _repository.UpdateCustomersAsync(customerToUpdate);
            return _mapper.Map<CreateOrUpdateCustomerResponse>(result);

        }
    }
}
