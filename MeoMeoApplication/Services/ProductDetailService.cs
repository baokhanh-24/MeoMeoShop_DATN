using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.ProductDetail;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Commons.Enums;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.Domain.IRepositories;
using MeoMeo.Shared.Utilities;

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
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductCategoryRepository _productCategoryRepository;
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
            ICategoryRepository categoryRepository, IProductCategoryRepository productCategoryRepository,
            IUnitOfWork unitOfWork, IMapper mapper, IColourRepository colourRepository,
            IIventoryBatchReposiory inventoryRepository)
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
            _categoryRepository = categoryRepository;
            _productCategoryRepository = productCategoryRepository;
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

                if (request.ClosureTypeFilter != null)
                {
                    detailQuery = detailQuery.Where(p => p.ClosureType == request.ClosureTypeFilter);
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
                    .Where(c => c.Status == EInventoryBatchStatus.Aprroved)
                    .GroupBy(p => p.ProductDetailId)
                    .Select(g => new
                    {
                        ProductDetailId = g.Key,
                        InventoryQuantity = g.Any() ? g.Sum(c => c.Quantity) : 0
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
                        detail.Price,
                        detail.Sku,
                        detail.Description,
                        detail.Gender,
                        detail.StockHeight,
                        detail.ViewNumber,
                        detail.SellNumber,
                        detail.ClosureType,
                        detail.OutOfStock,
                        detail.AllowReturn,
                        detail.Status,
                        detail.CreationTime,
                        Discount = (float?)promo.MaxDiscount,
                        InventoryQuantity = (int?)ib.InventoryQuantity
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

                var categoriesDict = await (from pdc in _productCategoryRepository.Query()
                        join c in _categoryRepository.Query() on pdc.CategoryId equals c.Id
                        where productDetailIds.Contains(pdc.ProductId)
                        group c.Name by pdc.ProductId)
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
                        ClosureType = main.ClosureType,
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
                        Categories =
                            categoriesDict.TryGetValue(main.Id, out var categories) ? categories : string.Empty,
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

        public async Task<CreateOrUpdateProductDetailDTO> GetProductDetailByIdAsync(Guid id)
        {
            try
            {
                var productDetail = await (from pd in _productDetailRepository.Query()
                    join p in _productRepository.Query() on pd.ProductId equals p.Id
                    where pd.Id == id
                    select new
                    {
                        ProductDetail = pd,
                        Product = p
                    }).FirstOrDefaultAsync();

                if (productDetail == null)
                {
                    return new CreateOrUpdateProductDetailDTO();
                }

                var sizeIds = _productDetaillSizeRepository.Query()
                    .Where(x => x.ProductDetailId == id)
                    .Select(x => x.SizeId)
                    .ToList();

                var colourIds = _productDetaillColourRepository.Query()
                    .Where(x => x.ProductDetailId == id)
                    .Select(x => x.ColourId)
                    .ToList();

                var seasonIds = _productSeasonRepository.Query()
                    .Where(x => x.ProductId == productDetail.Product.Id)
                    .Select(x => x.SeasonId)
                    .ToList();

                var materialIds = _productDetailMaterialRepository.Query()
                    .Where(x => x.ProductDetailId == id)
                    .Select(x => x.MaterialId)
                    .ToList();

                var categoryIds = _productCategoryRepository.Query()
                    .Where(x => x.ProductId == productDetail.Product.Id)
                    .Select(x => x.CategoryId)
                    .ToList();

                var images = _imageRepository.Query()
                    .Where(x => x.ProductDetailId == id)
                    .Select(i => new ProductMediaUpload
                    {
                        Id = i.Id,
                        ImageUrl = i.URL,
                        FileName = i.Name,
                        ContentType = "image/jpeg",
                        Base64Data = string.Empty,
                        UploadFile = null
                    })
                    .ToList();

                return new CreateOrUpdateProductDetailDTO
                {
                    Id = productDetail.ProductDetail.Id,
                    ProductId = productDetail.ProductDetail.ProductId,
                    ProductName = productDetail.Product.Name,
                    Price = productDetail.ProductDetail.Price,
                    Description = productDetail.ProductDetail.Description,
                    Gender = productDetail.ProductDetail.Gender,
                    StockHeight = productDetail.ProductDetail.StockHeight,
                    ClosureType = productDetail.ProductDetail.ClosureType,
                    OutOfStock = productDetail.ProductDetail.OutOfStock,
                    AllowReturn = productDetail.ProductDetail.AllowReturn,
                    Status = productDetail.ProductDetail.Status,
                    BrandId = productDetail.Product.BrandId,
                    SizeIds = sizeIds,
                    ColourIds = colourIds,
                    SeasonIds = seasonIds,
                    MaterialIds = materialIds,
                    CategoryIds = categoryIds,
                    MediaUploads = images
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProductDetailByIdAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<BaseResponse> CreateProductDetailAsync(
            CreateOrUpdateProductDetailDTO productDetail, List<FileUploadResult> lstFileMedia)
        {
            try
            {
                var checkExistedName =
                    await _productRepository.AnyAsync(c => c.Name.ToLower() == productDetail.ProductName.ToLower());
                if (checkExistedName)
                {
                    return new BaseResponse { Message = $"Tên sản phẩm đã tồn tại", ResponseStatus = BaseStatus.Error };
                }

                await _unitOfWork.BeginTransactionAsync();
                var thumbNail = lstFileMedia.First().FileName;
                var productToAdd = new Product()
                {
                    Id = productDetail.ProductId.Value,
                    BrandId = productDetail.BrandId,
                    Name = productDetail.ProductName,
                    Thumbnail = thumbNail,
                    CreationTime = DateTime.Now,
                };
                await _productRepository.AddAsync(productToAdd);
                var mappedProductDetail = _mapper.Map<ProductDetail>(productDetail);
                mappedProductDetail.Id = Guid.NewGuid();
                mappedProductDetail.ProductId = productToAdd.Id;
                mappedProductDetail.ViewNumber = 0;
                mappedProductDetail.SellNumber = 0;
                var latestSKU = await _productDetailRepository.Query().OrderByDescending(p => p.Sku).Select(p => p.Sku)
                    .FirstOrDefaultAsync();
                mappedProductDetail.Sku = SkuGenerator.GenerateNextSku(latestSKU);
                var response = await _productDetailRepository.CreateProductDetailAsync(mappedProductDetail);

                if (productDetail.SizeIds.Any())
                {
                    var sizeEntities = productDetail.SizeIds.Select(sizeId => new ProductDetailSize
                    {
                        ProductDetailId = mappedProductDetail.Id,
                        SizeId = sizeId
                    });
                    await _productDetaillSizeRepository.AddRangeAsync(sizeEntities);
                }

                if (productDetail.ColourIds.Any())
                {
                    var colourEntities = productDetail.ColourIds.Select(colourId => new ProductDetailColour
                    {
                        ProductDetailId = mappedProductDetail.Id,
                        ColourId = colourId
                    });
                    await _productDetaillColourRepository.AddRangeAsync(colourEntities);
                }

                if (productDetail.SeasonIds.Any())
                {
                    var seasonEntities = productDetail.SeasonIds.Select(seasonId => new ProductSeason
                    {
                        ProductId = productToAdd.Id,
                        SeasonId = seasonId
                    });
                    await _productSeasonRepository.AddRangeAsync(seasonEntities);
                }

                if (productDetail.MaterialIds.Any())
                {
                    var materialEntities = productDetail.MaterialIds.Select(materialId => new ProductDetailMaterial
                    {
                        ProductDetailId = mappedProductDetail.Id,
                        MaterialId = materialId
                    });
                    await _productDetailMaterialRepository.AddRangeAsync(materialEntities);
                }

                if (productDetail.CategoryIds.Any())
                {
                    var categoryEntities = productDetail.CategoryIds.Select(categoryId => new ProductCategory
                    {
                        ProductId = mappedProductDetail.Id,
                        CategoryId = categoryId
                    });
                    await _productCategoryRepository.AddRangeAsync(categoryEntities);
                }

                if (lstFileMedia.Any())
                {
                    List<Image> images = new List<Image>();
                    foreach (var file in lstFileMedia)
                    {
                        var newFile = new Image()
                        {
                            Id = Guid.NewGuid(),
                            Name = file.FileName,
                            Type = Convert.ToInt32(file.FileType),
                            URL = file.RelativePath
                        };
                        images.Add(newFile);
                    }

                    await _imageRepository.AddRangeAsync(images);
                }

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
        }

        public async Task<BaseResponse> UpdateProductDetailAsync(CreateOrUpdateProductDetailDTO productDetail,
            List<FileUploadResult> lstFileMedia)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var productDetailToUpdate =
                    await _productDetailRepository.GetProductDetailByIdAsync(productDetail.Id.Value);
                if (productDetailToUpdate == null)
                {
                    return new BaseResponse
                        { Message = "Không tìm thấy chi tiết sản phẩm", ResponseStatus = BaseStatus.Error };
                }

                _mapper.Map(productDetail, productDetailToUpdate);
                var product = await _productRepository.GetByIdAsync(productDetailToUpdate.ProductId);
                if (product != null && !string.IsNullOrWhiteSpace(productDetail.ProductName))
                {
                    product.Name = productDetail.ProductName;
                }

                // Xử lý ảnh: xóa ảnh cũ không còn, thêm ảnh mới
                var oldImages = (await _imageRepository.GetAllImage()).Where(x => x.ProductDetailId == productDetail.Id)
                    .ToList();
                var newImageIds =
                    productDetail.MediaUploads?.Where(i => i.Id != null).Select(i => i.Id.Value).ToList() ??
                    new List<Guid>();
                var imagesToDelete = oldImages.Where(img => !newImageIds.Contains(img.Id)).ToList();
                // Xóa file vật lý và record DB
                foreach (var img in imagesToDelete)
                {
                    await _imageRepository.DeleteImage(img.Id);
                }

                // Thêm ảnh mới
                if (lstFileMedia != null && lstFileMedia.Count > 0)
                {
                    foreach (var file in lstFileMedia)
                    {
                        var image = new Image
                        {
                            Id = Guid.NewGuid(),
                            ProductDetailId = productDetail.Id.Value,
                            Name = file.FileName,
                            Type = file.FileType ?? 1,
                            URL = file.FullPath
                        };
                        await _imageRepository.CreateImage(image);
                    }
                }

                // Thêm lại dữ liệu mới
                if (productDetail.SizeIds.Any())
                {
                    // Xóa sizes cũ
                    var oldSizes = await _productDetaillSizeRepository.Query()
                        .Where(x => x.ProductDetailId == productDetail.Id.Value)
                        .ToListAsync();
                    foreach (var size in oldSizes)
                    {
                        await _productDetaillSizeRepository.DeleteAsync(size);
                    }

                    // Thêm sizes mới
                    var sizeEntities = productDetail.SizeIds.Select(sizeId => new ProductDetailSize
                    {
                        ProductDetailId = productDetail.Id.Value,
                        SizeId = sizeId
                    });
                    await _productDetaillSizeRepository.AddRangeAsync(sizeEntities);
                }

                if (productDetail.ColourIds.Any())
                {
                    // Xóa colours cũ
                    var oldColours = await _productDetaillColourRepository.Query()
                        .Where(x => x.ProductDetailId == productDetail.Id.Value)
                        .ToListAsync();
                    foreach (var colour in oldColours)
                    {
                        await _productDetaillColourRepository.DeleteAsync(colour);
                    }

                    // Thêm colours mới
                    var colourEntities = productDetail.ColourIds.Select(colourId => new ProductDetailColour
                    {
                        ProductDetailId = productDetail.Id.Value,
                        ColourId = colourId
                    });
                    await _productDetaillColourRepository.AddRangeAsync(colourEntities);
                }

                if (productDetail.SeasonIds.Any())
                {
                    var oldSeasons = await _productSeasonRepository.Query()
                        .Where(x => x.ProductId == productDetailToUpdate.ProductId)
                        .ToListAsync();
                    foreach (var season in oldSeasons)
                    {
                        await _productSeasonRepository.DeleteAsync(season);
                    }

                    // Thêm seasons mới
                    var seasonEntities = productDetail.SeasonIds.Select(seasonId => new ProductSeason
                    {
                        ProductId = productDetailToUpdate.ProductId,
                        SeasonId = seasonId
                    });
                    await _productSeasonRepository.AddRangeAsync(seasonEntities);
                }

                if (productDetail.MaterialIds.Any())
                {
                    // Xóa materials cũ
                    var oldMaterials = await _productDetailMaterialRepository.Query()
                        .Where(x => x.ProductDetailId == productDetail.Id.Value)
                        .ToListAsync();
                    foreach (var material in oldMaterials)
                    {
                        await _productDetailMaterialRepository.DeleteAsync(material);
                    }

                    // Thêm materials mới
                    var materialEntities = productDetail.MaterialIds.Select(materialId => new ProductDetailMaterial
                    {
                        ProductDetailId = productDetail.Id.Value,
                        MaterialId = materialId
                    });
                    await _productDetailMaterialRepository.AddRangeAsync(materialEntities);
                }

                if (productDetail.CategoryIds.Any())
                {
                    await _productCategoryRepository.DeleteByProductIdAsync(productDetail.Id.Value);

                    var categoryEntities = productDetail.CategoryIds.Select(categoryId => new ProductCategory()
                    {
                        ProductId = productDetail.Id.Value,
                        CategoryId = categoryId
                    });
                    await _productCategoryRepository.AddRangeAsync(categoryEntities);
                }

                await _productDetailRepository.UpdateProductDetailAsync(productDetailToUpdate);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return new BaseResponse { Message = "Cập nhật thành công", ResponseStatus = BaseStatus.Success };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new BaseResponse { Message = ex.Message, ResponseStatus = BaseStatus.Error };
            }
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

        public async Task<List<Image>> GetOldImagesAsync(Guid productDetailId)
        {
            var allImages = await _imageRepository.GetAllImage();
            return allImages.Where(x => x.ProductDetailId == productDetailId).ToList();
        }
    }
}