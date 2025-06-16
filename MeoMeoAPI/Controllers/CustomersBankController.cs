using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersBankController : ControllerBase
    {
        private readonly ICustomersBankServices _customersBankServices;

        public CustomersBankController(ICustomersBankServices customersBankServices)
        {
            _customersBankServices = customersBankServices;
        }

        [HttpGet("get-all-customers-bank-async")]
        public async Task<IActionResult> GetAllCustomersBankAsync()
        {
            var result = await _customersBankServices.GetAllCustomersBankAsync();
            return Ok(result);
        }

        [HttpGet("find-customers-bank-by-id-async/{id}")]
        public async Task<IActionResult> GetCustomersBankByIdAsync(Guid id)
        {
            var result = await _customersBankServices.GetCustomersBankByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("create-customers-bank-async")]
        public async Task<IActionResult> CreateCustomersBankAsync([FromBody] CreateOrUpdateCustomersBankDTO customersBank)
        {
            var result = await _customersBankServices.CreateCustomersBankAsync(customersBank);
            return Ok(result);
        }

        [HttpDelete("delete-customers-bank-async/{id}")]
        public async Task<IActionResult> DeleteCustomersBankAsync(Guid id)
        {
            var result = await _customersBankServices.DeleteCustomersBankAsync(id);
            return Ok(result);
        }

        [HttpPut("update-customers-bank-async/{id}")]
        public async Task<IActionResult> UpdateCustomersBankAsync(Guid id, [FromBody] CreateOrUpdateCustomersBankDTO customersBank)
        {
            var result = await _customersBankServices.UpdateCustomersBankAsync(customersBank);
            return Ok(result);
        }
    }
}
