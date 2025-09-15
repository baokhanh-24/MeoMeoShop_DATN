using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Customer;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomersController(ICustomerServices customerServices, IWebHostEnvironment environment,
            IHttpContextAccessor httpContextAccessor)
        {
            _customerServices = customerServices;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("get-all-customer-async")]
        public async Task<PagingExtensions.PagedResult<CustomerDTO>> GetAllCustomersAsync(
            [FromQuery] GetListCustomerRequestDTO request)
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
        public async Task<CreateOrUpdateCustomerResponse> CreateCustomersAsync(
            [FromBody] CreateOrUpdateCustomerDTO customer)
        {
            var result = await _customerServices.CreateCustomersAsync(customer);
            return result;
        }

        [HttpPost("create-quick-customer-async")]
        public async Task<QuickCustomerResponseDTO> CreateQuickCustomerAsync([FromBody] CreateQuickCustomerDTO request)
        {
            var result = await _customerServices.CreateQuickCustomerAsync(request);
            return result;
        }

        [HttpDelete("delete-customer-async/{id}")]
        public async Task<bool> DeleteCustomersAsync(Guid id)
        {
            var result = await _customerServices.DeleteCustomersAsync(id);
            return result;
        }

        [HttpPut("update-customer-async/{id}")]
        public async Task<CreateOrUpdateCustomerResponse> UpdateCustomersAsync(Guid id,
            [FromBody] CreateOrUpdateCustomerDTO customer)
        {
            var result = await _customerServices.UpdateCustomersAsync(customer);
            return result;
        }

        [HttpPut("update-profile")]
        public async Task<CreateOrUpdateCustomerResponse> UpdateCustomersAsync(
            [FromBody] CreateOrUpdateCustomerDTO customer)
        {
            var customerId = _httpContextAccessor.HttpContext.GetCurrentCustomerId();
            var userId = _httpContextAccessor.HttpContext.GetCurrentUserId();
            if (customerId == Guid.Empty)
            {
                return new CreateOrUpdateCustomerResponse()
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Vui lòng đăng nhập"
                };
            }

            customer.Id = customerId;
            customer.UserId = userId;
            var result = await _customerServices.UpdateCustomersAsync(customer);
            return result;
        }

        [HttpPost("upload-avatar-async")]
        public async Task<BaseResponse> UploadAvatar([FromForm] UploadAvatarDTO request)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.GetCurrentUserId();
                if (userId == Guid.Empty)
                {
                    return new BaseResponse()
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Vui lòng đăng nhập"
                    };
                }

                if (request.AvatarFile == null || request.AvatarFile.Length == 0)
                {
                    return new BaseResponse()
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Không có file nào được tải lên."
                    };

                }

                var acceptedExtensions = new List<string>
                {
                    "jpg", "jpeg", "png", "gif", "bmp", "tiff", "tif", "webp", "svg", "heic", "heif"
                };
                var oldAvatar = await _customerServices.GetOldUrlAvatar(userId);
                var uploadResults = await FileUploadHelper.UploadFilesAsync(
                    _environment,
                    new List<IFormFile> { request.AvatarFile },
                    "Users",
                    userId,
                    acceptedExtensions,
                    true,
                    5 * 1024 * 1024
                );
                var response = await _customerServices.UploadAvatarAsync(userId, uploadResults.First());
                if (response.ResponseStatus != BaseStatus.Success)
                {
                    return response;
                }

                FileUploadHelper.DeleteUploadedFiles(_environment,
                    new List<FileUploadResult> { new FileUploadResult { RelativePath = oldAvatar } });
                return response;



            }
            catch (Exception ex)
            {
                return new BaseResponse()
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Có lỗi xảy ra: {ex.Message}"
                };
            }
        }

        [HttpGet("get-customer-detail-async/{customerId}")]
        public async Task<IActionResult> GetCustomerDetailAsync(Guid customerId)
        {
            try
            {
                var result = await _customerServices.GetCustomerDetailAsync(customerId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }
    }
}
