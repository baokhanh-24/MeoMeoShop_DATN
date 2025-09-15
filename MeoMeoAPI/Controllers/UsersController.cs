using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Employees;
using MeoMeo.Contract.DTOs.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MeoMeo.API.Extensions;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userServices;

        public UsersController(IUserService userServices)
        {
            _userServices = userServices;
        }

        [HttpGet("get-all-user-async")]
        public async Task<IActionResult> GetAllUserAsync()
        {
            var result = await _userServices.GetAllUserAsync();
            return Ok(result);
        }

        [HttpGet("find-user-by-id-async/{id}")]
        public async Task<IActionResult> GetUserByIdAsync(Guid id)
        {
            var result = await _userServices.GetUserByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("create-user-async")]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateOrUpdateUserDTO user)
        {
            var result = await _userServices.CreateUserAsync(user);
            return Ok(result);
        }

        [HttpDelete("delete-user-async/{id}")]
        public async Task<IActionResult> DeleteUserAsync(Guid id)
        {
            var result = await _userServices.DeleteUserAsync(id);
            return Ok(result);
        }

        [HttpPut("update-user-async")]
        public async Task<IActionResult> UpdateUserAsync([FromBody] CreateOrUpdateUserDTO user)
        {
            var result = await _userServices.UpdateUserAsync(user);
            return Ok(result);
        }

        [HttpGet("get-current-user")]
        public async Task<IActionResult> GetCurrentUserAsync()
        {
            try
            {
                var userId = HttpContext.GetCurrentUserId();
                if (userId == Guid.Empty)
                {
                    return Unauthorized(new BaseResponse
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Vui lòng đăng nhập"
                    });
                }

                var result = await _userServices.GetUserByIdAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Có lỗi xảy ra: {ex.Message}"
                });
            }
        }
        
    }
}
