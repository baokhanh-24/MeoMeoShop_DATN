using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeServices _employeeServices;

        public EmployeesController(IEmployeeServices employeeServices)
        {
            _employeeServices = employeeServices;
        }

        [HttpGet("get-all-employee-async")]
        public async Task<IActionResult> GetAllEmployeeAsync()
        {
            var result = await _employeeServices.GetAllEmployeeAsync();
            return Ok(result);
        }

        [HttpGet("find-employee-by-id-async/{id}")]
        public async Task<IActionResult> GetEmployeeByIdAsync(Guid id)
        {
            var result = await _employeeServices.GetEmployeeByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("create-employee-async")]
        public async Task<IActionResult> CreateEmployeeAsync([FromBody] CreateOrUpdateEmployeeDTO employee)
        {
            var result = await _employeeServices.CreateEmployeeAsync(employee);
            return Ok(result);
        }

        [HttpDelete("delete-employee-async/{id}")]
        public async Task<IActionResult> DeleteEmployeeAsync(Guid id)
        {
            var result = await _employeeServices.DeleteEmployeeAsync(id);
            return Ok(result);
        }

        [HttpPut("update-employee-async/{id}")]
        public async Task<IActionResult> UpdateEmployeeAsync(Guid id, [FromBody] CreateOrUpdateEmployeeDTO employee)
        {
            var result = await _employeeServices.UpdateEmployeeAsync(employee);
            return Ok(result);
        }
    }
}
