using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryAddressesController : ControllerBase
    {
        private readonly IDeliveryAddressService _deliveryAddressService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeliveryAddressesController(IDeliveryAddressService deliveryAddressService, IHttpContextAccessor httpContextAccessor)
        {
            _deliveryAddressService = deliveryAddressService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet()]
        public async Task<IActionResult> GetDeliveryAddress()
        {
            var result = await _deliveryAddressService.GetAllDeliveryAddressAsync();
            return Ok(result);
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetMyDeliveryAddresses()
        {
            var customerId = MeoMeo.API.Extensions.HttpContextExtensions.GetCurrentCustomerId(_httpContextAccessor.HttpContext);
            if (customerId == Guid.Empty) return Unauthorized();
            var result = await _deliveryAddressService.GetByCustomerIdAsync(customerId);
            return Ok(result);
        }

        [HttpGet("find-delivery-address-by-id/{id}")]
        public async Task<IActionResult> GetDeliveryAddressById(Guid id)
        {
            var result = await _deliveryAddressService.GetDeliveryAddressByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("create-delivery-address")]
        public async Task<IActionResult> CreateDeliveryAddress([FromBody] CreateOrUpdateDeliveryAddressDTO addressDTO)
        {
            var customerId = MeoMeo.API.Extensions.HttpContextExtensions.GetCurrentCustomerId(_httpContextAccessor.HttpContext);
            if (customerId == Guid.Empty) return Unauthorized();
            
            addressDTO.CustomerId = customerId;
            var result = await _deliveryAddressService.CreateDeliveryAddressAsync(addressDTO);
            return Ok(result);
        }

        [HttpPut("update-delivery-address/{id}")]
        public async Task<IActionResult> UpdateDeliveryAddress(Guid id, [FromBody] CreateOrUpdateDeliveryAddressDTO addressDTO)
        {
            var customerId = MeoMeo.API.Extensions.HttpContextExtensions.GetCurrentCustomerId(_httpContextAccessor.HttpContext);
            if (customerId == Guid.Empty) return Unauthorized();
            
            addressDTO.Id = id;
            addressDTO.CustomerId = customerId;
            var result = await _deliveryAddressService.UpdateDeliveryAddressAsync(addressDTO);
            return Ok(result);
        }

        [HttpDelete("delete-delivery-address/{id}")]
        public async Task<IActionResult> DeleteDeliveryAddress(Guid id)
        {
            var result = await _deliveryAddressService.DeleteDeliveryAddressAsync(id);
            return Ok(result);
        }
    }
}
