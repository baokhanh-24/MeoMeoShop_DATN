using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs.Statistics;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardStatisticsDTO>> GetDashboardStatistics([FromQuery] StatisticsRequestDTO request)
        {
            try
            {
                var result = await _statisticsService.GetDashboardStatisticsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("revenue")]
        public async Task<ActionResult<RevenueStatisticsDTO>> GetRevenueStatistics([FromQuery] StatisticsRequestDTO request)
        {
            try
            {
                var result = await _statisticsService.GetRevenueStatisticsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("orders")]
        public async Task<ActionResult<OrderStatisticsDTO>> GetOrderStatistics([FromQuery] StatisticsRequestDTO request)
        {
            try
            {
                var result = await _statisticsService.GetOrderStatisticsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("customers")]
        public async Task<ActionResult<CustomerStatisticsDTO>> GetCustomerStatistics([FromQuery] StatisticsRequestDTO request)
        {
            try
            {
                var result = await _statisticsService.GetCustomerStatisticsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("inventory")]
        public async Task<ActionResult<InventoryStatisticsDTO>> GetInventoryStatistics()
        {
            try
            {
                var result = await _statisticsService.GetInventoryStatisticsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("top-products")]
        public async Task<ActionResult<List<TopProductDTO>>> GetTopProductsByPeriod([FromQuery] TopProductsRequestDTO request)
        {
            try
            {
                var result = await _statisticsService.GetTopProductsByPeriodAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
