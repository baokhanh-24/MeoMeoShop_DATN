using AutoMapper;
using Azure.Core;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.Services
{
    public class EmployeeServices : IEmployeeServices
    {
        private readonly IEmployeeRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;

        public EmployeeServices(IEmployeeRepository repository, IMapper mapper, IUnitOfWork unitOfWork, IUserRepository userRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        public async Task<CreateOrUpdateEmployeeResponseDTO> CreateEmployeeAsync(CreateOrUpdateEmployeeDTO employee)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var userId = Guid.NewGuid();
                var usertoAdd = new User()
                {
                    Id = userId,
                    PasswordHash = "Ab@12345",
                    Avatar = "//////",
                    LastLogin = DateTime.Now,
                    CreationTime = DateTime.Now,
                    Email = "aaaa@gmail,com",
                    UserName = "aaaa@gmail,com",
                    Status = 1
                };
                await _userRepository.AddAsync(usertoAdd);
                var mappedEmployee = _mapper.Map<Employee>(employee);
                mappedEmployee.Id = Guid.NewGuid();
                mappedEmployee.UserId = userId;
                var response = await _repository.CreateEmployeeAsync(mappedEmployee);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return _mapper.Map<CreateOrUpdateEmployeeResponseDTO>(response);
            }
            catch (Exception ex)
            {
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
                var query = _repository.Query();
                if (!string.IsNullOrEmpty(requestDTO.NameFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.Name, $"%{requestDTO.NameFilter}%"));
                }

                if (!string.IsNullOrEmpty(requestDTO.CodeFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.Name, $"%{requestDTO.CodeFilter}%"));
                }

                if (!string.IsNullOrEmpty(requestDTO.PhoneNumberFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.PhoneNumber, $"%{requestDTO.PhoneNumberFilter}%"));
                }

                if (requestDTO.DateOfBirthFilter != null)
                {
                    var filterDate = requestDTO.DateOfBirthFilter.Value.Date;
                    query = query.Where(c => c.DateOfBird.Date == filterDate);
                }

                if (!string.IsNullOrEmpty(requestDTO.AddressFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.Address, $"%{requestDTO.AddressFilter}%"));
                }

                if (requestDTO.StatusFilter.HasValue)
                {
                    query = query.Where(c => c.Status == (int)requestDTO.StatusFilter.Value);
                }

                query = query.OrderByDescending(c => c.Name);
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

        public async Task<CreateOrUpdateEmployeeResponseDTO> GetEmployeeByIdAsyncccc(Guid id)
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
            var existing = await _repository.GetEmployeeByIdAsync(employee.Id);
            if (existing == null)
            {
                return new CreateOrUpdateEmployeeResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không tìm thấy nhân viên này."

                };
            }

            _mapper.Map(employee, existing);
            await _repository.UpdateEmployeeAsync(existing);
            Console.WriteLine($"[Update] Nhận ID: {employee.Id}");

            var response = _mapper.Map<CreateOrUpdateEmployeeResponseDTO>(existing);
            response.Message = "Update nhân viên successfully.";
            response.ResponseStatus = BaseStatus.Success;
            return response;
        }
    }
}
