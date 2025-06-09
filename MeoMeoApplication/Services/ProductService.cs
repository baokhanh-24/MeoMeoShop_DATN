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
            var mappedProduct = _mapper.Map<Product>(product);
            mappedProduct.Id = Guid.NewGuid();
            return await _repository.AddAsync(mappedProduct);
        }

        public Task<Product> GetProductAsync(Guid id)
        {
            return _repository.GetProductAsync(id);
        }
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddAsync(Product entity)
        {
            await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(Product entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}

