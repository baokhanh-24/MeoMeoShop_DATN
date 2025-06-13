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
        private readonly MeoMeoDbContext _context;
        private readonly ICartDetaillRepository _cartDetailRepo;
        public CartDetaillController(MeoMeoDbContext context, ICartDetaillRepository cartDetaillRepo)
        {
            _context = context;
            _cartDetailRepo = cartDetaillRepo;
        }
        //
        [HttpGet("GetAllCartDetail")]
        public async Task<IActionResult> GetAllCartDetai() 
        {
            var cartDetail = await _cartDetailRepo.GetAllCartDetail();
            return Ok(cartDetail);
        }
        //
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCartDetailById(Guid id)
        {
            var img = await _cartDetailRepo.GetCartDetailById(id);
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
                Id = cartDetailDTO.Id ?? Guid.NewGuid(),
                ProductDetailId = cartDetailDTO.ProductId,
                PromotionDetailId = cartDetailDTO.PonmotionId,
                Discount = cartDetailDTO.Discount,
                Price = cartDetailDTO.Price,
                Quantity= cartDetailDTO.Quantity,
            };

            try
            {
                await _cartDetailRepo.Create(newCartDetaill);
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
                var entity = await _cartDetailRepo.GetCartDetailById(id);
                //if (entity == null)
                //{
                //    return NotFound($"CartDetaill with ID {id} not found.");
                //}

                // Gán dữ liệu từ DTO sang Entity
                entity.CartId = dto.CartId;
                entity.ProductDetailId = dto.ProductId;
                entity.PromotionDetailId = dto.PonmotionId;
                entity.Discount = dto.Discount;
                entity.Price = dto.Price;
                entity.Quantity = dto.Quantity;

                await _cartDetailRepo.Update(dto.CartId, dto.ProductId, dto.Quantity);

                return Ok("Update successful.");
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
            await _cartDetailRepo.Delete(id);
            return Ok("Xóa giỏ hàng chi tiết thành công.");
        }
    }
}
