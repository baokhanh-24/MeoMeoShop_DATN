using System.Linq.Expressions;
using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;

using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace MeoMeo.Application.Services
{
    public class CustomerServices : ICustomerServices
    {
        private readonly ICustomerRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerServices(ICustomerRepository repository, IMapper mapper, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _mapper = mapper;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateOrUpdateCustomerResponse> CreateCustomersAsync(CreateOrUpdateCustomerDTO customer)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                //sau tạo mới customer thì tạo mới tài khoản luôn sẽ nhận từ UI password với email để tạo
                var userId = Guid.NewGuid();
                var userToAdd = new User()
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
                await _userRepository.AddAsync(userToAdd);
                var mappedCustomer = _mapper.Map<Customers>(customer);
                mappedCustomer.Id = Guid.NewGuid();
                mappedCustomer.UserId = userId;
                var response= await _repository.CreateCustomersAsync(mappedCustomer);
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

        public async Task<PagingExtensions.PagedResult<CustomerDTO>> GetAllCustomersAsync(GetListCustomerRequestDTO request)
        {
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
            query = query.OrderByDescending(c=>c.CreationTime);
            var filteredCustomers = await _repository.GetPagedAsync(query,request.PageIndex,request.PageSize);
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

            var isExistedPhoneNumber= await _repository.AnyAsync(c=>c.Id!=customer.Id && c.PhoneNumber==customer.PhoneNumber);
            if (isExistedPhoneNumber)
            {
                return new CreateOrUpdateCustomerResponse()
                {
                    Message = $"Số điện thoại đã tồn tại",
                    ResponseStatus = BaseStatus.Error
                };
            }
            var customerToUpdate = await _repository.GetCustomersByIdAsync(customer.Id);
            _mapper.Map(customer,customerToUpdate);

            var result = await _repository.UpdateCustomersAsync(customerToUpdate);
            return _mapper.Map<CreateOrUpdateCustomerResponse>(result);
          
        }
    }
}
