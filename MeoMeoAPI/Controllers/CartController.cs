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
        private readonly ICartRepository _cartRepo;
        private readonly MeoMeoDbContext _context;
        public CartController(ICartRepository cartRepo, MeoMeoDbContext context)
        {
            _cartRepo = cartRepo;
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCart()
        {
            try
            {
                return await _cartRepo.GetAllCart();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
        //
        [HttpGet("GetCartById")]
        public async Task<ActionResult<Cart>> GetByIdCart(Guid id)
        {
            try
            {
                return await _cartRepo.GetCartById(id);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
        //add
        [HttpPost]
        public async Task<ActionResult<Cart>> PostCart(Cart cart)
        {
            try
            {
                Cart ct = new Cart()
                {
                    Id = cart.Id,
                    Customers = cart.Customers,
                    CustomerId = cart.CustomerId
                };
                await _cartRepo.Create(ct);
                await _cartRepo.Savechanges();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
            return Content("Thanh cong");
        }
        //
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCart(Cart cart)
        {
            try
            {
                Cart ct = new Cart()
                {
                    Id = cart.Id,
                    Customers = cart.Customers,
                    CustomerId = cart.CustomerId
                };
                await _cartRepo.Create(ct);
                await _cartRepo.Savechanges();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
            return Content("Thanh cong");

        }
    }
}
