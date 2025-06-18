using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
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
        private readonly ISeasonRepository _seasonRepository;
        public SeasonService(ISeasonRepository seasonRepository)
        {
            _seasonRepository = seasonRepository;
        }
        public async Task<Season> CreateSeasonAsync(SeasonDTO dto)
        {
            var season = new Season
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Status = dto.Status
            };
            await _seasonRepository.CreateAsync(season);
            return season;
        }

        public async Task<bool> DeleteSeasonAsync(Guid id)
        {
            var phat = await _seasonRepository.GetSeasonByID(id);
            if (phat == null)
            {
                return false;
            }
            await _seasonRepository.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<Season>> GetAllSeasonsAsync()
        {
            return await _seasonRepository.GetSeasonsAsync();
        }

        public async Task<Season> GetSeasonByIdAsync(Guid id)
        {
            return await _seasonRepository.GetSeasonByID(id);
        }

        public async Task<Season> UpdateSeasonAsync(SeasonDTO dto)
        {
            var phat = await _seasonRepository.GetSeasonByID(dto.Id);
            if (phat == null)
            {
                return null;
            }
            phat.Name = dto.Name;
            phat.Description = dto.Description;
            phat.Status = dto.Status;
            await _seasonRepository.UpdateAsync(phat);
            return phat;
        }
    }
}
