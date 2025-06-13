using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.Services
{
    public class SeasonService : ISeasonServices
    {
        private readonly MeoMeoDbContext _context;
        public SeasonService(MeoMeoDbContext context)
        {
            _context = context;
        }
        public async Task<SeasonDTO> CreateSeasonAsync(SeasonDTO dto)
        {
            var newSeason = new Domain.Entities.Season
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description
            };
            _context.seasons.Add(newSeason);
            await _context.SaveChangesAsync();
            return new SeasonDTO
            {
                Id = newSeason.Id,
                Name = newSeason.Name,
                Description = newSeason.Description
            };
        }

        public async Task<bool> DeleteSeasonAsync(Guid id)
        {
            var season = await _context.seasons.FindAsync(id);
            if (season == null)
            {
                return false;
            }
            _context.seasons.Remove(season);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<SeasonDTO>> GetAllSeasonsAsync()
        {
            return await _context.seasons.Select(s => new SeasonDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description
                }).ToListAsync();
        }

        public async Task<SeasonDTO> GetSeasonByIdAsync(Guid id)
        {
            var x = await _context.seasons.FindAsync(id);
            if (x == null)
            {
                return null;
            }
            return new SeasonDTO
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            };
        }

        public async Task<SeasonDTO> UpdateSeasonAsync(Guid id, SeasonDTO dto)
        {
            var x = await _context.seasons.FindAsync(id);
            if (x == null)
            {
                throw new Exception("Not found");
            }
            x.Name = dto.Name;
            x.Description = dto.Description;
            _context.seasons.Update(x);
            await _context.SaveChangesAsync();
            return new SeasonDTO
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            };
        }
    }
}
