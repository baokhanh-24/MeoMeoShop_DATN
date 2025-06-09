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
    }
    public async Task<Product> CreateProductDetailAsync(CreateOrUpdateProductDTO product)
    {
        var mappedProduct = _mapper.Map<Product>(product);
        mappedProduct.Id = Guid.NewGuid();
        return await _repository.AddAsync(mappedProduct);
    }
    public async Task<IEnumerable<ProductDetail>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<ProductDetail?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task AddAsync(ProductDetail entity)
    {
        await _repository.AddAsync(entity);
    }

    public async Task UpdateAsync(ProductDetail entity)
    {
        await _repository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }
}
}
