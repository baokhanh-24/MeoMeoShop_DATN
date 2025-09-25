using AutoMapper;
using Azure.Core;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.Shared.Utilities;
using MeoMeo.Shared.IServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Application.Services
{
    public class EmployeeServices : IEmployeeServices
    {
        private readonly IEmployeeRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IEmailService _emailService;

        public EmployeeServices(IEmployeeRepository repository, IMapper mapper, IUnitOfWork unitOfWork,
            IUserRepository userRepository, IRoleRepository roleRepository, IUserRoleRepository userRoleRepository,
            IEmailService emailService)
        {
            _repository = repository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _emailService = emailService;
        }

        public async Task<CreateOrUpdateEmployeeResponseDTO> CreateEmployeeAsync(CreateOrUpdateEmployeeDTO employee)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Validation
                if (string.IsNullOrWhiteSpace(employee.Name))
                {
                    await _unitOfWork.RollbackAsync();
                    return new CreateOrUpdateEmployeeResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Vui lòng nhập họ và tên."
                    };
                }

                if (string.IsNullOrWhiteSpace(employee.PhoneNumber))
                {
                    await _unitOfWork.RollbackAsync();
                    return new CreateOrUpdateEmployeeResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Vui lòng nhập số điện thoại."
                    };
                }

                if (string.IsNullOrWhiteSpace(employee.Email))
                {
                    await _unitOfWork.RollbackAsync();
                    return new CreateOrUpdateEmployeeResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Vui lòng nhập email."
                    };
                }

                if (!IsValidEmail(employee.Email))
                {
                    await _unitOfWork.RollbackAsync();
                    return new CreateOrUpdateEmployeeResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Email không đúng định dạng."
                    };
                }

                if (string.IsNullOrWhiteSpace(employee.Address))
                {
                    await _unitOfWork.RollbackAsync();
                    return new CreateOrUpdateEmployeeResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Vui lòng nhập địa chỉ."
                    };
                }

                if (string.IsNullOrWhiteSpace(employee.Password))
                {
                    await _unitOfWork.RollbackAsync();
                    return new CreateOrUpdateEmployeeResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Vui lòng nhập mật khẩu."
                    };
                }

                // Check trùng số điện thoại
                var existingPhoneEmployee = await _repository.Query()
                    .FirstOrDefaultAsync(e => e.PhoneNumber == employee.PhoneNumber);
                if (existingPhoneEmployee != null)
                {
                    await _unitOfWork.RollbackAsync();
                    return new CreateOrUpdateEmployeeResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Số điện thoại này đã được sử dụng bởi nhân viên khác."
                    };
                }

                // Check trùng email
                if (!string.IsNullOrEmpty(employee.Email))
                {
                    var existingEmailUser = await _userRepository.Query()
                        .FirstOrDefaultAsync(u => u.Email == employee.Email);
                    if (existingEmailUser != null)
                    {
                        await _unitOfWork.RollbackAsync();
                        return new CreateOrUpdateEmployeeResponseDTO
                        {
                            ResponseStatus = BaseStatus.Error,
                            Message = "Email này đã được sử dụng bởi tài khoản khác."
                        };
                    }

                    // Check trùng userName
                    var existingUserNameUser = await _userRepository.Query()
                        .FirstOrDefaultAsync(u => u.UserName == employee.Email);
                    if (existingUserNameUser != null)
                    {
                        await _unitOfWork.RollbackAsync();
                        return new CreateOrUpdateEmployeeResponseDTO
                        {
                            ResponseStatus = BaseStatus.Error,
                            Message = "Tên đăng nhập này đã được sử dụng bởi tài khoản khác."
                        };
                    }
                }

                var userId = Guid.NewGuid();
                var usertoAdd = new User()
                {
                    Id = userId,
                    PasswordHash = FunctionHelper.ComputerSha256Hash(employee.Password ?? "Ab@12345"),
                    Avatar = "//////",
                    LastLogin = DateTime.Now,
                    CreationTime = DateTime.Now,
                    Email = employee.Email ?? "employee@meomeo.com",
                    UserName = employee.Email ?? "employee@meomeo.com",
                    Status = 1,
                    IsLocked = false
                };
                await _userRepository.AddAsync(usertoAdd);

                // Tìm role Employee và gán cho user
                var employeeRole = await _roleRepository.GetRoleByName("Employee");
                if (employeeRole != null)
                {
                    var userRole = new UserRole
                    {
                        UserId = userId,
                        RoleId = employeeRole.Id
                    };
                    await _userRoleRepository.AddUserRole(userRole);
                }

                // Tạo employee
                var mappedEmployee = _mapper.Map<Employee>(employee);
                mappedEmployee.Id = Guid.NewGuid();
                mappedEmployee.UserId = userId;

                // Xử lý ngày sinh - nếu null hoặc năm 1 thì set null
                if (mappedEmployee.DateOfBird.HasValue && mappedEmployee.DateOfBird.Value.Year <= 1)
                {
                    mappedEmployee.DateOfBird = null;
                }

                if (mappedEmployee.DateOfBird.HasValue && mappedEmployee.DateOfBird.Value > DateTime.Today)
                {
                    await _unitOfWork.RollbackAsync();
                    return new CreateOrUpdateEmployeeResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Ngày sinh không được là ngày tương lai."
                    };
                }

                // Tự động sinh mã nhân viên theo format NV00001, NV00002...
                mappedEmployee.Code = await GenerateEmployeeCodeAsync();

                var response = await _repository.CreateEmployeeAsync(mappedEmployee);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                // Gửi email thông báo tạo nhân viên
                try
                {
                    await _emailService.SendEmployeeNotificationAsync(
                        employee.Email ?? "employee@meomeo.com",
                        employee.Name,
                        "tạo mới"
                    );
                }
                catch (Exception emailEx)
                {
                    // Log lỗi email nhưng không làm fail transaction
                    Console.WriteLine($"Failed to send employee creation email: {emailEx.Message}");
                }

                return _mapper.Map<CreateOrUpdateEmployeeResponseDTO>(response);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                Console.WriteLine($"Transaction failed: {ex.Message}");
                throw new Exception("Lỗi khi tạo nhân viên", ex);
            }
        }

        public async Task<bool> DeleteEmployeeAsync(Guid id)
        {
            var employeeToDelete = await _repository.GetEmployeeByIdAsync(id);

            if (employeeToDelete == null)
            {
                return false;
            }

            await _repository.DeleteEmployeeAsync(employeeToDelete.Id);
            return true;
        }

        public async Task<PagingExtensions.PagedResult<CreateOrUpdateEmployeeDTO>> GetAllEmployeeAsync(GetlistEmployeesRequestDTO requestDTO)
        {
            try
            {
                IQueryable<Employee> query = _repository.Query().Include(e => e.User);

                if (!string.IsNullOrEmpty(requestDTO.NameFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.Name, $"%{requestDTO.NameFilter}%"));
                }

                if (!string.IsNullOrEmpty(requestDTO.CodeFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.Code, $"%{requestDTO.CodeFilter}%"));
                }

                if (!string.IsNullOrEmpty(requestDTO.PhoneNumberFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.PhoneNumber, $"%{requestDTO.PhoneNumberFilter}%"));
                }

                if (!string.IsNullOrEmpty(requestDTO.EmailFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.User.Email, $"%{requestDTO.EmailFilter}%"));
                }

                if (!string.IsNullOrEmpty(requestDTO.UserNameFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.User.UserName, $"%{requestDTO.UserNameFilter}%"));
                }

                if (requestDTO.DateOfBirthFilter != null)
                {
                    var filterDate = requestDTO.DateOfBirthFilter.Value.Date;
                    query = query.Where(c => c.DateOfBird == filterDate);
                }

                if (!string.IsNullOrEmpty(requestDTO.AddressFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.Address, $"%{requestDTO.AddressFilter}%"));
                }

                if (requestDTO.StatusFilter.HasValue)
                {
                    query = query.Where(c => c.Status == (int)requestDTO.StatusFilter.Value);
                }

                query = query.OrderByDescending(c => c.User.CreationTime);
                var fileteredEmployees = await _repository.GetPagedAsync(query, requestDTO.PageIndex, requestDTO.PageSize);
                var dtoItems = _mapper.Map<List<CreateOrUpdateEmployeeDTO>>(fileteredEmployees.Items);

                return new PagingExtensions.PagedResult<CreateOrUpdateEmployeeDTO>
                {
                    Items = dtoItems,
                    PageIndex = fileteredEmployees.PageIndex,
                    PageSize = fileteredEmployees.PageSize,
                    TotalRecords = fileteredEmployees.TotalRecords,
                };

            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách nhân viên", ex);
            }
        }

        public async Task<CreateOrUpdateEmployeeResponseDTO> GetEmployeeByIdAsync(Guid id)
        {
            CreateOrUpdateEmployeeResponseDTO responseDTO = new CreateOrUpdateEmployeeResponseDTO();

            var check = await _repository.GetEmployeeByIdAsync(id);
            if (check == null)
            {
                responseDTO.ResponseStatus = BaseStatus.Error;
                responseDTO.Message = "Không tìm thấy employee";
                return responseDTO;
            }

            responseDTO = _mapper.Map<CreateOrUpdateEmployeeResponseDTO>(check);
            responseDTO.ResponseStatus = BaseStatus.Success;
            responseDTO.Message = "";
            return responseDTO;
        }

        public async Task<CreateOrUpdateEmployeeResponseDTO> UpdateEmployeeAsync(CreateOrUpdateEmployeeDTO employee)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Validation
                if (string.IsNullOrWhiteSpace(employee.Name))
                {
                    await _unitOfWork.RollbackAsync();
                    return new CreateOrUpdateEmployeeResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Vui lòng nhập họ và tên."
                    };
                }

                if (string.IsNullOrWhiteSpace(employee.PhoneNumber))
                {
                    await _unitOfWork.RollbackAsync();
                    return new CreateOrUpdateEmployeeResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Vui lòng nhập số điện thoại."
                    };
                }

                if (string.IsNullOrWhiteSpace(employee.Email))
                {
                    await _unitOfWork.RollbackAsync();
                    return new CreateOrUpdateEmployeeResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Vui lòng nhập email."
                    };
                }

                if (!IsValidEmail(employee.Email))
                {
                    await _unitOfWork.RollbackAsync();
                    return new CreateOrUpdateEmployeeResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Email không đúng định dạng."
                    };
                }

                if (string.IsNullOrWhiteSpace(employee.Address))
                {
                    await _unitOfWork.RollbackAsync();
                    return new CreateOrUpdateEmployeeResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Vui lòng nhập địa chỉ."
                    };
                }

                // Validation ngày sinh không được là ngày tương lai
                if (employee.DateOfBird.HasValue && employee.DateOfBird.Value > DateTime.Today)
                {
                    await _unitOfWork.RollbackAsync();
                    return new CreateOrUpdateEmployeeResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Ngày sinh không được là ngày tương lai."
                    };
                }

                var existing = await _repository.GetEmployeeByIdAsync(employee.Id);
                if (existing == null)
                {
                    await _unitOfWork.RollbackAsync();
                    return new CreateOrUpdateEmployeeResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Không tìm thấy nhân viên này."
                    };
                }

                // Check trùng số điện thoại (khác với employee hiện tại)
                var existingPhoneEmployee = await _repository.Query()
                    .FirstOrDefaultAsync(e => e.PhoneNumber == employee.PhoneNumber && e.Id != employee.Id);
                if (existingPhoneEmployee != null)
                {
                    await _unitOfWork.RollbackAsync();
                    return new CreateOrUpdateEmployeeResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Số điện thoại này đã được sử dụng bởi nhân viên khác."
                    };
                }

                // Check trùng email (khác với user hiện tại)
                if (!string.IsNullOrEmpty(employee.Email))
                {
                    var existingEmailUser = await _userRepository.Query()
                        .FirstOrDefaultAsync(u => u.Email == employee.Email && u.Id != existing.UserId);
                    if (existingEmailUser != null)
                    {
                        await _unitOfWork.RollbackAsync();
                        return new CreateOrUpdateEmployeeResponseDTO
                        {
                            ResponseStatus = BaseStatus.Error,
                            Message = "Email này đã được sử dụng bởi tài khoản khác."
                        };
                    }

                    // Check trùng userName (khác với user hiện tại)
                    var existingUserNameUser = await _userRepository.Query()
                        .FirstOrDefaultAsync(u => u.UserName == employee.Email && u.Id != existing.UserId);
                    if (existingUserNameUser != null)
                    {
                        await _unitOfWork.RollbackAsync();
                        return new CreateOrUpdateEmployeeResponseDTO
                        {
                            ResponseStatus = BaseStatus.Error,
                            Message = "Tên đăng nhập này đã được sử dụng bởi tài khoản khác."
                        };
                    }
                }

                // Không update Code khi edit, chỉ update các field khác
                existing.Name = employee.Name;
                existing.PhoneNumber = employee.PhoneNumber;
                existing.DateOfBird = employee.DateOfBird;
                existing.Address = employee.Address;
                existing.Status = employee.Status;

                await _repository.UpdateEmployeeAsync(existing);

                // Update User data if UserId exists
                if (existing.UserId != Guid.Empty)
                {
                    var user = await _userRepository.GetUserByIdAsync(existing.UserId);
                    if (user != null)
                    {
                        // Luôn update email và userName từ employee.Email
                        if (!string.IsNullOrEmpty(employee.Email))
                        {
                            user.Email = employee.Email;
                        }

                        // Đồng bộ trạng thái giữa Employee và User
                        // Map Employee status to User status
                        user.Status = employee.Status == 1 ? 1 : 0;
                        user.IsLocked = employee.Status == 0;

                        await _userRepository.UpdateAsync(user);
                    }
                }

                await _unitOfWork.CommitAsync();
                Console.WriteLine($"[Update] Nhận ID: {employee.Id}");
                var response = _mapper.Map<CreateOrUpdateEmployeeResponseDTO>(existing);
                response.Message = "Update nhân viên successfully.";
                response.ResponseStatus = BaseStatus.Success;
                return response;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new CreateOrUpdateEmployeeResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Có lỗi xảy ra: {ex.Message}"
                };
            }
        }

        public async Task<string> GetOldUrlAvatar(Guid userId)
        {
            var user = await _userRepository.Query().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return "";
            }
            return user.Avatar ?? "";
        }

        public async Task<CreateOrUpdateEmployeeResponseDTO> UpdateProfileAsync(CreateOrUpdateEmployeeDTO employee)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Update Employee
                var existingEmployee = await _repository.GetEmployeeByIdAsync(employee.Id);
                if (existingEmployee == null)
                {
                    await _unitOfWork.RollbackAsync();
                    return new CreateOrUpdateEmployeeResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Không tìm thấy nhân viên này."
                    };
                }

                // Map Employee data
                existingEmployee.Name = employee.Name;
                existingEmployee.PhoneNumber = employee.PhoneNumber ?? "";
                existingEmployee.DateOfBird = employee.DateOfBird;
                existingEmployee.Address = employee.Address;

                await _repository.UpdateEmployeeAsync(existingEmployee);

                // Update User data if UserId exists
                if (existingEmployee.UserId != Guid.Empty && !string.IsNullOrEmpty(employee.Email))
                {
                    var user = await _userRepository.GetUserByIdAsync(existingEmployee.UserId);
                    if (user != null)
                    {
                        user.Email = employee.Email;
                        await _userRepository.UpdateAsync(user);
                    }
                }

                await _unitOfWork.CommitAsync();

                var response = _mapper.Map<CreateOrUpdateEmployeeResponseDTO>(existingEmployee);
                response.Message = "Cập nhật profile thành công.";
                response.ResponseStatus = BaseStatus.Success;
                return response;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new CreateOrUpdateEmployeeResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Có lỗi xảy ra: {ex.Message}"
                };
            }
        }

        public async Task<BaseResponse> UploadAvatarAsync(Guid userId, FileUploadResult file)
        {
            var user = await _userRepository.Query().FirstOrDefaultAsync(u => u.Id == userId);
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

        public async Task<BaseResponse> ChangePasswordAsync(Guid userId, ChangePasswordDTO request)
        {
            var user = await _userRepository.Query().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return new BaseResponse()
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "User not found"
                };
            }
            var currentPassword = FunctionHelper.ComputerSha256Hash(request.CurrentPassword);
            if (user.PasswordHash != currentPassword)
            {
                return new BaseResponse()
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Mật khẩu hiện tại không chính xác"
                };
            }
            var newPassword = FunctionHelper.ComputerSha256Hash(request.NewPassword);
            user.PasswordHash = newPassword;
            await _userRepository.UpdateAsync(user);
            return new BaseResponse()
            {
                ResponseStatus = BaseStatus.Success,
                Message = "Thay đổi mật khẩu thành công"
            };
        }

        private async Task<string> GenerateEmployeeCodeAsync()
        {
            try
            {
                string newCode;
                int attempts = 0;
                const int maxAttempts = 100;

                do
                {
                    // Lấy mã nhân viên lớn nhất hiện tại
                    var lastEmployee = await _repository.Query()
                        .Where(e => e.Code.StartsWith("NV"))
                        .OrderByDescending(e => e.Code)
                        .FirstOrDefaultAsync();

                    int nextNumber = 1;
                    if (lastEmployee != null && !string.IsNullOrEmpty(lastEmployee.Code))
                    {
                        // Extract số từ mã cuối cùng (VD: NV00001 -> 1)
                        var codeNumber = lastEmployee.Code.Substring(2); // Bỏ "NV"
                        if (int.TryParse(codeNumber, out int lastNumber))
                        {
                            nextNumber = lastNumber + 1;
                        }
                    }

                    // Format thành NV00001, NV00002, NV00003...
                    newCode = $"NV{nextNumber:D5}";

                    // Kiểm tra xem mã có trùng không
                    var existingEmployee = await _repository.Query()
                        .FirstOrDefaultAsync(e => e.Code == newCode);

                    if (existingEmployee == null)
                    {
                        break; // Mã không trùng, thoát khỏi vòng lặp
                    }

                    attempts++;
                } while (attempts < maxAttempts);

                if (attempts >= maxAttempts)
                {
                    // Fallback nếu có lỗi hoặc không tìm được mã unique
                    var randomNumber = new Random().Next(1, 99999);
                    newCode = $"NV{randomNumber:D5}";
                }

                return newCode;
            }
            catch (Exception)
            {
                // Fallback nếu có lỗi
                var randomNumber = new Random().Next(1, 99999);
                return $"NV{randomNumber:D5}";
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
