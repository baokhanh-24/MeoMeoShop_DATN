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
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerServices _customerServices;

        public CustomerController(ICustomerServices customerServices)
        {
            _customerServices = customerServices;
        }

        [HttpGet("get-all-customer-async")]
        public async Task<PagingExtensions.PagedResult<CustomerDTO>> GetAllCustomersAsync([FromQuery] GetListCustomerRequestDTO request)
        {
            var result = await _customerServices.GetAllCustomersAsync(request);
            return result;
        }

        [HttpGet("find-customer-by-id-async/{id}")]
        public async Task<CustomerDTO> GetCustomersByIdAsync(Guid id)
        {
            var result = await _customerServices.GetCustomersByIdAsync(id);
            return result;
        }

        [HttpPost("create-customer-async")]
        public async Task<CreateOrUpdateCustomerResponse> CreateCustomersAsync([FromBody] CreateOrUpdateCustomerDTO customer)
        {
            var result = await _customerServices.CreateCustomersAsync(customer);
            return result;
        }

        [HttpDelete("delete-customer-async/{id}")]
        public async Task<bool> DeleteCustomersAsync(Guid id)
        {
            var result = await _customerServices.DeleteCustomersAsync(id);
            return result;
        }

        [HttpPut("update-customer-async/{id}")]
        public async Task<CreateOrUpdateCustomerResponse> UpdateCustomersAsync(Guid id, [FromBody] CreateOrUpdateCustomerDTO customer)
        {
            var result = await _customerServices.UpdateCustomersAsync(customer);
            return result;
        }
    }
}
