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
            var images = await _cartDetaillService.GetAllCartDetaillAsync();
            var result = images.Select(p => new CartDetailDTO
            {
                Id = p.Id,
                CartId = p.CartId,
                ProductId = p.ProductDetailId,
                PonmotionId = p.PromotionDetailId,
                Discount = p.Discount,
                Quantity = p.Quantity,
                Price = p.Price,
            }).ToList();

            return Ok(result);
        }
        //
        [HttpGet("{id}")]
        public async Task<ActionResult<CartDetailDTO>> GetCartDetailById(Guid id)
        {
            var img = await _cartDetaillService.GetCartDetaillByIdAsync(id);
            if (img == null) return NotFound();

            var dto = new CartDetailDTO
            {
                Id = img.Id,
                ProductId = img.ProductDetailId,
                PonmotionId = img.PromotionDetailId,
                Discount = img.Discount,
                Price = img.Price,
                Quantity = img.Quantity,
            };

            return Ok(dto);
        }
        //
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CartDetailDTO cartDetailDTO)
        {
            var newCartDetaill = new CartDetail
            {
                ProductDetailId = cartDetailDTO.ProductId,
                PromotionDetailId = cartDetailDTO.PonmotionId,
                Discount = cartDetailDTO.Discount,
                Price = cartDetailDTO.Price,
                Quantity= cartDetailDTO.Quantity,
            };

            try
            {
                await _cartDetaillService.CreateCartDetaillAsync(cartDetailDTO);
                return CreatedAtAction(nameof(GetCartDetailById), new { id = newCartDetaill.Id }, cartDetailDTO);
            }
            catch (DuplicateWaitObjectException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
        //
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CartDetailDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                // Tìm entity hiện tại
                var entity = await _cartDetaillService.UpdataCartDetaillAsync(dto);
                if (entity == null)
                {
                    return NotFound($"Image with ID {id} not found.");
                }

                await _cartDetaillService.UpdataCartDetaillAsync(dto);

                return Ok("Sửa ảnh thành công.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        //
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _cartDetaillService.DeleteCartDetaillAsync(id);
                return Ok(new { message = "Xóa ảnh thành công", result });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa ảnh", detail = ex.Message });
            }
        }
    }
}
