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
using MeoMeo.Domain.Commons;
using Microsoft.EntityFrameworkCore;

namespace MeoMeo.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductsDetailRepository _productDetailRepository;
        private readonly IMapper _mapper;
        public CategoryService(
            ICategoryRepository categoryRepository,
            IProductCategoryRepository productCategoryRepository,
            IProductRepository productRepository,
            IProductsDetailRepository productDetailRepository,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _productCategoryRepository = productCategoryRepository;
            _productRepository = productRepository;
            _productDetailRepository = productDetailRepository;
            _mapper = mapper;
        }

        public async Task<CategoryResponseDTO> CreateCategoryAsync(CategoryDTO categoryDTO)
        {
            // Check trùng Name
            var isNameExist = await _categoryRepository.AnyAsync(x => x.Name == categoryDTO.Name);
            if (isNameExist)
            {
                return new CategoryResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Tên danh mục đã tồn tại."
                };
            }


            var category = _mapper.Map<Category>(categoryDTO);
            category.Id = Guid.NewGuid();

            await _categoryRepository.AddAsync(category);
            return new CategoryResponseDTO
            {
                ResponseStatus = BaseStatus.Success,
                Message = "Thêm danh mục thành công"
            };
        }

        public async Task<CategoryResponseDTO> DeleteCategoryAsync(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return new CategoryResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy Id" };
            }

            await _categoryRepository.DeleteAsync(id);
            return new CategoryResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Xóa thành công" };
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories;
        }

        public async Task<PagingExtensions.PagedResult<CategoryDTO>> GetAllCategoriesPagedAsync(GetListCategoryRequestDTO request)
        {
            try
            {
                var query = _categoryRepository.Query();

                if (!string.IsNullOrEmpty(request.NameFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.Name, $"%{request.NameFilter}%"));
                }
                

                if (!string.IsNullOrEmpty(request.DescriptionFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.Description, $"%{request.DescriptionFilter}%"));
                }

                if (request.StatusFilter.HasValue)
                {
                    query = query.Where(c => c.Status == request.StatusFilter);
                }

                query = query.OrderByDescending(c => c.Name);

                var pagedResult = await _categoryRepository.GetPagedAsync(query, request.PageIndex, request.PageSize);
                var dtoItems = _mapper.Map<List<CategoryDTO>>(pagedResult.Items);

                return new PagingExtensions.PagedResult<CategoryDTO>
                {
                    TotalRecords = pagedResult.TotalRecords,
                    PageIndex = pagedResult.PageIndex,
                    PageSize = pagedResult.PageSize,
                    Items = dtoItems
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllCategoriesPagedAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<CategoryResponseDTO> GetCategoryByIdAsync(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return new CategoryResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = $"Không tìm thấy Category với ID: {id}"
                };
            }

            var response = _mapper.Map<CategoryResponseDTO>(category);
            response.ResponseStatus = BaseStatus.Success;
            response.Message = "Lấy dữ liệu thành công";
            return response;
        }

        public async Task<CategoryResponseDTO> UpdateCategoryAsync(CategoryDTO categoryDTO)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryDTO.Id);
            if (category == null)
            {
                return new CategoryResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy danh mục cần cập nhật" };
            }

            // Check trùng Name (trừ record hiện tại)
            var isNameExist = await _categoryRepository.AnyAsync(x => x.Name == categoryDTO.Name && x.Id != categoryDTO.Id);
            if (isNameExist)
            {
                return new CategoryResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Tên danh mục đã tồn tại."
                };
            }

            _mapper.Map(categoryDTO, category);

            await _categoryRepository.UpdateAsync(category);

            return new CategoryResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Cập nhật danh mục thành công" };
        }

        public async Task<MeoMeo.Contract.DTOs.Product.CategoryHoverResponseDTO> GetCategoryHoverPreviewAsync(
            Guid categoryId, int take = 6)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                return new MeoMeo.Contract.DTOs.Product.CategoryHoverResponseDTO
                {
                    CategoryId = categoryId,
                    CategoryName = string.Empty,
                    Products = new List<MeoMeo.Contract.DTOs.Product.ProductPreviewDTO>()
                };
            }

            var productIds = await _productCategoryRepository.Query()
                .Where(pc => pc.CategoryId == categoryId)
                .Select(pc => pc.ProductId)
                .Distinct()
                .ToListAsync();

            var products = await _productRepository.Query()
                .Where(p => productIds.Contains(p.Id))
                .Take(take)
                .ToListAsync();

            var minMaxPrices = await _productDetailRepository.Query()
                .Where(pd => productIds.Contains(pd.ProductId))
                .GroupBy(pd => pd.ProductId)
                .Select(g => new { ProductId = g.Key, MinPrice = g.Min(x => x.Price), MaxPrice = g.Max(x => x.Price) })
                .ToListAsync();

            var priceDict = minMaxPrices.ToDictionary(x => x.ProductId,
                x => (Min: (float?)x.MinPrice, Max: (float?)x.MaxPrice));

            var result = new MeoMeo.Contract.DTOs.Product.CategoryHoverResponseDTO
            {
                CategoryId = category.Id,
                CategoryName = category.Name,
                Products = products.Select(p => new MeoMeo.Contract.DTOs.Product.ProductPreviewDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Thumbnail = p.Thumbnail,
                    MinPrice = priceDict.ContainsKey(p.Id) ? priceDict[p.Id].Min : null,
                    MaxPrice = priceDict.ContainsKey(p.Id) ? priceDict[p.Id].Max : null
                }).ToList()
            };

            return result;
        }
    }


}