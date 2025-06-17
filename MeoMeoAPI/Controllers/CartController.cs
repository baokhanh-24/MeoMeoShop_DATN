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
            var images = await _cartService.GetAllCartsAsync();
            var result = images.Select(img => new CartDTO
            {
                Id = img.Id,
                CustomersId = img.CustomerId,
                createBy = img.CreatedBy,
                NgayTao = img.CreationTime,
                TongTien = img.TotalPrice,
            }).ToList();

            return Ok(result);
        }
        //
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var img = await _cartService.GetCartByIdAsync(id);
            if (img == null) return NotFound();

            var dto = new CartDTO
            {
                Id = img.Id,
                CustomersId = img.CustomerId,
                createBy = img.CreatedBy,
                NgayTao = img.CreationTime,
                TongTien = img.TotalPrice,
            };

            return Ok(dto);
        }
        //add
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CartDTO cartDTO)
        {
            var newCart = new Cart
            {
                Id = cartDTO.Id,
                CustomerId = cartDTO.CustomersId,
                TotalPrice = cartDTO.TongTien,
                CreationTime = cartDTO.NgayTao,
                LastModificationTime = cartDTO.lastModificationTime,

            };
            try
            {
                await _cartService.CreateCartAsync(cartDTO);
                return CreatedAtAction(nameof(GetById), new { id = newCart.Id }, cartDTO);
            }
            catch (DuplicateWaitObjectException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
        //
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CartDTO cartDto)
        {
            if (id != cartDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                // Tìm entity hiện tại
                var entity = await _cartService.UpdateCartAsync(cartDto);
                if (entity == null)
                {
                    return NotFound($"Image with ID {id} not found.");
                }

                await _cartService.UpdateCartAsync(cartDto);

                return Ok("Sửa ảnh thành công.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
