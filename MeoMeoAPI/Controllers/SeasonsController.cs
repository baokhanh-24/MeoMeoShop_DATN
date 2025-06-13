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

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeasonsController : ControllerBase
    {
        private readonly ISeasonServices _seasonServices;

        public SeasonsController(ISeasonServices context)
        {
            _seasonServices = context;
        }

        // GET: api/Seasons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Season>>> Getseasons()
        {
            var x = await _seasonServices.GetAllSeasonsAsync();
            return Ok(x);
        }

        // GET: api/Seasons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Season>> GetSeason(Guid id)
        {
            var season = await _seasonServices.GetSeasonByIdAsync(id);

            if (season == null)
            {
                return NotFound();
            }

            return Ok(season);
        }

        // PUT: api/Seasons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSeason(Guid id, [FromBody] SeasonDTO dto)
        {
            await _seasonServices.UpdateSeasonAsync(id, dto);
            return Ok();
        }

        // POST: api/Seasons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Season>> PostSeason([FromBody] SeasonDTO dto)
        {
            var id = await _seasonServices.CreateSeasonAsync(dto);
            return CreatedAtAction("GetSeason", new { id = id.Id }, id);
        }

        // DELETE: api/Seasons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeason(Guid id)
        {
            await _seasonServices.DeleteSeasonAsync(id);
            return Ok();
        }
    }
}
