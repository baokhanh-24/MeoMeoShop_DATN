using Humanizer;
using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        //
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _cartService.GetAllCartsAsync();
            return Ok(result);
        }
        //
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _cartService.GetCartByIdAsync(id);
            return Ok(result);
        }
        //add
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CartDTO cartDTO)
        {
            var result = await _cartService.CreateCartAsync(cartDTO);
            return Ok(result);
        }
        //
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CartDTO cartDto)
        {
            var result = await _cartService.UpdateCartAsync(cartDto);
            return Ok(result);
        }
    }
}
