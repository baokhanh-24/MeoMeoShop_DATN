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

        public async Task<ProductReponseDTO> DeleteAsync(Guid id)
        {
                var response = new ProductReponseDTO();

                var product = await _repository.GetProductByIdAsync(id);
                if (product == null)
                {
                    response.Message = "Không tìm thấy sản phẩm để xóa.";
                    response.ResponseStatus = BaseStatus.Error;
                    return response;
                }

                await _repository.DeleteProductAsync(id);

                // Nếu muốn trả về thông tin sản phẩm đã xóa
                response = _mapper.Map<ProductReponseDTO>(product);
                response.Message = "Xóa sản phẩm thành công.";
                response.ResponseStatus = BaseStatus.Success;

                return response;
            }

        

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _repository.GetAllProductAsync();
        }

        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            return await _repository.GetProductByIdAsync(id);
        }

        public async Task<ProductReponseDTO> UpdateAsync(CreateOrUpdateProductDTO model)
        {
            ProductReponseDTO response = new ProductReponseDTO();
            Product updateProduct = new Product();

            var product = await _repository.GetProductByIdAsync(Guid.Parse(model.Id.ToString()));
            if (product == null)
            {
                response.Message = "Khong tim thay san pham";
                response.ResponseStatus = BaseStatus.Error;
                return response;
            }

            updateProduct = _mapper.Map<Product>(product);
            var result = await _repository.UpdateAsync(updateProduct);
            response = _mapper.Map<ProductReponseDTO>(result);
            response.Message = "";
            response.ResponseStatus = BaseStatus.Success;
            return response;
        }
    }
}

