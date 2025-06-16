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
    public class ProductService : IProductServices
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Product> CreateProductAsync(CreateOrUpdateProductDTO product)
        {
            var mapper = _mapper.Map<Product>(product);
            mapper.Id = Guid.NewGuid();
            var result = await _repository.AddProductAsync(mapper);
            return result;
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _repository.GetProductByIdAsync(id);
                
            if (product == null)
            {
                throw new Exception("Sản phẩm không tồn tại.");
            }
            await _repository.DeleteProductAsync(id);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _repository.GetAllProductAsync();
        }

        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            return await _repository.GetProductByIdAsync(id);
        }

        public async Task UpdateAsync(Product model)
        {
            var product = await _repository.GetProductByIdAsync(model.Id); 
            if (product == null)
                throw new Exception("Sản phẩm không tồn tại.");
            _mapper.Map(model, product);


            product.Name = model.Name;
            product.Thumbnail = model.Thumbnail;
            product.Brand = model.Brand;
            product.BrandId = model.BrandId;

            await _repository.UpdateProductAsync(product);
        }
    }
}

