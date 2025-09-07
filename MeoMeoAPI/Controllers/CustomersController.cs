using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MeoMeo.API.Extensions;
using MeoMeo.Contract.Commons;
using MeoMeo.Domain.IRepositories;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerServices _customerServices;
        private readonly IWebHostEnvironment _environment;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CustomersController(ICustomerServices customerServices, IWebHostEnvironment environment, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _customerServices = customerServices;
            _environment = environment;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("get-all-customer-async")]
        public async Task<PagingExtensions.PagedResult<CustomerDTO>> GetAllCustomersAsync([FromQuery] GetListCustomerRequestDTO request)
        {
            var result = await _customerServices.GetAllCustomersAsync(request);
            return result;
        }

        [HttpGet("find-customer-by-id-async/{id}")]
        public async Task<CustomerDTO> GetCustomersByIdAsync(Guid id)
        {
            var result = await _customerServices.GetCustomersByIdAsync(id);
            return result;
        }

        [HttpPost("create-customer-async")]
        public async Task<CreateOrUpdateCustomerResponse> CreateCustomersAsync([FromBody] CreateOrUpdateCustomerDTO customer)
        {
            var result = await _customerServices.CreateCustomersAsync(customer);
            return result;
        }

        [HttpDelete("delete-customer-async/{id}")]
        public async Task<bool> DeleteCustomersAsync(Guid id)
        {
            var result = await _customerServices.DeleteCustomersAsync(id);
            return result;
        }

        [HttpPut("update-customer-async/{id}")]
        public async Task<CreateOrUpdateCustomerResponse> UpdateCustomersAsync(Guid id, [FromBody] CreateOrUpdateCustomerDTO customer)
        {
            var result = await _customerServices.UpdateCustomersAsync(customer);
            return result;
        }

        [HttpPost("upload-avatar-async/{customerId}")]
        public async Task<IActionResult> UploadAvatarAsync(Guid customerId, [FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Không có file nào được tải lên.");
                }

                var acceptedExtensions = new List<string> { "jpg", "jpeg", "png", "gif" };
                var uploadResults = await FileUploadHelper.UploadFilesAsync(
                    _environment,
                    new List<IFormFile> { file },
                    "Customers",
                    customerId,
                    acceptedExtensions,
                    true,
                    5 * 1024 * 1024 // 5MB
                );

                if (uploadResults.Any())
                {
                    var customer = await _customerServices.GetCustomersByIdAsync(customerId);
                    if (customer != null)
                    {
                        var updateDto = new CreateOrUpdateCustomerDTO
                        {
                            Id = customer.Id,
                            UserId = customer.UserId,
                            Name = customer.Name,
                            PhoneNumber = customer.PhoneNumber,
                            Address = customer.Address,
                            DateOfBirth = customer.DateOfBirth,
                            Status = customer.Status,
                            Avatar = uploadResults.First().RelativePath
                        };

                        await _customerServices.UpdateCustomersAsync(updateDto);
                        return Ok(new { success = true, avatarUrl = uploadResults.First().RelativePath });
                    }
                }

                return BadRequest("Tải ảnh thất bại.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Có lỗi xảy ra: {ex.Message}");
            }
        }

        [HttpPut("change-password-async")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                var customer = await _customerServices.GetCustomersByIdAsync(changePasswordDTO.CustomerId);
                if (customer == null)
                {
                    return NotFound("Không tìm thấy khách hàng.");
                }

                var user = await _userRepository.GetByIdAsync(customer.UserId);
                if (user == null)
                {
                    return NotFound("Không tìm thấy tài khoản.");
                }

                // Kiểm tra mật khẩu hiện tại (trong thực tế cần hash và verify)
                if (user.PasswordHash != changePasswordDTO.CurrentPassword)
                {
                    return BadRequest("Mật khẩu hiện tại không chính xác.");
                }

                // Cập nhật mật khẩu mới
                user.PasswordHash = changePasswordDTO.NewPassword;
                await _unitOfWork.SaveChangesAsync();

                return Ok(new { success = true, message = "Đổi mật khẩu thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Có lỗi xảy ra: {ex.Message}");
            }
        }
    }
}
