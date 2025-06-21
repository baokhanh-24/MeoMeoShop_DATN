using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
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

        public async Task<CreateOrUpdateSeasonResponse> GetSeasonByIdAsync(Guid id)
        {
            CreateOrUpdateSeasonResponse response = new CreateOrUpdateSeasonResponse();
            var season = await _seasonRepository.GetSeasonByID(id);
            if (season == null)
            {
                response.Message = "Không tìm thấy mùa này";
                response.ResponseStatus = BaseStatus.Error;
                return response;
            }
            response.Id = season.Id;
            response.Name = season.Name;
            response.Description = season.Description;
            response.Status = season.Status;
            response.Message = "Lấy thông tin mùa thành công";
            response.ResponseStatus = BaseStatus.Success;
            return response;
        }

        async Task<CreateOrUpdateSeasonResponse> ISeasonServices.CreateSeasonAsync(SeasonDTO dto)
        {
            CreateOrUpdateSeasonResponse response = new CreateOrUpdateSeasonResponse();
            var newseason = new Season
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Status = dto.Status
            };
            await _seasonRepository.CreateAsync(newseason);
            response.Id = newseason.Id;
            response.Name = newseason.Name;
            response.Description = newseason.Description;
            response.Status = newseason.Status;
            response.Message = "Tạo mùa thành công";
            response.ResponseStatus = BaseStatus.Success;
            return response;
        }


        async Task<CreateOrUpdateSeasonResponse> ISeasonServices.UpdateSeasonAsync(SeasonDTO dto)
        {
            CreateOrUpdateSeasonResponse response = new CreateOrUpdateSeasonResponse();
            var getId = await _seasonRepository.GetSeasonByID(dto.Id);
            if(getId == null)
            {
                response.Message = "Không tìm thấy mùa này";
                response.ResponseStatus = BaseStatus.Error;
                return response;
            }
            getId.Name = dto.Name;
            getId.Description = dto.Description;
            getId.Status = dto.Status;

            await _seasonRepository.UpdateAsync(getId);

            response.Message = "Cập nhật mùa thành công";
            response.ResponseStatus = BaseStatus.Success;
            return response;
        }
    }
}
