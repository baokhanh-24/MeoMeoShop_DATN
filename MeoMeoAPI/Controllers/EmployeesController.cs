using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Employees;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MeoMeo.API.Extensions;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeServices _employeeServices;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmployeesController(IEmployeeServices employeeServices, IUserRepository user, IUnitOfWork unitOfWork,
            IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _employeeServices = employeeServices;
            _userRepository = user;
            _unitOfWork = unitOfWork;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPut("change-password-async")]
        public async Task<MeoMeo.Contract.Commons.BaseResponse> ChangePasswordAsync([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.GetCurrentUserId();
                if (userId == Guid.Empty)
                {
                    return new MeoMeo.Contract.Commons.BaseResponse()
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Vui lòng đăng nhập"
                    };
                }
                var response = await _employeeServices.ChangePasswordAsync(userId, changePasswordDTO);
                return response;
            }
            catch (Exception ex)
            {
                return new MeoMeo.Contract.Commons.BaseResponse()
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Có lỗi xảy ra: {ex.Message}"
                };
            }
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
            var result = await _employeeServices.GetEmployeeByIdAsync(id);
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

        [HttpPut("update-profile")]
        public async Task<CreateOrUpdateEmployeeResponseDTO> UpdateProfileAsync([FromBody] CreateOrUpdateEmployeeDTO employee)
        {
            var userId = _httpContextAccessor.HttpContext.GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return new CreateOrUpdateEmployeeResponseDTO()
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Vui lòng đăng nhập"
                };
            }
            employee.UserId = userId;
            var result = await _employeeServices.UpdateProfileAsync(employee);
            return result;
        }

        [HttpGet("get-avatar-url")]
        public async Task<IActionResult> GetAvatarUrl()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.GetCurrentUserId();
                if (userId == Guid.Empty)
                {
                    return BadRequest(new { message = "Vui lòng đăng nhập" });
                }

                var avatarUrl = await _employeeServices.GetOldUrlAvatar(userId);
                return Ok(new { avatarUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        [HttpPost("upload-avatar-async")]
        public async Task<MeoMeo.Contract.Commons.BaseResponse> UploadAvatar([FromForm] UploadAvatarDTO request)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.GetCurrentUserId();
                if (userId == Guid.Empty)
                {
                    return new MeoMeo.Contract.Commons.BaseResponse()
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Vui lòng đăng nhập"
                    };
                }
                if (request.AvatarFile == null || request.AvatarFile.Length == 0)
                {
                    return new MeoMeo.Contract.Commons.BaseResponse()
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Không có file nào được tải lên."
                    };
                }
                var acceptedExtensions = new List<string>
                {
                    "jpg", "jpeg", "png", "gif", "bmp", "tiff", "tif", "webp", "svg", "heic", "heif"
                };
                var oldAvatar = await _employeeServices.GetOldUrlAvatar(userId);
                var uploadResults = await FileUploadHelper.UploadFilesAsync(
                    _environment,
                    new List<IFormFile> { request.AvatarFile },
                    "Employees",
                    userId,
                    acceptedExtensions,
                    true,
                    5 * 1024 * 1024
                );
                var response = await _employeeServices.UploadAvatarAsync(userId, uploadResults.First());
                if (response.ResponseStatus != BaseStatus.Success)
                {
                    return response;
                }
                FileUploadHelper.DeleteUploadedFiles(_environment, new List<FileUploadResult> { new FileUploadResult { RelativePath = oldAvatar } });
                return response;
            }
            catch (Exception ex)
            {
                return new MeoMeo.Contract.Commons.BaseResponse()
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Có lỗi xảy ra: {ex.Message}"
                };
            }
        }
    }
}
