using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResetPasswordHistoryController : ControllerBase
    {
        private readonly IResetPasswordHistoryServices _resetPasswordHistoryServices;

        public ResetPasswordHistoryController(IResetPasswordHistoryServices resetPasswordHistoryServices)
        {
            _resetPasswordHistoryServices = resetPasswordHistoryServices;
        }

        [HttpGet("get-all-reset-password-history-async")]
        public async Task<IActionResult> GetAllResetPasswordHistoryAsync()
        {
            var result = await _resetPasswordHistoryServices.GetAllResetPasswordHistoryAsync();
            return Ok(result);
        }

        [HttpGet("find-reset-password-history-by-id-async/{id}")]
        public async Task<IActionResult> GetResetPasswordHistoryByIdAsync(Guid id)
        {
            var result = await _resetPasswordHistoryServices.GetResetPasswordHistoryByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("create-reset-password-history-async")]
        public async Task<IActionResult> CreateResetPasswordHistoryAsync([FromBody] CreateOrUpdateResetPasswordHistoryDTO resetPasswordHistory)
        {
            var result = await _resetPasswordHistoryServices.CreateResetPasswordHistoryAsync(resetPasswordHistory);
            return Ok(result);
        }

        [HttpDelete("delete-reset-password-history-async/{id}")]
        public async Task<IActionResult> DeleteResetPasswordHistoryAsync(Guid id)
        {
            var result = await _resetPasswordHistoryServices.DeleteResetPasswordHistoryAsync(id);
            return Ok(result);
        }

        [HttpPut("update-reset-password-history-async/{id}")]
        public async Task<IActionResult> UpdateResetPasswordHistoryAsync(Guid id, [FromBody] CreateOrUpdateResetPasswordHistoryDTO resetPasswordHistory)
        {
            var result = await _resetPasswordHistoryServices.UpdateResetPasswordHistoryAsync(resetPasswordHistory);
            return Ok(result);
        }
    }
}
