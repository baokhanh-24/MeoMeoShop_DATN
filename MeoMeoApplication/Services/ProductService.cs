using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Product;
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


        public async Task<BaseResponse> UpdateVariantStatusAsync(UpdateProductStatusDTO input)
        {
            try
            {
                var variant = await _productDetailRepository.GetProductDetailByIdAsync(input.Id);
                if (variant == null)
                {
                    return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy chi tiết sản phẩm" };
                }
                variant.Status = input.Status;
                variant.LastModificationTime = DateTime.Now;
                await _productDetailRepository.UpdateProductDetailAsync(variant);
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                return new BaseResponse { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<List<BestSellerItemDTO>> GetWeeklyBestSellersAsync(int take = 12)
        {
            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(7);

            var soldByProduct = await _orderDetailRepository.Query()
                .Include(od => od.Order)
                .Where(od => od.Order != null && od.Order.CreationTime >= startOfWeek && od.Order.CreationTime < endOfWeek)
                .GroupBy(od => od.ProductDetail.ProductId)
                .Select(g => new { ProductId = g.Key, TotalSold = g.Sum(x => x.Quantity) })
                .OrderByDescending(x => x.TotalSold)
                .Take(take)
                .ToListAsync();

            var productIds = soldByProduct.Select(x => x.ProductId).ToList();
            var products = await _repository.Query()
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p);

            var minMaxPrices = await _productDetailRepository.Query()
                .Where(pd => productIds.Contains(pd.ProductId))
                .GroupBy(pd => pd.ProductId)
                .Select(g => new { ProductId = g.Key, MinPrice = g.Min(x => x.Price), MaxPrice = g.Max(x => x.Price) })
                .ToListAsync();
            var priceDict = minMaxPrices.ToDictionary(x => x.ProductId, x => (Min: (float?)x.MinPrice, Max: (float?)x.MaxPrice));

            var results = new List<BestSellerItemDTO>();
            foreach (var item in soldByProduct)
            {
                if (products.TryGetValue(item.ProductId, out var product))
                {
                    results.Add(new BestSellerItemDTO
                    {
                        ProductId = product.Id,
                        Name = product.Name,
                        Thumbnail = product.Thumbnail,
                        TotalSold = item.TotalSold,
                        MinPrice = priceDict.ContainsKey(product.Id) ? priceDict[product.Id].Min : null,
                        MaxPrice = priceDict.ContainsKey(product.Id) ? priceDict[product.Id].Max : null
                    });
                }
            }

            return results;
        }

        public async Task<CategoryHoverResponseDTO> GetCategoryHoverPreviewAsync(Guid categoryId, int take = 6)
        {
            // Delegate to CategoryService would be ideal, but provide a fallback using existing repos
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                return new CategoryHoverResponseDTO
                {
                    CategoryId = categoryId,
                    CategoryName = string.Empty,
                    Products = new List<ProductPreviewDTO>()
                };
            }

            var productIds = await _productCategoryRepository.Query()
                .Where(pc => pc.CategoryId == categoryId)
                .Select(pc => pc.ProductId)
                .Distinct()
                .ToListAsync();

            var products = await _repository.Query()
                .Where(p => productIds.Contains(p.Id))
                .Take(take)
                .ToListAsync();

            var minMaxPrices = await _productDetailRepository.Query()
                .Where(pd => productIds.Contains(pd.ProductId))
                .GroupBy(pd => pd.ProductId)
                .Select(g => new { ProductId = g.Key, MinPrice = g.Min(x => x.Price), MaxPrice = g.Max(x => x.Price) })
                .ToListAsync();

            var priceDict = minMaxPrices.ToDictionary(x => x.ProductId, x => (Min: (float?)x.MinPrice, Max: (float?)x.MaxPrice));

            return new CategoryHoverResponseDTO
            {
                CategoryId = category.Id,
                CategoryName = category.Name,
                Products = products.Select(p => new ProductPreviewDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Thumbnail = p.Thumbnail,
                    MinPrice = priceDict.ContainsKey(p.Id) ? priceDict[p.Id].Min : null,
                    MaxPrice = priceDict.ContainsKey(p.Id) ? priceDict[p.Id].Max : null
                }).ToList()
            };
        }

        private async Task UpdateProductRelationships(Guid productId, CreateOrUpdateProductDTO productDto)
        {


            // Update materials
            var existingMaterials = await _productMaterialRepository.Query()
                .Where(pm => pm.ProductId == productId)
                .ToListAsync();
            await _productMaterialRepository.DeleteRangeAsync(existingMaterials);

            if (productDto.MaterialIds != null && productDto.MaterialIds.Any())
            {
                var materialEntities = productDto.MaterialIds.Select(materialId => new ProductMaterial
                {
                    ProductId = productId,
                    MaterialId = materialId
                }).ToList();
                await _productMaterialRepository.AddRangeAsync(materialEntities);
            }

            // Update categories
            var existingCategories = await _productCategoryRepository.Query()
                .Where(pc => pc.ProductId == productId)
                .ToListAsync();
            await _productCategoryRepository.DeleteRangeAsync(existingCategories);

            if (productDto.CategoryIds != null && productDto.CategoryIds.Any())
            {
                var categoryEntities = productDto.CategoryIds.Select(categoryId => new ProductCategory
                {
                    ProductId = productId,
                    CategoryId = categoryId
                }).ToList();
                await _productCategoryRepository.AddRangeAsync(categoryEntities);
            }

            // Update seasons
            var existingSeasons = await _productSeasonRepository.Query()
                .Where(ps => ps.ProductId == productId)
                .ToListAsync();
            await _productSeasonRepository.DeleteRangeAsync(existingSeasons);

            if (productDto.SeasonIds != null && productDto.SeasonIds.Any())
            {
                var seasonEntities = productDto.SeasonIds.Select(seasonId => new ProductSeason
                {
                    ProductId = productId,
                    SeasonId = seasonId
                }).ToList();
                await _productSeasonRepository.AddRangeAsync(seasonEntities);
            }
        }

        public async Task<List<ProductResponseDTO>> GetProductsByIdsAsync(List<Guid> ids)
        {
            if (ids == null || ids.Count == 0) return new List<ProductResponseDTO>();

            var products = await _repository.Query()
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();

            var productIds = products.Select(p => p.Id).ToList();

            var variantsDict = await _productDetailRepository.Query()
                .Where(pd => productIds.Contains(pd.ProductId))
                .GroupBy(pd => pd.ProductId)
                .ToDictionaryAsync(g => g.Key, g => g.ToList());

            var imageDict = await _imageRepository.Query()
                .Where(i => productIds.Contains(i.ProductId))
                .GroupBy(i => i.ProductId)
                .ToDictionaryAsync(g => g.Key, g => g.ToList());

            var result = new List<ProductResponseDTO>();
            foreach (var p in products)
            {
                var variants = variantsDict.ContainsKey(p.Id) ? variantsDict[p.Id] : new List<ProductDetail>();
                var images = imageDict.ContainsKey(p.Id) ? imageDict[p.Id] : new List<Image>();
                result.Add(new ProductResponseDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    BrandId = p.BrandId,
                    Thumbnail = p.Thumbnail,
                    CreationTime = p.CreationTime,
                    LastModificationTime = p.LastModificationTime,
                    CreatedBy = p.CreatedBy,
                    UpdatedBy = p.UpdatedBy,
                    ProductVariants = variants.Select(v => new ProductDetailGrid
                    {
                        Id = v.Id,
                        ProductId = v.ProductId,
                        SizeId = v.SizeId,
                        ColourId = v.ColourId,
                        Price = v.Price,
                        SellNumber = v.SellNumber,
                        OutOfStock = v.OutOfStock,
                        StockHeight = v.StockHeight
                    }).ToList(),
                    Media = images.Select(i => new ProductMediaUpload
                    {
                        ImageUrl = i.URL
                    }).ToList()
                });
            }

            var orderMap = ids.Select((id, idx) => new { id, idx }).ToDictionary(x => x.id, x => x.idx);
            result = result.OrderBy(r => orderMap.ContainsKey(r.Id) ? orderMap[r.Id] : int.MaxValue).ToList();
            return result;
        }


    }
}

