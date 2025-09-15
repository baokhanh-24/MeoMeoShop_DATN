using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpPost("send-daily-report")]
        public async Task<IActionResult> SendDailyReport([FromBody] SendReportRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.AdminEmail))
                {
                    return BadRequest(new BaseResponse
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Email admin không được để trống"
                    });
                }

                var result = await _reportService.SendDailyReportAsync(request.AdminEmail);

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

        [HttpPost("send-weekly-report")]
        public async Task<IActionResult> SendWeeklyReport([FromBody] SendReportRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.AdminEmail))
                {
                    return BadRequest(new BaseResponse
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Email admin không được để trống"
                    });
                }

                var result = await _reportService.SendWeeklyReportAsync(request.AdminEmail);

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

    public class SendReportRequest
    {
        public string AdminEmail { get; set; } = string.Empty;
    }
}
