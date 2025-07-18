using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeoMeo.Domain.Entities;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using MeoMeo.Domain.IRepositories;
using MeoMeo.Contract.DTOs;
using MeoMeo.Application.IServices;
using MeoMeo.Domain.Commons;
using MeoMeo.Contract.Commons;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeasonsController : ControllerBase
    {
        private readonly ISeasonService _seasonServices;

        public SeasonsController(ISeasonService context)
        {
            _seasonServices = context;
        }

        [HttpGet("get-all-season-async")]
        public async Task<PagingExtensions.PagedResult<SeasonDTO>> GetAllSeasonsAsync([FromQuery] GetListSeasonRequestDTO request)
        {
            var result = await _seasonServices.GetAllSeasonsAsync(request);
            return result;
        }

        [HttpGet("find-season-by-id-async/{id}")]
        public async Task<SeasonDTO> GetSeasonByIdAsync(Guid id)
        {
            var result = await _seasonServices.GetSeasonByIdAsync(id);
            return result;
        }

        [HttpPost("create-season-async")]
        public async Task<CreateOrUpdateSeasonResponseDTO> CreateSeasonAsync([FromBody] CreateOrUpdateSeasonDTO dto)
        {
            var result = await _seasonServices.CreateSeasonAsync(dto);
            return result;


        }

        [HttpDelete("delete-season-async/{id}")]
        public async Task<bool> DeleteSeasonAsync(Guid id)
        {
            var result = await _seasonServices.DeleteSeasonAsync(id);
            return result;
        }

        [HttpPut("update-season-async/{id}")]
        public async Task<CreateOrUpdateSeasonResponseDTO> UpdateSeasonAsync(Guid id, [FromBody] CreateOrUpdateSeasonDTO dto)
        {
            var result = await _seasonServices.UpdateSeasonAsync(dto);
            return result;

        }
    }
}
