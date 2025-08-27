using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;

namespace MeoMeo.Application.Services
{
    public class DistrictService : IDistrictService
    {
        private readonly IDistrictRepository _districtRrepository;
        private readonly IMapper _mapper;

        public DistrictService(IDistrictRepository districtRrepository, IMapper mapper)
        {
            _districtRrepository = districtRrepository;
            _mapper = mapper;
        }

        public async Task<District> CreateDistrictAsync(CreateOrUpdateDistrictDTO district)
        {
            var mappedDistrict = _mapper.Map<District>(district);
            mappedDistrict.Id = Guid.NewGuid();
            var result = await _districtRrepository.CreateDistrictAsync(mappedDistrict);
            return result;
        }

        public async Task<bool> DeleteDistrictAsync(Guid id)
        {
            var checkDistrict = await _districtRrepository.GetDistrictByIdAsync(id);
            if (checkDistrict.Id != id)
            {
                return false;
            }
            else
            {
                await _districtRrepository.DeleteDistrictAsync(checkDistrict.Id);
                return true;
            }
        }

        public Task<IEnumerable<District>> GetAllAsync()
        {
            return _districtRrepository.GetAllAsync();
        }

        public async Task<IEnumerable<District>> GetByProvinceIdAsync(Guid provinceId)
        {
            var query = _districtRrepository.Query().Where(d => d.ProvinceId == provinceId);
            return await Task.FromResult(query.AsEnumerable());
        }

        public async Task<CreateOrUpdateDistrictRespose> GetDistrictByIdAsync(Guid id)
        {
            CreateOrUpdateDistrictRespose respose = new CreateOrUpdateDistrictRespose();
            var result = await _districtRrepository.GetDistrictByIdAsync(id);
            if (result == null)
            {
                respose.Message = "Không tìm thấy huyện này";
                respose.ResponseStatus = BaseStatus.Error;
                return respose;
            }
            respose = _mapper.Map<CreateOrUpdateDistrictRespose>(result);
            respose.Message = "";
            respose.ResponseStatus = BaseStatus.Success;
            return respose;
        }

        public async Task<CreateOrUpdateDistrictRespose> UpdateDistrictAsync(CreateOrUpdateDistrictDTO district)
        {
            CreateOrUpdateDistrictRespose respose = new CreateOrUpdateDistrictRespose();
            District itemDistrict = new District();
            var check = await _districtRrepository.GetDistrictByIdAsync(district.Id);
            if (check == null)
            {
                respose.Message = "Không tìm thấy huyện này, không thể update";
                respose.ResponseStatus = BaseStatus.Error;
                return respose;
            }
            itemDistrict = _mapper.Map(district, check);
            var result = await _districtRrepository.UpdateDistrictAsync(itemDistrict);
            respose = _mapper.Map<CreateOrUpdateDistrictRespose>(result);
            respose.Message = "";
            respose.ResponseStatus = BaseStatus.Success;
            return respose;
        }
    }
}
