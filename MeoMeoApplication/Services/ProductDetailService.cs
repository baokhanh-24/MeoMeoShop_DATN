using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Material;
using MeoMeo.Contract.DTOs.ProductDetail;
using MeoMeo.Contract.DTOs.Size;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Commons.Enums;
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
        private readonly ISizeRepository _sizeRepository;
        private readonly IProductDetaillSizeRepository _productDetaillSizeRepository;
        private readonly IProductSeasonRepository _productSeasonRepository;
        private readonly ISeasonRepository _seasonRepository;
        private readonly IPromotionDetailRepository _promotionDetailRepository;
        private readonly IProductDetaillColourRepository _productDetaillColourRepository;
        private readonly IColourRepository _colourRepository;
        private readonly IMaterialRepository _materialRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IProductDetailMaterialRepository _productDetailMaterialRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IIventoryBatchReposiory _inventoryRepository;

        public ProductDetailService(IProductsDetailRepository productDetailRepository,
            IProductRepository productRepository, IBrandRepository brandRepository, ISizeRepository sizeRepository,
            IProductDetaillSizeRepository productDetaillSizeRepository,
            IProductSeasonRepository productSeasonRepository, ISeasonRepository seasonRepository,
            IPromotionDetailRepository promotionDetailRepository,
            IProductDetaillColourRepository productDetaillColourRepository, IMaterialRepository materialRepository,
            IImageRepository imageRepository, IProductDetailMaterialRepository productDetailMaterialRepository,
            IUnitOfWork unitOfWork, IMapper mapper, IColourRepository colourRepository, IIventoryBatchReposiory inventoryRepository)
        {
            _productDetailRepository = productDetailRepository;
            _productRepository = productRepository;
            _brandRepository = brandRepository;
            _sizeRepository = sizeRepository;
            _productDetaillSizeRepository = productDetaillSizeRepository;
            _productSeasonRepository = productSeasonRepository;
            _seasonRepository = seasonRepository;
            _promotionDetailRepository = promotionDetailRepository;
            _productDetaillColourRepository = productDetaillColourRepository;
            _materialRepository = materialRepository;
            _imageRepository = imageRepository;
            _productDetailMaterialRepository = productDetailMaterialRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _colourRepository = colourRepository;
            _inventoryRepository = inventoryRepository;
        }


        public async Task<PagingExtensions.PagedResult<ProductDetailDTO>> GetAllProductDetailAsync(
            GetListProductDetailRequestDTO request)
        {
            try
            {
                var detailQuery = _productDetailRepository.Query();
                var productQuery = _productRepository.Query();
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

                var promotionSubQuery = _promotionDetailRepository.Query()
                    .GroupBy(p => p.ProductDetailId)
                    .Select(g => new
                    {
                        ProductDetailId = g.Key,
                        MaxDiscount = g.Any() ? g.Max(x => x.Discount) : 0
                    });
                var inventorySubQuery = _inventoryRepository.Query()
                    .Where(c=>c.Status== EInventoryBatchStatus.Aprroved)
                    .GroupBy(p => p.ProductDetailId)
                    .Select(g => new
                    {
                        ProductDetailId = g.Key,
                        InventoryQuantity = g.Any() ? g.Sum(c=>c.Quantity) : 0
                    });
                var mainQuery = from detail in detailQuery
                    join product in productQuery on detail.ProductId equals product.Id
                    join promo in promotionSubQuery on detail.Id equals promo.ProductDetailId into promoGroup
                    from promo in promoGroup.DefaultIfEmpty()
                    join ib in inventorySubQuery on detail.Id equals ib.ProductDetailId into ibGroup
                    from ib in ibGroup.DefaultIfEmpty()
                    select new
                    {
                        detail.Id,
                        detail.ProductId,
                        ProductName = product.Name + "-" + detail.Sku,
                        detail.Barcode,
                        detail.Price,
                        detail.Sku,
                        detail.Description,
                        detail.Gender,
                        detail.StockHeight,
                        detail.ViewNumber,
                        detail.SellNumber,
                        detail.ShoeLength,
                        detail.OutOfStock,
                        detail.AllowReturn,
                        detail.Status,
                        detail.CreationTime,
                        Discount = (float?)promo.MaxDiscount,
                        InventoryQuantity= (int?) ib.InventoryQuantity
                        
                    };
                switch (request.SortField)
                {
                    case EProductDetailSortField.Name:
                        mainQuery = request.SortDirection == ESortDirection.Desc
                            ? mainQuery.OrderByDescending(x => x.ProductName)
                            : mainQuery.OrderBy(x => x.ProductName);
                        break;

                    case EProductDetailSortField.Price:
                        mainQuery = request.SortDirection == ESortDirection.Desc
                            ? mainQuery.OrderByDescending(x => x.Price)
                            : mainQuery.OrderBy(x => x.Price);
                        break;

                    case EProductDetailSortField.Discount:
                        mainQuery = request.SortDirection == ESortDirection.Desc
                            ? mainQuery.OrderByDescending(x => x.Discount)
                            : mainQuery.OrderBy(x => x.Discount);
                        break;

                    case EProductDetailSortField.CreationTime:
                        mainQuery = request.SortDirection == ESortDirection.Desc
                            ? mainQuery.OrderByDescending(x => x.CreationTime)
                            : mainQuery.OrderBy(x => x.CreationTime);
                        break;

                    case EProductDetailSortField.StockQuantity:
                        mainQuery = request.SortDirection == ESortDirection.Desc
                            ? mainQuery.OrderByDescending(x => x.InventoryQuantity)
                            : mainQuery.OrderBy(x => x.InventoryQuantity);
                        break;

                    case EProductDetailSortField.ViewNumber:
                        mainQuery = request.SortDirection == ESortDirection.Desc
                            ? mainQuery.OrderByDescending(x => x.ViewNumber)
                            : mainQuery.OrderBy(x => x.ViewNumber);
                        break;

                    case EProductDetailSortField.SellNumber:
                        mainQuery = request.SortDirection == ESortDirection.Desc
                            ? mainQuery.OrderByDescending(x => x.SellNumber)
                            : mainQuery.OrderBy(x => x.SellNumber);
                        break;

                    default:
                        mainQuery = mainQuery.OrderByDescending(x => x.Id); 
                        break;
                }
                var totalRecords = await mainQuery.CountAsync();
                var mainResults = await mainQuery
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                var productDetailIds = mainResults.Select(x => x.Id);
                var imagesDict = await _imageRepository.Query()
                    .Where(i => productDetailIds.Contains(i.ProductDetailId))
                    .GroupBy(i => i.ProductDetailId)
                    .ToDictionaryAsync(g => g.Key, g => g.Select(x => x.URL).ToList());

                var sizesDict = await (from pds in _productDetaillSizeRepository.Query()
                        join s in _sizeRepository.Query() on pds.SizeId equals s.Id
                        where productDetailIds.Contains(pds.ProductDetailId)
                        group s.Value by pds.ProductDetailId)
                    .ToDictionaryAsync(g => g.Key, g => string.Join(", ", g));

                var coloursDict = await (from pdc in _productDetaillColourRepository.Query()
                        join c in _colourRepository.Query() on pdc.ColourId equals c.Id
                        where productDetailIds.Contains(pdc.ProductDetailId)
                        group c.Name by pdc.ProductDetailId)
                    .ToDictionaryAsync(g => g.Key, g => string.Join(", ", g));

                var materialsDict = await (from pdm in _productDetailMaterialRepository.Query()
                        join m in _materialRepository.Query() on pdm.MaterialId equals m.Id
                        where productDetailIds.Contains(pdm.ProductDetailId)
                        group m.Name by pdm.ProductDetailId)
                    .ToDictionaryAsync(g => g.Key, g => string.Join(", ", g));

                var items = mainResults.Select(main =>
                {
                    var hasImages = imagesDict.TryGetValue(main.Id, out var imageList);
                    return new ProductDetailDTO
                    {
                        Id = main.Id,
                        ProductId = main.ProductId,
                        ProductName = main.ProductName,
                        Price = main.Price,
                        Sku = main.Sku,
                        Description = main.Description,
                        Gender = main.Gender,
                        StockHeight = main.StockHeight,
                        ShoeLength = main.ShoeLength,
                        OutOfStock = main.OutOfStock,
                        AllowReturn = main.AllowReturn,
                        Status = main.Status,
                        ViewNumber = main.ViewNumber,
                        SellNumber = main.SellNumber,
                        InventoryQuantity = main.InventoryQuantity ?? 0,
                        Discount = main.Discount ?? 0,
                        Images = hasImages ? string.Join(", ", imageList) : string.Empty,
                        Thumbnail = hasImages ? imageList.FirstOrDefault() : string.Empty,
                        Sizes = sizesDict.TryGetValue(main.Id, out var sizes) ? sizes : string.Empty,
                        Colours = coloursDict.TryGetValue(main.Id, out var colours) ? colours : string.Empty,
                        Materials = materialsDict.TryGetValue(main.Id, out var materials) ? materials : string.Empty,
                        PromotionDetailId = null
                    };
                }).ToList();
                return new PagingExtensions.PagedResult<ProductDetailDTO>
                {
                    TotalRecords = totalRecords,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    Items = items
                };
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

        public async Task<CreateOrUpdateProductDetailResponseDTO> CreateProductDetailAsync(
            CreateOrUpdateProductDetailDTO productDetail)
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

        public async Task<CreateOrUpdateProductDetailResponseDTO> UpdateProductDetailAsync(
            CreateOrUpdateProductDetailDTO productDetail)
        {
            var isExistedSKU =
                await _productDetailRepository.AnyAsync(p => p.Id != productDetail.Id && p.Sku == productDetail.Sku);
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
    public class ProductDetailRelatedData
    {
        public Guid ProductDetailId { get; set; }
        public string Value { get; set; }
    }
}