using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;

namespace MeoMeo.Application.Services
{
    public class CommuneService : ICommuneService
    {
        private readonly ICommuneRepository _communeRepository;
        private readonly IMapper _mapper;

        public CommuneService(ICommuneRepository communeRepository, IMapper mapper)
        {
            _communeRepository = communeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Commune>> GetAllAsync()
        {
            return await _communeRepository.GetAllAsync();
        }

        public async Task<Commune> GetByIdAsync(Guid id)
        {
            return await _communeRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Commune>> GetByDistrictIdAsync(Guid districtId)
        {
            var query = _communeRepository.Query().Where(c => c.DistrictId == districtId);
            return await Task.FromResult(query.AsEnumerable());
        }

        public async Task<Commune> CreateAsync(CreateOrUpdateCommuneDTO commune)
        {
            var mappedCommune = _mapper.Map<Commune>(commune);
            mappedCommune.Id = Guid.NewGuid();
            var result = await _communeRepository.CreateAsync(mappedCommune);
            return result;
        }

        public async Task<Commune> UpdateAsync(CreateOrUpdateCommuneDTO commune)
        {
            if (!commune.Id.HasValue)
                throw new ArgumentException("Id is required for update");
                
            var existingCommune = await _communeRepository.GetByIdAsync(commune.Id.Value);
            if (existingCommune == null)
                throw new ArgumentException("Commune not found");
                
            existingCommune.Name = commune.Name;
            existingCommune.Code = commune.Code;
            existingCommune.DistrictId = commune.DistrictId;
                
            var result = await _communeRepository.UpdateAsync(existingCommune);
            return result;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var commune = await _communeRepository.GetByIdAsync(id);
            if (commune == null)
                return false;
                
            await _communeRepository.DeleteAsync(commune);
            return true;
        }
    }
}
