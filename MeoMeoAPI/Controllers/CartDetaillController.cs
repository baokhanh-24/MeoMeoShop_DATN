using Humanizer;
using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartDetaillController : ControllerBase
    {
        private readonly ICartDetaillService _cartDetaillService;
        public CartDetaillController(ICartDetaillService cartDetaillService)
        {
            _cartDetaillService = cartDetaillService;
        }
        //
        [HttpGet("GetAllCartDetail")]
        public async Task<IActionResult> GetAllCartDetai() 
        {
            var result = await _cartDetaillService.GetAllCartDetaillAsync();
            return Ok(result);
        }
        //
        [HttpGet("{id}")]
        public async Task<ActionResult<CartDetailDTO>> GetCartDetailById(Guid id)
        {
            var result = await _cartDetaillService.GetCartDetaillByIdAsync(id);
            return Ok(result);
        }
        //
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CartDetailDTO cartDetailDTO)
        {
            var result = await _cartDetaillService.CreateCartDetaillAsync(cartDetailDTO);
            return Ok(result);
        }
        //
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CartDetailDTO dto)
        {
            var result = await _cartDetaillService.UpdataCartDetaillAsync(dto);
            return Ok(result);
        }
        //
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _cartDetaillService.DeleteCartDetaillAsync(id);
            return Ok(result);
        }
    }
}
