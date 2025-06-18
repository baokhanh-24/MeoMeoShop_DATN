using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;

namespace MeoMeo.Application.Services
{
    public class ProvinceService : IProvinceService
    {
        private readonly IProvinceRepository _provinceRepository;
        private readonly IMapper _mapper;

        public ProvinceService(IProvinceRepository provinceRepository, IMapper mapper)
        {
            _provinceRepository = provinceRepository;
            _mapper = mapper;
        }

        public async Task<Province> CreateProvinceAsync(CreateOrUpdateProvinceDTO province)
        {
            var mappedProvince = _mapper.Map<Province>(province);
            mappedProvince.Id = Guid.NewGuid();
            var result = await _provinceRepository.CreateProvinceAsync(mappedProvince);
            return result;
        }

        public async Task<bool> DeleteProvinceAsync(Guid id)
        {
            var checkProvince = await _provinceRepository.GetProvinceByIdsync(id);
            if (checkProvince == null)
            {
                return false;
            }

            await _provinceRepository.DeleteProvinceAsync(checkProvince.Id);
            return true;


        }

        public Task<IEnumerable<Province>> GetAllProvinceAsync()
        {
            return _provinceRepository.GetAllProvinceAsync();
        }

        public async Task<Province> GetProvinceByIdAsync(Guid id)
        {
            var result = await _provinceRepository.GetProvinceByIdsync(id);
            return result;
        }

        public async Task<Province> UpdateProvinceAsync(CreateOrUpdateProvinceDTO province)
        {
            Province itemProvince = new Province();
            itemProvince = _mapper.Map<Province>(province);
            //if (itemProvince.Id == province.Id)
            //{

            //}
            var result = await _provinceRepository.UpdateProvinceAsync(itemProvince);
            return result;

        }
    }
}
