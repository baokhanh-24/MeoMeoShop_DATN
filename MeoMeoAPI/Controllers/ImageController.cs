using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }
        // GET: api/image
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _imageService.GetAllImagesAsync();
            return Ok(result);
        }
        // GET: api/image/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _imageService.GetImageByIdAsync(id);
            return Ok(result);
        }
        // POST: api/image
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ImageDTO imageDto)
        {
            var result = await _imageService.CreateImageAsync(imageDto);
            return Ok(result);
        }
        // PUT: api/image/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ImageDTO dto)
        {
            var result = await _imageService.UpdateImageAsync(dto);
            return Ok(result);
        }
        // DELETE: api/image/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _imageService.DeleteImageAsync(id);
            return Ok(result);
        }
    }
}
