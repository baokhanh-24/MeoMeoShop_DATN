using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherServices _voucherServices;

        public VoucherController(IVoucherServices voucherServices)
        {
            _voucherServices = voucherServices;
        }

        [HttpGet("get-all-voucher-async")]
        public async Task<IActionResult> GetAllVoucherAsync()
        {
            var result = await _voucherServices.GetAllVoucherAsync();
            return Ok(result);
        }

        [HttpGet("find-voucher-by-id-async/{id}")]
        public async Task<IActionResult> GetVoucherByIdAsync(Guid id)
        {
            var result = await _voucherServices.GetVoucherByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("create-voucher-async")]
        public async Task<IActionResult> CreateVoucherAsync([FromBody] CreateOrUpdateVoucherDTO voucher)
        {
            var result = await _voucherServices.CreateVoucherAsync(voucher);
            return Ok(result);
        }

        [HttpDelete("delete-voucher-async/{id}")]
        public async Task<IActionResult> DeleteVoucherAsync(Guid id)
        {
            var result = await _voucherServices.DeleteVoucherAsync(id);
            return Ok(result);
        }

        [HttpPut("update-voucher-async/{id}")]
        public async Task<IActionResult> UpdateVoucherAsync(Guid id,[FromBody] CreateOrUpdateVoucherDTO voucher)
        {
            var result = await _voucherServices.UpdateVoucherAsync( voucher);
            return Ok(result);
        }
    }
}
