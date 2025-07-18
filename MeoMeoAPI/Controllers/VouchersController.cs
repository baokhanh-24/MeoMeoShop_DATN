using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VouchersController : ControllerBase
    {
        private readonly IVoucherService _voucherServices;

        public VouchersController(IVoucherService voucherServices)
        {
            _voucherServices = voucherServices;
        }

        [HttpGet("get-all-voucher-async")]
        public async Task<PagingExtensions.PagedResult<VoucherDTO>> GetAllVouchersAsync([FromQuery] GetListVoucherRequestDTO request)
        {
            var result = await _voucherServices.GetAllVoucherAsync(request);
            return result;
        }

        [HttpGet("find-voucher-by-id-async/{id}")]
        public async Task<CreateOrUpdateVoucherResponseDTO> GetVoucherByIdAsync(Guid id)
        {
            var result = await _voucherServices.GetVoucherByIdAsync(id);
            return result;
        }

        [HttpPost("create-voucher-async")]
        public async Task<CreateOrUpdateVoucherResponseDTO> CreateVoucherAsync([FromBody] CreateOrUpdateVoucherDTO voucher)
        {
            var result = await _voucherServices.CreateVoucherAsync(voucher);
            return result;
        }

        [HttpPut("update-voucher-async/{id}")]
        public async Task<CreateOrUpdateVoucherResponseDTO> UpdateVoucherAsync(Guid id, [FromBody] CreateOrUpdateVoucherDTO voucher)
        {
            voucher.Id = id; // gán id từ route vào DTO
            var result = await _voucherServices.UpdateVoucherAsync(voucher);
            return result;
        }

        [HttpDelete("delete-voucher-async/{id}")]
        public async Task<bool> DeleteVoucherAsync(Guid id)
        {
            var result = await _voucherServices.DeleteVoucherAsync(id);
            return result;
        }
    }
}