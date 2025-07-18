
﻿using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.Services
{
    public class SeasonService : ISeasonService
    {
        private readonly ISeasonRepository _seasonRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public SeasonService(ISeasonRepository seasonRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _seasonRepository = seasonRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        public async Task<PagingExtensions.PagedResult<SeasonDTO>> GetAllSeasonsAsync(GetListSeasonRequestDTO request)
        {
            var query = _seasonRepository.Query();

            if (!string.IsNullOrWhiteSpace(request.NameFilter))
                query = query.Where(x => EF.Functions.Like(x.Name, $"%{request.NameFilter}%"));

            if (request.StatusFilter.HasValue)
                query = query.Where(x => x.Status == request.StatusFilter.Value);

            query = query.OrderByDescending(x => x.Name);

            var pagedResult = await _seasonRepository.GetPagedAsync(query, request.PageIndex, request.PageSize);
            var dtoItems = _mapper.Map<List<SeasonDTO>>(pagedResult.Items);

            return new PagingExtensions.PagedResult<SeasonDTO>
            {
                TotalRecords = pagedResult.TotalRecords,
                PageIndex = pagedResult.PageIndex,
                PageSize = pagedResult.PageSize,
                Items = dtoItems
            };
        }

        public async Task<SeasonDTO> GetSeasonByIdAsync(Guid id)
        {
            var season = await _seasonRepository.GetSeasonByIDAsync(id);
            return _mapper.Map<SeasonDTO>(season);
        }

        public async Task<CreateOrUpdateSeasonResponseDTO> CreateSeasonAsync(CreateOrUpdateSeasonDTO dto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var isCodeExist = await _seasonRepository.AnyAsync(x => x.Name == dto.Name);
                if (isCodeExist)
                {
                    return new CreateOrUpdateSeasonResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Name đã tồn tại."
                    };
                }

                var entity = _mapper.Map<Season>(dto);
                entity.Id = Guid.NewGuid();

                var created = await _seasonRepository.CreateSeasonAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                var response = _mapper.Map<CreateOrUpdateSeasonResponseDTO>(created);
                response.ResponseStatus = BaseStatus.Success;
                response.Message = "Tạo mùa thành công";

                return response;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new CreateOrUpdateSeasonResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = ex.Message
                };
            }
        }

        public async Task<CreateOrUpdateSeasonResponseDTO> UpdateSeasonAsync(CreateOrUpdateSeasonDTO dto)
        {
          

            
                var exists = await _seasonRepository.AnyAsync(x => x.Name == dto.Name && x.Id != dto.Id);
                if (exists)
                {
                    return new CreateOrUpdateSeasonResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Name đã tồn tại."
                    };
                }

                var seasonUpdate = await _seasonRepository.GetSeasonByIDAsync(dto.Id);
                if (seasonUpdate == null)
                { 
                    return new CreateOrUpdateSeasonResponseDTO
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Mùa không tồn tại."
                    };
                }

                _mapper.Map(dto, seasonUpdate);

                var result = await _seasonRepository.UpdateSeasonAsync(seasonUpdate);
                return _mapper.Map<CreateOrUpdateSeasonResponseDTO>(result);
            }
            

        public async Task<bool> DeleteSeasonAsync(Guid id)
        {
            var seasonDelete = await _seasonRepository.GetSeasonByIDAsync(id);
            if (seasonDelete == null)
            {
                return false;
            }
            await _seasonRepository.DeleteSeasonAsync(seasonDelete.Id);
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

        async Task<CreateOrUpdateSeasonResponse> ISeasonService.CreateSeasonAsync(SeasonDTO dto)
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


        async Task<CreateOrUpdateSeasonResponse> ISeasonService.UpdateSeasonAsync(SeasonDTO dto)
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
