using AutoMapper;
using MeoMeo.Application.IServices;
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

        public async Task<ProductDetail> DeleteProductDetailAsync(Guid id)
        {
            var existing = await _repository.GetByProductIdAsync(id);
            if (existing == null)
                throw new Exception("Sản phẩm không tồn tại.");

            await _repository.DeleteAsync(id);

            return existing; 
        }

        public async Task<IEnumerable<ProductDetail>> GetProductDetailAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ProductDetail> GetProductDetailByIdAsync(Guid id)
        {
          return await _repository.GetByProductIdAsync(id);
        }

        public async Task<ProductDetail> UpdateProductDetailAsync(CreateOrUpdateProductDetailDTO entity)
        {
            var existing = await _repository.GetByProductIdAsync(entity.Id);
            if (existing == null)
                throw new Exception("Sản phẩm không tồn tại.");

           
            _mapper.Map(entity, existing);

            await _repository.UpdateProductAsync(existing);

            return existing;
        }
    }
}




