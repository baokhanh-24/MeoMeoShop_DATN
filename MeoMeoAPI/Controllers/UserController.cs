using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;

        public UserController(IUserServices userServices)
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

        [HttpPut("update-user-async/{id}")]
        public async Task<IActionResult> UpdateUserAsync(Guid id, [FromBody] CreateOrUpdateUserDTO user)
        {
            var result = await _userServices.UpdateUserAsync(user);
            return Ok(result);
        }
    }
}
