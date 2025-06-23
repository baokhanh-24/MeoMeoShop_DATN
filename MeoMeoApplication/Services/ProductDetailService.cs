using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.Services
{
    public class ProductDetailService : IProductDetailServices
    {
        private readonly IProductsDetailRepository _repository;
        private readonly IMapper _mapper;

        public ProductDetailService(IProductsDetailRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ProductDetail> AddProductDetailAsync(CreateOrUpdateProductDetailDTO dto)
        {
            var entity = _mapper.Map<ProductDetail>(dto);
            entity.Id = Guid.NewGuid();
            return await _repository.AddProductAsync(entity);
        }

        public async Task<ProductDetailResponseDTO> DeleteProductDetailAsync(Guid id)
        {
            var response = new ProductDetailResponseDTO();

            var existing = await _repository.GetByProductIdAsync(id);
            if (existing == null)
            {
                response.Message = "Không tìm thấy chi tiết sản phẩm để xóa.";
                response.ResponseStatus = BaseStatus.Error;
                return response;
            }

            await _repository.DeleteAsync(id);

            response = _mapper.Map<ProductDetailResponseDTO>(existing);
            response.Message = "Xóa chi tiết sản phẩm thành công.";
            response.ResponseStatus = BaseStatus.Success;

            return response;
        }

        public async Task<IEnumerable<ProductDetail>> GetProductDetailAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ProductDetail> GetProductDetailByIdAsync(Guid id)
        {
          return await _repository.GetByProductIdAsync(id);
        }

        public async Task<ProductDetailResponseDTO> UpdateProductDetailAsync(CreateOrUpdateProductDetailDTO entity)
        {
            var response = new ProductDetailResponseDTO();

            var existing = await _repository.GetByProductIdAsync(entity.Id);
            if (existing == null)
            {
                response.Message = "Không tìm thấy sản phẩm để cập nhật.";
                response.ResponseStatus = BaseStatus.Error;
                return response;
            }

            _mapper.Map(entity, existing);

            var updated = await _repository.UpdateProductAsync(existing);

            response = _mapper.Map<ProductDetailResponseDTO>(updated);
            response.Message = "Cập nhật sản phẩm chi tiết thành công.";
            response.ResponseStatus = BaseStatus.Success;

            return response;
        }
    }
}




