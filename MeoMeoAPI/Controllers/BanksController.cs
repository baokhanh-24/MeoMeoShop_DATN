using AutoMapper;
using MeoMeo.API.Extensions;
using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BanksController : ControllerBase
    {
        private readonly IBankService _bankServices;

        public BanksController(IBankService bankServices)
        {
            _bankServices = bankServices;
        }

        [HttpGet("get-all-bank-async")]
        public async Task<IActionResult> GetAllBankAsync()
        {
            var result = await _bankServices.GetListAllBankAsync();
            return Ok(result);
        }

        [HttpGet("get-paging-bank-async")]
        public async Task<IActionResult> GetAllBankAsync([FromQuery] GetListBankRequestDTO request)
        {
            var result = await _bankServices.GetAllBankAsync(request);
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
