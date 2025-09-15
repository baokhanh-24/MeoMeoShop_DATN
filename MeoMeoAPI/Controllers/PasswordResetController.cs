using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordResetController : ControllerBase
    {
        private readonly IPasswordResetService _passwordResetService;

        public PasswordResetController(IPasswordResetService passwordResetService)
        {
            _passwordResetService = passwordResetService;
        }

        [HttpPost("request-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetDTO request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    return BadRequest(new BaseResponse
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Email không được để trống"
                    });
                }

                var result = await _passwordResetService.RequestPasswordResetAsync(request.Email);

                if (result.ResponseStatus == BaseStatus.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Có lỗi xảy ra: {ex.Message}"
                });
            }
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.ResetToken))
                {
                    return BadRequest(new BaseResponse
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Reset token không được để trống"
                    });
                }

                if (string.IsNullOrWhiteSpace(request.NewPassword))
                {
                    return BadRequest(new BaseResponse
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Mật khẩu mới không được để trống"
                    });
                }

                if (request.NewPassword.Length < 6)
                {
                    return BadRequest(new BaseResponse
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Mật khẩu mới phải có ít nhất 6 ký tự"
                    });
                }

                var result = await _passwordResetService.ResetPasswordAsync(request.ResetToken, request.NewPassword);

                if (result.ResponseStatus == BaseStatus.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Có lỗi xảy ra: {ex.Message}"
                });
            }
        }

        [HttpPost("validate-token")]
        public async Task<IActionResult> ValidateResetToken([FromBody] ValidateResetTokenDTO request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.ResetToken))
                {
                    return BadRequest(new BaseResponse
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Reset token không được để trống"
                    });
                }

                var result = await _passwordResetService.ValidateResetTokenAsync(request.ResetToken);

                if (result.ResponseStatus == BaseStatus.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Có lỗi xảy ra: {ex.Message}"
                });
            }
        }
    }
}
