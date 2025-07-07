using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.ProductDetail;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace MeoMeo.Application.Services
{
    public class ProductDetailService : IProductDetailServices
    {
        private readonly IProductsDetailRepository _productDetailRepository;
        private readonly IProductRepository _productRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductDetailService(IProductsDetailRepository productDetailRepository, IProductRepository productRepository, IBrandRepository brandRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _productDetailRepository = productDetailRepository;
            _productRepository = productRepository;
            _brandRepository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagingExtensions.PagedResult<ProductDetailDTO>> GetAllProductDetailAsync(GetListProductDetailRequestDTO request)
        {
            try
            {
                var detailQuery = _productDetailRepository.Query();
                var productQuery = _productRepository.Query();
                //var query = _productDetailRepository.Query();
                if (!string.IsNullOrEmpty(request.ProductNameFilter))
                {
                    productQuery = productQuery.Where(p => EF.Functions.Like(p.Name, $"%{request.ProductNameFilter}%"));
                }
                if (request.PriceFilter != null)
                {
                    detailQuery = detailQuery.Where(p => p.Price == request.PriceFilter);
                }
                if (!string.IsNullOrEmpty(request.SKUFilter))
                {
                    detailQuery = detailQuery.Where(p => EF.Functions.Like(p.Sku, $"%{request.SKUFilter}%"));
                }
                if (request.GenderFilter != null)
                {
                    detailQuery = detailQuery.Where(p => p.Gender == request.GenderFilter);
                }
                if (request.StockHeightFilter != null)
                {
                    detailQuery = detailQuery.Where(p => p.StockHeight == request.StockHeightFilter);
                }
                if (request.ShoeLengthFilter != null)
                {
                    detailQuery = detailQuery.Where(p => p.ShoeLength == request.ShoeLengthFilter);
                }
                if (request.OutOfStockFilter != null)
                {
                    detailQuery = detailQuery.Where(p => p.OutOfStock == request.OutOfStockFilter);
                }
                detailQuery = detailQuery.OrderByDescending(c => c.CreationTime);
                var joinQuery = from detail in detailQuery
                                join product in productQuery on detail.ProductId equals product.Id
                                select new ProductDetailDTO
                                {
                                    Id = detail.Id,
                                    ProductId = detail.ProductId,
                                    ProductName = product.Name,
                                    Barcode = detail.Barcode,
                                    Price = detail.Price,
                                    Sku = detail.Sku,
                                    Description = detail.Description,
                                    Gender = detail.Gender,
                                    StockHeight = detail.StockHeight,
                                    ShoeLength = detail.ShoeLength,
                                    OutOfStock = detail.OutOfStock,
                                    AllowReturn = detail.AllowReturn,
                                    Status = detail.Status,
                                };
                var totalRecords = await joinQuery.CountAsync();
                var items = await joinQuery
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();
                return new PagingExtensions.PagedResult<ProductDetailDTO>
                {
                    TotalRecords = totalRecords,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    Items = items
                };
                /*
                var filterProductDetails = await _productDetailRepository.GetPagedAsync(detailQuery, request.PageIndex, request.PageSize);
                var dtoItems = _mapper.Map<List<ProductDetailDTO>>(filterProductDetails.Items);
                return new PagingExtensions.PagedResult<ProductDetailDTO>
                {
                    TotalRecords = totalRecords,
                    PageIndex = filterProductDetails.PageIndex,
                    PageSize = filterProductDetails.PageSize,
                    Items = dtoItems
                };*/
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public async Task<ProductDetailDTO> GetProductDetailByIdAsync(Guid id)
        {
            var productDetail = await _productDetailRepository.GetProductDetailByIdAsync(id);
            return _mapper.Map<ProductDetailDTO>(productDetail);
        }

        public async Task<CreateOrUpdateProductDetailResponseDTO> CreateProductDetailAsync(CreateOrUpdateProductDetailDTO productDetail)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var productId = Guid.NewGuid();
                var brandId = "8f2a8e93-58fe-41ef-8adc-386734754261";
                var ProductToAdd = new Product()
                {
                    Id = productId,
                    BrandId = Guid.Parse(brandId),
                    Name = productDetail.ProductName,
                    //Name = "Nike",
                    Thumbnail = "/////",
                    CreationTime = DateTime.Now,
                };
                await _productRepository.AddAsync(ProductToAdd);
                var mappedProductDetail = _mapper.Map<ProductDetail>(productDetail);
                mappedProductDetail.Id = Guid.NewGuid();
                mappedProductDetail.ProductId = productId;
                var response = await _productDetailRepository.CreateProductDetailAsync(mappedProductDetail);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return _mapper.Map<CreateOrUpdateProductDetailResponseDTO>(response);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                Console.WriteLine($"Transaction failed: {ex.Message}");
                return new CreateOrUpdateProductDetailResponseDTO()
                {
                    Message = ex.Message,
                    ResponseStatus = BaseStatus.Error
                };
            }
            //var entity = _mapper.Map<ProductDetail>(dto);
            //entity.Id = Guid.NewGuid();
            //return await _repository.AddProductAsync(entity);
        }

        public async Task<CreateOrUpdateProductDetailResponseDTO> UpdateProductDetailAsync(CreateOrUpdateProductDetailDTO productDetail)
        {
            var isExistedSKU = await _productDetailRepository.AnyAsync(p => p.Id != productDetail.Id && p.Sku == productDetail.Sku);
            if (isExistedSKU)
            {
                return new CreateOrUpdateProductDetailResponseDTO()
                {
                    Message = $"Mã SKU đã tồn tại",
                    ResponseStatus = BaseStatus.Error
                };
            }
            var productDetailToUpdate = await _productDetailRepository.GetProductDetailByIdAsync(productDetail.Id);
            _mapper.Map(productDetail, productDetailToUpdate);
            var product = await _productRepository.GetByIdAsync(productDetailToUpdate.ProductId);
            if (product != null && !string.IsNullOrWhiteSpace(productDetail.ProductName))
            {
                product.Name = productDetail.ProductName;
            }

            var result = await _productDetailRepository.UpdateProductDetailAsync(productDetailToUpdate);
            return _mapper.Map<CreateOrUpdateProductDetailResponseDTO>(result);
        }

        public async Task<bool> DeleteProductDetailAsync(Guid id)
        {
            var productDetailToDelete = await _productDetailRepository.GetProductDetailByIdAsync(id);
            if (productDetailToDelete == null)
            {
                return false;
            }
            await _productRepository.DeleteAsync(productDetailToDelete.ProductId);
            await _productDetailRepository.DeleteProductDetailAsync(productDetailToDelete.Id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

    }
}




