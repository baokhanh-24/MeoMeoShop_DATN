using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : ControllerBase
    {
        private readonly IBankServices _bankServices;

        public BankController(IBankServices bankServices)
        {
            _bankServices = bankServices;
        }

        [HttpGet("get-all-bank-async")]
        public async Task<IActionResult> GetAllBankAsync()
        {
            var result = await _bankServices.GetAllBankAsync();
            return Ok(result);
        }

        [HttpGet("find-bank-by-id-async/{id}")]
        public async Task<IActionResult> GetBankByIdAsync(Guid id)
        {
            var result = await _bankServices.GetBankByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("create-bank-async")]
        public async Task<IActionResult> CreateBankAsync([FromBody] CreateOrUpdateBankDTO bank)
        {
            var result = await _bankServices.CreateBankAsync(bank);
            return Ok(result);
        }

        [HttpDelete("delete-bank-async/{id}")]
        public async Task<IActionResult> DeleteBankAsync(Guid id)
        {
            var result = await _bankServices.DeleteBankAsync(id);
            return Ok(result);
        }

        [HttpPut("update-bank-async/{id}")]
        public async Task<IActionResult> UpdateBankAsync(Guid id, [FromBody] CreateOrUpdateBankDTO bank)
        {
            var result = await _bankServices.UpdateBankAsync(bank);
            return Ok(result);
        }
    }
}
