using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryStatisticsController : ControllerBase
    {
        private readonly IInventoryStatisticsService _inventoryStatisticsService;

        public InventoryStatisticsController(IInventoryStatisticsService inventoryStatisticsService)
        {
            _inventoryStatisticsService = inventoryStatisticsService;
        }

        [HttpPost("get-statistics")]
        public async Task<IActionResult> GetInventoryStatistics([FromBody] GetInventoryStatisticsRequestDTO request)
        {
            var result = await _inventoryStatisticsService.GetInventoryStatisticsAsync(request);
            return Ok(result);
        }

        [HttpPost("get-history")]
        public async Task<IActionResult> GetInventoryHistory([FromBody] GetInventoryHistoryRequestDTO request)
        {
            var result = await _inventoryStatisticsService.GetInventoryHistoryAsync(request);
            return Ok(result);
        }
    }
}
