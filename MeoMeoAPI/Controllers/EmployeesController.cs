using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Employees;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeServices _employeeServices;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeesController(IEmployeeServices employeeServices, IUserRepository user, IUnitOfWork unitOfWork)
        {
            _employeeServices = employeeServices;
            _userRepository = user;
            _unitOfWork = unitOfWork;
        }

        [HttpPut("change-password-async")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequestDTO changePasswordDTO)
        {
            var user = await _userRepository.GetByIdAsync(changePasswordDTO.UserId);
            if (user == null)
            {
                return NotFound("Không tìm thấy tài khoản.");
            }
            user.PasswordHash = changePasswordDTO.NewPassword;
            await _unitOfWork.SaveChangesAsync();
            return Ok( new CreateOrUpdateEmployeeResponseDTO
            {
                ResponseStatus = BaseStatus.Success,
                Message = "Đổi mật khẩu thành công"
            });

        }

        [HttpGet("get-all-employee-async")]
        public async Task<ActionResult<PagingExtensions.PagedResult<CreateOrUpdateEmployeeDTO>>> GetAllEmployeeAsync([FromQuery] GetlistEmployeesRequestDTO request)
        {
            var result = await _employeeServices.GetAllEmployeeAsync(request);
            return Ok(result); 
        }


        [HttpGet("find-employee-by-id-async/{id}")]
        public async Task<IActionResult> GetEmployeeByIdAsync(Guid id)
        {
            var result = await _employeeServices.GetEmployeeByIdAsyncccc(id);
            return Ok(result);
        }

        [HttpPost("create-employee-async")]
        public async Task<IActionResult> CreateEmployeeAsync([FromBody] CreateOrUpdateEmployeeDTO employee)
        {
            var result = await _employeeServices.CreateEmployeeAsync(employee);
            return Ok(result);
        }

        [HttpDelete("delete-employee-async/{id}")]
        public async Task<IActionResult> DeleteEmployeeAsync(Guid id)
        {
            var result = await _employeeServices.DeleteEmployeeAsync(id);
            return Ok(result);
        }

        [HttpPut("update-employee-async/{id}")]
        public async Task<IActionResult> UpdateEmployeeAsync(Guid id, [FromBody] CreateOrUpdateEmployeeDTO employee)
        {
            employee.Id = id;
            var result = await _employeeServices.UpdateEmployeeAsync(employee);
            return Ok(result);
        }
    }
}
