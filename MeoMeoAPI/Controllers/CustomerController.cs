using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerServices _customerServices;

        public CustomerController(ICustomerServices customerServices)
        {
            _customerServices = customerServices;
        }

        [HttpGet("get-all-customer-async")]
        public async Task<IActionResult> GetAllCustomersAsync()
        {
            var result = await _customerServices.GetAllCustomersAsync();
            return Ok(result);
        }

        [HttpGet("find-customer-by-id-async/{id}")]
        public async Task<IActionResult> GetCustomersByIdAsync(Guid id)
        {
            var result = await _customerServices.GetCustomersByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("create-customer-async")]
        public async Task<IActionResult> CreateCustomersAsync([FromBody] CreateOrUpdateCustomerDTO customer)
        {
            var result = await _customerServices.CreateCustomersAsync(customer);
            return Ok(result);
        }

        [HttpDelete("delete-customer-async/{id}")]
        public async Task<IActionResult> DeleteCustomersAsync(Guid id)
        {
            var result = await _customerServices.DeleteCustomersAsync(id);
            return Ok(result);
        }

        [HttpPut("update-customer-async/{id}")]
        public async Task<IActionResult> UpdateCustomersAsync(Guid id, [FromBody] CreateOrUpdateCustomerDTO customer)
        {
            var result = await _customerServices.UpdateCustomersAsync(customer);
            return Ok(result);
        }
    }
}
