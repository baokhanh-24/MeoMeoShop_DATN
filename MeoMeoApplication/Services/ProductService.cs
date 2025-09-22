using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Product;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.Shared.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Text;
using MeoMeo.Contract.DTOs.ProductDetail;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Commons.Enums;
using Microsoft.Extensions.Logging;

namespace MeoMeo.Application.Services
{
    public class ProductService : IProductServices
    {
        private readonly IProductRepository _repository;
        private readonly IProductsDetailRepository _productDetailRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IMaterialRepository _materialRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISeasonRepository _seasonRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IProductMaterialRepository _productMaterialRepository;
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IProductSeasonRepository _productSeasonRepository;
        private readonly ICartDetaillRepository _cartDetailRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IPromotionDetailRepository _promotionDetailRepository;
        private readonly IProductReviewRepository _reviewRepository;
        private readonly IIventoryBatchReposiory _inventoryBatchRepository;
        private readonly IInventoryTranSactionRepository _inventoryTransactionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IProductRepository repository,
            IProductsDetailRepository productDetailRepository,
            IBrandRepository brandRepository,
            IMaterialRepository materialRepository,
            ICategoryRepository categoryRepository,
            ISeasonRepository seasonRepository,
            IImageRepository imageRepository,
            IProductMaterialRepository productMaterialRepository,
            IProductCategoryRepository productCategoryRepository,
            IProductSeasonRepository productSeasonRepository,
            ICartDetaillRepository cartDetailRepository,
            IOrderDetailRepository orderDetailRepository,
            IPromotionDetailRepository promotionDetailRepository,
            IProductReviewRepository reviewRepository,
            IIventoryBatchReposiory inventoryBatchRepository,
            IInventoryTranSactionRepository inventoryTransactionRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ProductService> logger)
        {
            _repository = repository;
            _productDetailRepository = productDetailRepository;
            _brandRepository = brandRepository;
            _materialRepository = materialRepository;
            _categoryRepository = categoryRepository;
            _seasonRepository = seasonRepository;
            _imageRepository = imageRepository;
            _productMaterialRepository = productMaterialRepository;
            _productCategoryRepository = productCategoryRepository;
            _productSeasonRepository = productSeasonRepository;
            _cartDetailRepository = cartDetailRepository;
            _orderDetailRepository = orderDetailRepository;
            _promotionDetailRepository = promotionDetailRepository;
            _reviewRepository = reviewRepository;
            _inventoryBatchRepository = inventoryBatchRepository;
            _inventoryTransactionRepository = inventoryTransactionRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BaseResponse> CreateProductAsync(CreateOrUpdateProductDTO productDto, List<FileUploadResult> uploadedFiles)
        {
            try
            {
                var checkExistedName =
                    await _repository.AnyAsync(c => c.Name.ToLower() == productDto.Name.ToLower());
                if (checkExistedName)
                {
                    return new BaseResponse { Message = $"Tên sản phẩm đã tồn tại", ResponseStatus = BaseStatus.Error };
                }
                var thumbNail = uploadedFiles.FirstOrDefault(c => c.FileType == 0).RelativePath;
                await _unitOfWork.BeginTransactionAsync();
                var mappedProduct = _mapper.Map<Product>(productDto);
                mappedProduct.Thumbnail = thumbNail;
                mappedProduct.CreationTime = DateTime.Now;

                var createdProduct = await _repository.AddProductAsync(mappedProduct);
                if (productDto.ProductVariants.Any())
                {
                    var latestSKU = await _productDetailRepository.Query().OrderByDescending(p => p.Sku).Select(p => p.Sku)
                        .FirstOrDefaultAsync();
                    var newSkus = SkuGenerator.GenerateRange(latestSKU, productDto.ProductVariants.Count()).ToList();
                    var variantEntities = productDto.ProductVariants.Select((variant, index) =>
                    {
                        var mappedVariant = _mapper.Map<ProductDetail>(variant);
                        mappedVariant.Id = Guid.NewGuid();
                        mappedVariant.ProductId = createdProduct.Id;
                        mappedVariant.CreationTime = DateTime.Now;
                        mappedVariant.ViewNumber = 0;
                        mappedVariant.SellNumber = 0;
                        mappedVariant.Sku = newSkus[index];
                        mappedVariant.Status = variant.Status;
                        return mappedVariant;
                    });
                    await _productDetailRepository.AddRangeAsync(variantEntities);
                }

                // Create product-material relationships
                if (productDto.MaterialIds != null && productDto.MaterialIds.Any())
                {
                    var materialEntities = productDto.MaterialIds.Select(materialId => new ProductMaterial
                    {
                        ProductId = mappedProduct.Id,
                        MaterialId = materialId
                    }).ToList();
                    await _productMaterialRepository.AddRangeAsync(materialEntities);
                }

                // Create product-category relationships
                if (productDto.CategoryIds != null && productDto.CategoryIds.Any())
                {
                    var categoryEntities = productDto.CategoryIds.Select(categoryId => new ProductCategory
                    {
                        ProductId = mappedProduct.Id,
                        CategoryId = categoryId
                    }).ToList(); await _productCategoryRepository.AddRangeAsync(categoryEntities);
                }

                // Create product-season relationships
                if (productDto.SeasonIds != null && productDto.SeasonIds.Any())
                {
                    var seasonEntities = productDto.SeasonIds.Select(seasonId => new ProductSeason
                    {
                        ProductId = mappedProduct.Id,
                        SeasonId = seasonId
                    }).ToList();
                    await _productSeasonRepository.AddRangeAsync(seasonEntities);
                }

                // Handle uploaded files - follow ProductDetailService pattern
                if (uploadedFiles != null && uploadedFiles.Any())
                {
                    List<Image> images = new List<Image>();
                    foreach (var uploadedFile in uploadedFiles)
                    {
                        var mappedImage = new Image();
                        mappedImage.Id = Guid.NewGuid();
                        mappedImage.ProductId = createdProduct.Id;
                        mappedImage.URL = uploadedFile.RelativePath;
                        mappedImage.Name = uploadedFile.FileName;
                        mappedImage.Type = uploadedFile.FileType ?? 0;
                        images.Add(mappedImage);
                    }
                    await _imageRepository.AddRangeAsync(images);
                }
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new BaseResponse
                {
                    Message = $"Lỗi khi tạo sản phẩm: {ex.Message}",
                    ResponseStatus = BaseStatus.Error
                };
            }
        }

        public async Task<BaseResponse> UpdateAsync(CreateOrUpdateProductDTO productDto, List<FileUploadResult> uploadedFiles)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var existingProduct = await _repository.GetProductByIdAsync(productDto.Id.Value);
                if (existingProduct == null)
                {
                    return new BaseResponse
                    {
                        Message = "Không tìm thấy sản phẩm để cập nhật",
                        ResponseStatus = BaseStatus.Error
                    };
                }
                string thumbNail = "";
                if (uploadedFiles != null && uploadedFiles.Any())
                {
                    thumbNail = uploadedFiles.FirstOrDefault(c => c.FileType == 0).RelativePath;
                }
                existingProduct.Thumbnail = string.IsNullOrEmpty(thumbNail) ? existingProduct.Thumbnail : thumbNail;
                _mapper.Map(productDto, existingProduct);
                await _repository.UpdateAsync(existingProduct);
                // Xử lý cập nhật biến thể sản phẩm với logic so sánh đầy đủ
                if (productDto.ProductVariants.Any())
                {
                    // Lấy danh sách biến thể hiện tại từ database
                    var existingVariants = await _productDetailRepository.Query()
                        .Where(pd => pd.ProductId == existingProduct.Id)
                        .ToListAsync();

                    // Lấy danh sách biến thể mới từ DTO
                    var newVariants = productDto.ProductVariants ?? new List<ProductDetailGrid>();

                    // Tìm các biến thể cần xóa (có trong DB nhưng không có trong danh sách mới)
                    var variantsToDelete = existingVariants
                        .Where(existing => !newVariants.Any(newVariant =>
                            newVariant.Id.HasValue && newVariant.Id.Value == existing.Id))
                        .ToList();

                    // Tìm các biến thể cần cập nhật (có trong cả DB và danh sách mới)
                    var variantsToUpdate = existingVariants
                        .Where(existing => newVariants.Any(newVariant =>
                            newVariant.Id.HasValue && newVariant.Id.Value == existing.Id))
                        .ToList();

                    // Tìm các biến thể cần thêm mới (có trong danh sách mới nhưng không có trong DB)
                    var variantsToAdd = newVariants
                        .Where(newVariant => !newVariant.Id.HasValue ||
                            !existingVariants.Any(existing => existing.Id == newVariant.Id.Value))
                        .ToList();

                    // Xóa các biến thể không còn cần thiết
                    if (variantsToDelete.Any())
                    {
                        // Kiểm tra ràng buộc với các bảng liên quan trước khi xóa
                        foreach (var variant in variantsToDelete)
                        {
                            // Kiểm tra xem biến thể có đang được sử dụng trong giỏ hàng không
                            var cartDetails = await _cartDetailRepository.Query()
                                .Where(cd => cd.ProductDetailId == variant.Id)
                                .AnyAsync();
                            if (cartDetails)
                            {
                                return new BaseResponse
                                {
                                    Message = $"Không thể xóa biến thể {variant.Sku} vì đang được sử dụng trong giỏ hàng của khách hàng",
                                    ResponseStatus = BaseStatus.Error
                                };
                            }

                            // Kiểm tra xem biến thể có đang được sử dụng trong đơn hàng không
                            var orderDetails = await _orderDetailRepository.Query()
                                .Where(od => od.ProductDetailId == variant.Id)
                                .AnyAsync();
                            if (orderDetails)
                            {
                                return new BaseResponse
                                {
                                    Message = $"Không thể xóa biến thể {variant.Sku} vì đang được sử dụng trong đơn hàng",
                                    ResponseStatus = BaseStatus.Error
                                };
                            }

                            // Kiểm tra xem biến thể có đang được sử dụng trong khuyến mãi không
                            var promotionDetails = await _promotionDetailRepository.Query()
                                .Where(pd => pd.ProductDetailId == variant.Id)
                                .AnyAsync();
                            if (promotionDetails)
                            {
                                return new BaseResponse
                                {
                                    Message = $"Không thể xóa biến thể {variant.Sku} vì đang được sử dụng trong chương trình khuyến mãi",
                                    ResponseStatus = BaseStatus.Error
                                };
                            }
                        }

                        // Nếu tất cả kiểm tra đều pass, tiến hành xóa các biến thể
                        await _productDetailRepository.DeleteRangeAsync(variantsToDelete);
                    }

                    // Cập nhật các biến thể hiện có
                    if (variantsToUpdate.Any())
                    {
                        foreach (var existingVariant in variantsToUpdate)
                        {
                            // Tìm dữ liệu mới tương ứng với biến thể hiện tại
                            var newVariantData = newVariants.First(nv =>
                                nv.Id.HasValue && nv.Id.Value == existingVariant.Id);
                            // Cập nhật các trường thông tin nhưng giữ nguyên dữ liệu quan trọng
                            existingVariant.SizeId = newVariantData.SizeId;
                            existingVariant.ColourId = newVariantData.ColourId;
                            existingVariant.Price = newVariantData.Price;
                            existingVariant.OutOfStock = newVariantData.OutOfStock;
                            existingVariant.StockHeight = newVariantData.StockHeight;
                            existingVariant.ClosureType = newVariantData.ClosureType;
                            existingVariant.AllowReturn = newVariantData.AllowReturn;
                            existingVariant.Status = newVariantData.Status;

                            // Cập nhật thông tin vận chuyển
                            existingVariant.Weight = newVariantData.Weight;
                            existingVariant.Length = newVariantData.Length;
                            existingVariant.Width = newVariantData.Width;
                            existingVariant.Height = newVariantData.Height;

                            // Cập nhật giới hạn mua hàng
                            existingVariant.MaxBuyPerOrder = newVariantData.MaxBuyPerOrder;

                            existingVariant.LastModificationTime = DateTime.Now;
                            await _productDetailRepository.UpdateProductDetailAsync(existingVariant);
                        }
                    }

                    // Thêm các biến thể mới
                    if (variantsToAdd.Any())
                    {
                        // Lấy SKU cuối cùng để tạo SKU mới

                        var latestSKU = await _productDetailRepository.Query().OrderByDescending(p => p.Sku).Select(p => p.Sku)
                            .FirstOrDefaultAsync();
                        var newSkus = SkuGenerator.GenerateRange(latestSKU, productDto.ProductVariants.Count()).ToList();
                        // Tạo các entity biến thể mới
                        var newVariantEntities = variantsToAdd.Select((variant, index) =>
                        {
                            var mappedVariant = _mapper.Map<ProductDetail>(variant);
                            mappedVariant.Id = Guid.NewGuid();
                            mappedVariant.ProductId = existingProduct.Id;
                            mappedVariant.ViewNumber = 0; // Khởi tạo số lượt xem = 0
                            mappedVariant.SellNumber = 0; // Khởi tạo số lượng đã bán = 0
                            mappedVariant.Sku = newSkus[index]; // Tạo SKU mới
                            mappedVariant.CreationTime = DateTime.Now;
                            mappedVariant.LastModificationTime = DateTime.Now;
                            return mappedVariant;
                        });

                        // Lưu các biến thể mới vào database
                        await _productDetailRepository.AddRangeAsync(newVariantEntities);
                    }
                }
                // Xử lý ảnh: xóa ảnh cũ không còn, thêm ảnh mới
                var oldImages = (await _imageRepository.GetAllImage()).Where(x => x.ProductId == existingProduct.Id).ToList();
                var newImageIds =
                    productDto.MediaUploads?.Where(i => i.Id != null).Select(i => i.Id.Value).ToList() ??
                    new List<Guid>();
                var imagesToDelete = oldImages.Where(img => !newImageIds.Contains(img.Id)).ToList();
                // Xóa file vật lý và record DB
                foreach (var img in imagesToDelete)
                {
                    await _imageRepository.DeleteImage(img.Id);
                }
                // Update relationships
                await UpdateProductRelationships(existingProduct.Id, productDto);

                // Handle uploaded files - follow ProductDetailService pattern
                if (uploadedFiles != null && uploadedFiles.Any())
                {
                    List<Image> images = new List<Image>();
                    foreach (var uploadedFile in uploadedFiles)
                    {

                        var mappedImage = new Image();
                        mappedImage.Id = Guid.NewGuid();
                        mappedImage.ProductId = existingProduct.Id;
                        mappedImage.URL = uploadedFile.RelativePath;
                        mappedImage.Name = uploadedFile.FileName;
                        mappedImage.Type = uploadedFile.FileType ?? 0;
                        images.Add(mappedImage);
                    }
                    await _imageRepository.AddRangeAsync(images);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return new BaseResponse
                {
                    Message = "Cập nhật sản phẩm thành công",
                    ResponseStatus = BaseStatus.Success
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new BaseResponse
                {
                    Message = $"Lỗi khi cập nhật sản phẩm: {ex.Message}",
                    ResponseStatus = BaseStatus.Error
                };
            }
        }

        public async Task<ProductResponseDTO> DeleteAsync(Guid id)
        {
            try
            {
                var product = await _repository.GetProductByIdAsync(id);
                if (product == null)
                {
                    return new ProductResponseDTO
                    {
                        Message = "Không tìm thấy sản phẩm để xóa",
                        ResponseStatus = BaseStatus.Error
                    };
                }

                await _unitOfWork.BeginTransactionAsync();

                // Delete related data
                var variants = await _productDetailRepository.Query()
                    .Where(pd => pd.ProductId == id)
                    .ToListAsync();
                await _productDetailRepository.DeleteRangeAsync(variants);
                // Delete relationships
                var productMaterials = await _productMaterialRepository.Query()
                    .Where(pm => pm.ProductId == id)
                    .ToListAsync();
                await _productMaterialRepository.DeleteRangeAsync(productMaterials);

                var productCategories = await _productCategoryRepository.Query()
                    .Where(pc => pc.ProductId == id)
                    .ToListAsync();
                await _productCategoryRepository.DeleteRangeAsync(productCategories);

                var productSeasons = await _productSeasonRepository.Query()
                    .Where(ps => ps.ProductId == id)
                    .ToListAsync();
                await _productSeasonRepository.DeleteRangeAsync(productSeasons);


                var images = await _imageRepository.Query()
                    .Where(i => i.ProductId == id)
                    .ToListAsync();

                await _imageRepository.DeleteRangeAsync(images);
                // Delete main product
                await _repository.DeleteProductAsync(id);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                var response = _mapper.Map<ProductResponseDTO>(product);
                response.Message = "Xóa sản phẩm thành công";
                response.ResponseStatus = BaseStatus.Success;

                return response;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new ProductResponseDTO
                {
                    Message = $"Lỗi khi xóa sản phẩm: {ex.Message}",
                    ResponseStatus = BaseStatus.Error
                };
            }
        }

        public async Task<List<Image>> GetOldImagesAsync(Guid productId)
        {
            var currentThumbnail = "";
            var currentProduct= await _repository.Query().FirstOrDefaultAsync(c=>c.Id==productId);
            if (currentProduct != null)
            {
                currentThumbnail= currentProduct.Thumbnail;
            }
            //tránh xóa thumbnail để phần orderdetail có lưu Trường Image không bị lỗi
            return await _imageRepository.Query()
                .Where(i => i.ProductId == productId && i.Name !=currentThumbnail)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductResponseDTO>> GetAllAsync()
        {
            var products = await _repository.GetAllProductAsync();
            var result = new List<ProductResponseDTO>();

            foreach (var product in products)
            {
                result.Add(await GetProductWithDetailsAsync(product.Id));
            }

            return result;
        }

        public async Task<CreateOrUpdateProductDTO> GetProductByIdAsync(Guid id)
        {
            try
            {
                var mainResult = await (from product in _repository.Query()
                                        join brand in _brandRepository.Query() on product.BrandId equals brand.Id
                                        where product.Id == id
                                        select new
                                        {
                                            product.Id,
                                            product.Name,
                                            product.BrandId,
                                            BrandName = brand.Name,
                                            product.Thumbnail,
                                            product.Description,
                                            product.CreationTime,
                                            product.LastModificationTime,
                                            product.CreatedBy,
                                            product.UpdatedBy
                                        }).FirstOrDefaultAsync();

                if (mainResult == null)
                {
                    return null;
                }

                var variants = await _productDetailRepository.Query()
                    .Where(pd => pd.ProductId == id)
                    .Include(pd => pd.Size)
                    .Include(pd => pd.Colour)
                    .ToListAsync();

                // Get related IDs
                var materialIds = await _productMaterialRepository.Query()
                    .Where(pm => pm.ProductId == id)
                    .Select(pm => pm.MaterialId)
                    .ToListAsync();

                var categoryIds = await _productCategoryRepository.Query()
                    .Where(pc => pc.ProductId == id)
                    .Select(pc => pc.CategoryId)
                    .ToListAsync();

                var seasonIds = await _productSeasonRepository.Query()
                    .Where(ps => ps.ProductId == id)
                    .Select(ps => ps.SeasonId)
                    .ToListAsync();

                // Get images
                var images = await _imageRepository.Query()
                    .Where(i => i.ProductId == id)
                    .ToListAsync();

                // Build CreateOrUpdateProductDTO
                var result = new CreateOrUpdateProductDTO
                {
                    Id = mainResult.Id,
                    Name = mainResult.Name,
                    Description = mainResult.Description,
                    BrandId = mainResult.BrandId,
                    MaterialIds = materialIds,
                    CategoryIds = categoryIds,
                    SeasonIds = seasonIds,
                    ProductVariants = _mapper.Map<List<ProductDetailGrid>>(variants),
                    MediaUploads = _mapper.Map<List<ProductMediaUpload>>(images)
                };

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProductByIdAsync: {ex.Message}");
                return new CreateOrUpdateProductDTO();

            }
        }

        public async Task<PagingExtensions.PagedResult<ProductResponseDTO, GetListProductResponseDTO>> GetPagedProductsAsync(GetListProductRequestDTO request)
        {
            try
            {
                var productQuery = _repository.Query();
                var brandQuery = _brandRepository.Query();

                // Apply product name filter
                if (!string.IsNullOrEmpty(request.NameFilter))
                {
                    productQuery = productQuery.Where(p => EF.Functions.Like(p.Name, $"%{request.NameFilter}%"));
                }

                // Apply brand filter
                if (request.BrandFilter.HasValue)
                {
                    productQuery = productQuery.Where(p => p.BrandId == request.BrandFilter.Value);
                }

                // Apply status filter (filter by product variants status)
                if (request.StatusFilter.HasValue)
                {
                    var productIdsWithStatus = await _productDetailRepository.Query()
                        .Where(pd => pd.Status == request.StatusFilter.Value)
                        .Select(pd => pd.ProductId)
                        .Distinct()
                        .ToListAsync();
                    productQuery = productQuery.Where(p => productIdsWithStatus.Contains(p.Id));
                }

                // Apply allow return filter
                if (request.AllowReturnFilter.HasValue)
                {
                    var productIdsWithAllowReturn = await _productDetailRepository.Query()
                        .Where(pd => pd.AllowReturn == request.AllowReturnFilter.Value)
                        .Select(pd => pd.ProductId)
                        .Distinct()
                        .ToListAsync();
                    productQuery = productQuery.Where(p => productIdsWithAllowReturn.Contains(p.Id));
                }

                // Apply SKU filter
                if (!string.IsNullOrEmpty(request.SKUFilter))
                {
                    var productIdsWithSKU = await _productDetailRepository.Query()
                        .Where(pd => EF.Functions.Like(pd.Sku, $"%{request.SKUFilter}%"))
                        .Select(pd => pd.ProductId)
                        .Distinct()
                        .ToListAsync();
                    productQuery = productQuery.Where(p => productIdsWithSKU.Contains(p.Id));
                }
                // Apply closure type filter
                if (request.ClosureTypeFilter.HasValue)
                {
                    var productIdsWithClosureType = await _productDetailRepository.Query()
                        .Where(pd => pd.ClosureType == request.ClosureTypeFilter.Value)
                        .Select(pd => pd.ProductId)
                        .Distinct()
                        .ToListAsync();
                    productQuery = productQuery.Where(p => productIdsWithClosureType.Contains(p.Id));
                }

                // Main query with joins
                var mainQuery = from product in productQuery
                                join brand in brandQuery on product.BrandId equals brand.Id
                                select new
                                {
                                    product.Id,
                                    product.Name,
                                    product.BrandId,
                                    BrandName = brand.Name,
                                    product.Thumbnail,
                                    product.CreationTime,
                                    product.LastModificationTime,
                                    product.CreatedBy,
                                    product.UpdatedBy
                                };

                // Apply sorting
                switch (request.SortField)
                {
                    case EProductSortField.Name:
                        mainQuery = request.SortDirection == ESortDirection.Desc
                            ? mainQuery.OrderByDescending(x => x.Name)
                            : mainQuery.OrderBy(x => x.Name);
                        break;
                    case EProductSortField.Price:
                        // Note: Price sorting would need to be implemented differently since price is in variants
                        mainQuery = mainQuery.OrderByDescending(x => x.CreationTime);
                        break;
                    case EProductSortField.CreationTime:
                        mainQuery = request.SortDirection == ESortDirection.Desc
                            ? mainQuery.OrderByDescending(x => x.CreationTime)
                            : mainQuery.OrderBy(x => x.CreationTime);
                        break;
                    default:
                        mainQuery = mainQuery.OrderByDescending(x => x.CreationTime);
                        break;
                }

                var totalRecords = await mainQuery.CountAsync();
                var mainResults = await mainQuery
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                var productIds = mainResults.Select(x => x.Id).ToList();

                // Apply additional filters based on related data
                if (request.CategoryFilter.HasValue || request.MaterialFilter.HasValue || request.SeasonFilter.HasValue ||
                    request.SizeFilter.HasValue || request.ColourFilter.HasValue || request.MinPriceFilter.HasValue || request.MaxPriceFilter.HasValue ||
                    request.MinStockHeightFilter.HasValue || request.MaxStockHeightFilter.HasValue ||
                    request.MinSellNumberFilter.HasValue || request.MaxSellNumberFilter.HasValue)
                {
                    var filteredProductIds = new List<Guid>();

                    // Get all product details for filtering
                    var allProductDetails = await _productDetailRepository.Query()
                        .Where(pd => productIds.Contains(pd.ProductId))
                        .Include(pd => pd.Size)
                        .Include(pd => pd.Colour)
                        .ToListAsync();

                    // Apply size filter
                    if (request.SizeFilter.HasValue)
                    {
                        allProductDetails = allProductDetails.Where(pd => pd.SizeId == request.SizeFilter.Value).ToList();
                    }

                    // Apply color filter
                    if (request.ColourFilter.HasValue)
                    {
                        allProductDetails = allProductDetails.Where(pd => pd.ColourId == request.ColourFilter.Value).ToList();
                    }

                    // Apply price range filter
                    if (request.MinPriceFilter.HasValue || request.MaxPriceFilter.HasValue)
                    {
                        if (request.MinPriceFilter.HasValue)
                        {
                            allProductDetails = allProductDetails.Where(pd => pd.Price >= (float)request.MinPriceFilter.Value).ToList();
                        }
                        if (request.MaxPriceFilter.HasValue)
                        {
                            allProductDetails = allProductDetails.Where(pd => pd.Price <= (float)request.MaxPriceFilter.Value).ToList();
                        }
                    }

                    // Apply stock height range filter
                    if (request.MinStockHeightFilter.HasValue || request.MaxStockHeightFilter.HasValue)
                    {
                        if (request.MinStockHeightFilter.HasValue)
                        {
                            allProductDetails = allProductDetails.Where(pd => pd.StockHeight >= request.MinStockHeightFilter.Value).ToList();
                        }
                        if (request.MaxStockHeightFilter.HasValue)
                        {
                            allProductDetails = allProductDetails.Where(pd => pd.StockHeight <= request.MaxStockHeightFilter.Value).ToList();
                        }
                    }

                    // Apply sell number range filter
                    if (request.MinSellNumberFilter.HasValue || request.MaxSellNumberFilter.HasValue)
                    {
                        if (request.MinSellNumberFilter.HasValue)
                        {
                            allProductDetails = allProductDetails.Where(pd => pd.SellNumber >= request.MinSellNumberFilter.Value).ToList();
                        }
                        if (request.MaxSellNumberFilter.HasValue)
                        {
                            allProductDetails = allProductDetails.Where(pd => pd.SellNumber <= request.MaxSellNumberFilter.Value).ToList();
                        }
                    }

                    // Get filtered product IDs
                    var filteredIds = allProductDetails.Select(pd => pd.ProductId).Distinct().ToList();

                    // Apply category filter
                    if (request.CategoryFilter.HasValue)
                    {
                        var categoryProductIds = await _productCategoryRepository.Query()
                            .Where(pc => pc.CategoryId == request.CategoryFilter.Value && filteredIds.Contains(pc.ProductId))
                            .Select(pc => pc.ProductId)
                            .Distinct()
                            .ToListAsync();
                        filteredIds = filteredIds.Intersect(categoryProductIds).ToList();
                    }

                    // Apply material filter
                    if (request.MaterialFilter.HasValue)
                    {
                        var materialProductIds = await _productMaterialRepository.Query()
                            .Where(pm => pm.MaterialId == request.MaterialFilter.Value && filteredIds.Contains(pm.ProductId))
                            .Select(pm => pm.ProductId)
                            .Distinct()
                            .ToListAsync();
                        filteredIds = filteredIds.Intersect(materialProductIds).ToList();
                    }

                    // Apply season filter
                    if (request.SeasonFilter.HasValue)
                    {
                        var seasonProductIds = await _productSeasonRepository.Query()
                            .Where(ps => ps.SeasonId == request.SeasonFilter.Value && filteredIds.Contains(ps.ProductId))
                            .Select(ps => ps.ProductId)
                            .Distinct()
                            .ToListAsync();
                        filteredIds = filteredIds.Intersect(seasonProductIds).ToList();
                    }

                    // Update mainResults to only include filtered products
                    mainResults = mainResults.Where(m => filteredIds.Contains(m.Id)).ToList();
                    productIds = mainResults.Select(x => x.Id).ToList();
                }

                // Batch queries for related data
                var variantsDict = await _productDetailRepository.Query()
                    .Where(pd => productIds.Contains(pd.ProductId))
                    .Include(pd => pd.Size)
                    .Include(pd => pd.Colour)
                    .GroupBy(pd => pd.ProductId)
                    .ToDictionaryAsync(g => g.Key, g => g.ToList());

                // Compute inventory quantity per variant (ProductDetail)
                var allVariantIds = variantsDict.Values
                    .SelectMany(v => v)
                    .Select(v => v.Id)
                    .Distinct()
                    .ToList();

                var inventoryQuantityByVariantId = await _inventoryBatchRepository.Query()
                    .Where(b => allVariantIds.Contains(b.ProductDetailId) && b.Status == EInventoryBatchStatus.Approved)
                    .GroupBy(b => b.ProductDetailId)
                    .ToDictionaryAsync(g => g.Key, g => g.Sum(x => x.Quantity));

                var materialIdsDict = await _productMaterialRepository.Query()
                    .Where(pm => productIds.Contains(pm.ProductId))
                    .GroupBy(pm => pm.ProductId)
                    .ToDictionaryAsync(g => g.Key, g => g.Select(pm => pm.MaterialId).ToList());

                var categoryIdsDict = await _productCategoryRepository.Query()
                    .Where(pc => productIds.Contains(pc.ProductId))
                    .GroupBy(pc => pc.ProductId)
                    .ToDictionaryAsync(g => g.Key, g => g.Select(pc => pc.CategoryId).ToList());

                var seasonIdsDict = await _productSeasonRepository.Query()
                    .Where(ps => productIds.Contains(ps.ProductId))
                    .GroupBy(ps => ps.ProductId)
                    .ToDictionaryAsync(g => g.Key, g => g.Select(ps => ps.SeasonId).ToList());

                var imageIdsDict = await _imageRepository.Query()
                    .Where(i => productIds.Contains(i.ProductId))
                    .GroupBy(i => i.ProductId)
                    .ToDictionaryAsync(g => g.Key, g => g.ToList());

                var allMaterialIds = materialIdsDict.Values.SelectMany(x => x).Distinct().ToList();
                var allCategoryIds = categoryIdsDict.Values.SelectMany(x => x).Distinct().ToList();
                var allSeasonIds = seasonIdsDict.Values.SelectMany(x => x).Distinct().ToList();

                var materialsDict = await _materialRepository.Query()
                    .Where(m => allMaterialIds.Contains(m.Id))
                    .ToDictionaryAsync(m => m.Id, m => m.Name);

                var categoriesDict = await _categoryRepository.Query()
                    .Where(c => allCategoryIds.Contains(c.Id))
                    .ToDictionaryAsync(c => c.Id, c => c.Name);

                var seasonsDict = await _seasonRepository.Query()
                    .Where(s => allSeasonIds.Contains(s.Id))
                    .ToDictionaryAsync(s => s.Id, s => s.Name);

                // Build response
                var items = mainResults.Select(main =>
                {
                    var variants = variantsDict.TryGetValue(main.Id, out var variantList) ? variantList : new List<ProductDetail>();
                    var materialIds = materialIdsDict.TryGetValue(main.Id, out var matIds) ? matIds : new List<Guid>();
                    var categoryIds = categoryIdsDict.TryGetValue(main.Id, out var catIds) ? catIds : new List<Guid>();
                    var seasonIds = seasonIdsDict.TryGetValue(main.Id, out var seaIds) ? seaIds : new List<Guid>();
                    var images = imageIdsDict.TryGetValue(main.Id, out var imgList) ? imgList : new List<Image>();

                    return new ProductResponseDTO
                    {
                        Id = main.Id,
                        Name = main.Name,
                        BrandId = main.BrandId,
                        BrandName = main.BrandName,
                        Thumbnail = main.Thumbnail,
                        CreationTime = main.CreationTime,
                        LastModificationTime = main.LastModificationTime,
                        CreatedBy = main.CreatedBy,
                        UpdatedBy = main.UpdatedBy,

                        // Set related collections
                        SizeIds = variants.Select(v => v.SizeId).Distinct().ToList(),
                        SizeValues = variants.Select(v => v.Size?.Value ?? string.Empty).Distinct().ToList(),
                        ColourIds = variants.Select(v => v.ColourId).Distinct().ToList(),
                        ColourNames = variants.Select(v => v.Colour?.Name ?? string.Empty).Distinct().ToList(),
                        MaterialIds = materialIds,
                        MaterialNames = materialIds.Select(id => materialsDict.GetValueOrDefault(id, string.Empty)).ToList(),
                        CategoryIds = categoryIds,
                        CategoryNames = categoryIds.Select(id => categoriesDict.GetValueOrDefault(id, string.Empty)).ToList(),
                        SeasonIds = seasonIds,
                        SeasonNames = seasonIds.Select(id => seasonsDict.GetValueOrDefault(id, string.Empty)).ToList(),
                        // Set variants with inventory quantity
                        ProductVariants = variants.Select(v =>
                        {
                            var dto = _mapper.Map<ProductDetailGrid>(v);
                            dto.InventoryQuantity = inventoryQuantityByVariantId.TryGetValue(v.Id, out var qty) ? qty : 0;
                            return dto;
                        }).ToList(),
                        Media = _mapper.Map<List<ProductMediaUpload>>(images)
                    };
                }).ToList();

                // Calculate metadata counts from ALL products in database (fixed numbers, not affected by filters)
                var totalAllProducts = await _repository.Query().CountAsync();

                // Get status counts in one query for better performance
                var statusCounts = await _productDetailRepository.Query()
                    .GroupBy(pd => pd.Status)
                    .Select(g => new { Status = g.Key, Count = g.Select(x => x.ProductId).Distinct().Count() })
                    .ToListAsync();

                var sellingCount = statusCounts.FirstOrDefault(x => x.Status == EProductStatus.Selling)?.Count ?? 0;
                var stopSellingCount = statusCounts.FirstOrDefault(x => x.Status == EProductStatus.StopSelling)?.Count ?? 0;
                var pendingCount = statusCounts.FirstOrDefault(x => x.Status == EProductStatus.Pending)?.Count ?? 0;
                var rejectedCount = statusCounts.FirstOrDefault(x => x.Status == EProductStatus.Rejected)?.Count ?? 0;

                return new PagingExtensions.PagedResult<ProductResponseDTO, GetListProductResponseDTO>
                {
                    TotalRecords = totalRecords,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    Items = items,
                    Metadata = new GetListProductResponseDTO
                    {
                        TotalAll = totalAllProducts,
                        Selling = sellingCount,
                        StopSelling = stopSellingCount,
                        Pending = pendingCount,
                        Rejected = rejectedCount
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPagedProductsAsync: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get paged products for Portal with simplified filtering, proper variant status filtering, and discount calculation
        /// </summary>
        public async Task<PagingExtensions.PagedResult<ProductResponseDTO, GetListProductResponseDTO>> GetPagedProductsForPortalAsync(GetListProductRequestDTO request)
        {
            try
            {
                // Step 1: Build base product query with basic filters
                var productQuery = _repository.Query();
                var brandQuery = _brandRepository.Query();

                // Apply basic product filters
                if (!string.IsNullOrEmpty(request.NameFilter))
                {
                    productQuery = productQuery.Where(p => EF.Functions.Like(p.Name, $"%{request.NameFilter}%"));
                }

                if (request.BrandFilter.HasValue)
                {
                    productQuery = productQuery.Where(p => p.BrandId == request.BrandFilter.Value);
                }

                // Step 2: Get products with selling variants (CRITICAL FILTER)
                var productsWithSellingVariants = await _productDetailRepository.Query()
                    .Where(pd => pd.Status == EProductStatus.Selling)
                    .Select(pd => pd.ProductId)
                    .Distinct()
                    .ToListAsync();

                productQuery = productQuery.Where(p => productsWithSellingVariants.Contains(p.Id));

                // Step 3: Apply complex filters (Category, Material, Season, Size, Color, Price)
                var filteredProductIds = await ApplyComplexFiltersAsync(request, productsWithSellingVariants);

                if (filteredProductIds.Any())
                {
                    productQuery = productQuery.Where(p => filteredProductIds.Contains(p.Id));
                }

                // Step 4: Build main query with joins
                var mainQuery = from product in productQuery
                                join brand in brandQuery on product.BrandId equals brand.Id
                                select new
                                {
                                    product.Id,
                                    product.Name,
                                    product.BrandId,
                                    BrandName = brand.Name,
                                    product.Thumbnail,
                                    product.CreationTime,
                                    product.LastModificationTime,
                                    product.CreatedBy,
                                    product.UpdatedBy
                                };

                // Step 5: Apply sorting
                mainQuery = ApplySorting(mainQuery, request);

                // Step 6: Count and paginate
                var totalRecords = await mainQuery.CountAsync();
                var mainResults = await mainQuery
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                var productIds = mainResults.Select(x => x.Id).ToList();

                // Batch queries for related data
                var variantsDict = await _productDetailRepository.Query()
                    .Where(pd => productIds.Contains(pd.ProductId))
                    .Include(pd => pd.Size)
                    .Include(pd => pd.Colour)
                    .GroupBy(pd => pd.ProductId)
                    .ToDictionaryAsync(g => g.Key, g => g.ToList());

                // Compute inventory quantity per variant (ProductDetail)
                var allVariantIds = variantsDict.Values
                    .SelectMany(v => v)
                    .Select(v => v.Id)
                    .Distinct()
                    .ToList();

                var inventoryQuantityByVariantId = await _inventoryBatchRepository.Query()
                    .Where(b => allVariantIds.Contains(b.ProductDetailId) && b.Status == EInventoryBatchStatus.Approved)
                    .GroupBy(b => b.ProductDetailId)
                    .ToDictionaryAsync(g => g.Key, g => g.Sum(x => x.Quantity));

                // Get active promotions for all variants
                var now = DateTime.Now;
                var activePromotionsDict = await _promotionDetailRepository.Query()
                    .Where(pd => allVariantIds.Contains(pd.ProductDetailId) &&
                               pd.Promotion != null &&
                               pd.Promotion.StartDate <= now &&
                               pd.Promotion.EndDate >= now)
                    .Include(pd => pd.Promotion)
                    .GroupBy(pd => pd.ProductDetailId)
                    .ToDictionaryAsync(g => g.Key, g => g.OrderByDescending(pd => pd.Discount).First());

                var materialIdsDict = await _productMaterialRepository.Query()
                    .Where(pm => productIds.Contains(pm.ProductId))
                    .GroupBy(pm => pm.ProductId)
                    .ToDictionaryAsync(g => g.Key, g => g.Select(pm => pm.MaterialId).ToList());

                var categoryIdsDict = await _productCategoryRepository.Query()
                    .Where(pc => productIds.Contains(pc.ProductId))
                    .GroupBy(pc => pc.ProductId)
                    .ToDictionaryAsync(g => g.Key, g => g.Select(pc => pc.CategoryId).ToList());

                var seasonIdsDict = await _productSeasonRepository.Query()
                    .Where(ps => productIds.Contains(ps.ProductId))
                    .GroupBy(ps => ps.ProductId)
                    .ToDictionaryAsync(g => g.Key, g => g.Select(ps => ps.SeasonId).ToList());

                var imageIdsDict = await _imageRepository.Query()
                    .Where(i => productIds.Contains(i.ProductId))
                    .GroupBy(i => i.ProductId)
                    .ToDictionaryAsync(g => g.Key, g => g.ToList());

                var allMaterialIds = materialIdsDict.Values.SelectMany(x => x).Distinct().ToList();
                var allCategoryIds = categoryIdsDict.Values.SelectMany(x => x).Distinct().ToList();
                var allSeasonIds = seasonIdsDict.Values.SelectMany(x => x).Distinct().ToList();

                var materialsDict = await _materialRepository.Query()
                    .Where(m => allMaterialIds.Contains(m.Id))
                    .ToDictionaryAsync(m => m.Id, m => m.Name);

                var categoriesDict = await _categoryRepository.Query()
                    .Where(c => allCategoryIds.Contains(c.Id))
                    .ToDictionaryAsync(c => c.Id, c => c.Name);

                var seasonsDict = await _seasonRepository.Query()
                    .Where(s => allSeasonIds.Contains(s.Id))
                    .ToDictionaryAsync(s => s.Id, s => s.Name);

                // Build response
                var items = mainResults.Select(main =>
                {
                    var variants = variantsDict.TryGetValue(main.Id, out var variantList) ? variantList : new List<ProductDetail>();
                    var materialIds = materialIdsDict.TryGetValue(main.Id, out var matIds) ? matIds : new List<Guid>();
                    var categoryIds = categoryIdsDict.TryGetValue(main.Id, out var catIds) ? catIds : new List<Guid>();
                    var seasonIds = seasonIdsDict.TryGetValue(main.Id, out var seaIds) ? seaIds : new List<Guid>();
                    var images = imageIdsDict.TryGetValue(main.Id, out var imgList) ? imgList : new List<Image>();

                    return new ProductResponseDTO
                    {
                        Id = main.Id,
                        Name = main.Name,
                        BrandId = main.BrandId,
                        BrandName = main.BrandName,
                        Thumbnail = main.Thumbnail,
                        CreationTime = main.CreationTime,
                        LastModificationTime = main.LastModificationTime,
                        CreatedBy = main.CreatedBy,
                        UpdatedBy = main.UpdatedBy,

                        // Set related collections
                        SizeIds = variants.Select(v => v.SizeId).Distinct().ToList(),
                        SizeValues = variants.Select(v => v.Size?.Value ?? string.Empty).Distinct().ToList(),
                        ColourIds = variants.Select(v => v.ColourId).Distinct().ToList(),
                        ColourNames = variants.Select(v => v.Colour?.Name ?? string.Empty).Distinct().ToList(),
                        MaterialIds = materialIds,
                        MaterialNames = materialIds.Select(id => materialsDict.GetValueOrDefault(id, string.Empty)).ToList(),
                        CategoryIds = categoryIds,
                        CategoryNames = categoryIds.Select(id => categoriesDict.GetValueOrDefault(id, string.Empty)).ToList(),
                        SeasonIds = seasonIds,
                        SeasonNames = seasonIds.Select(id => seasonsDict.GetValueOrDefault(id, string.Empty)).ToList(),
                        // Set variants with inventory quantity and discount
                        ProductVariants = variants.Select(v =>
                        {
                            var dto = _mapper.Map<ProductDetailGrid>(v);
                            dto.InventoryQuantity = inventoryQuantityByVariantId.TryGetValue(v.Id, out var qty) ? qty : 0;

                            // Set discount for this variant
                            var activePromotion = activePromotionsDict.TryGetValue(v.Id, out var promo) ? promo : null;
                            dto.Discount = activePromotion?.Discount;

                            return dto;
                        }).ToList(),
                        // Calculate MaxDiscount for the product (maximum discount among all variants)
                        MaxDiscount = variants.Any() ? variants.Max(v =>
                        {
                            var activePromotion = activePromotionsDict.TryGetValue(v.Id, out var promo) ? promo : null;
                            return activePromotion?.Discount ?? 0f;
                        }) : 0f,
                        Media = _mapper.Map<List<ProductMediaUpload>>(images)
                    };
                }).ToList();

                return new PagingExtensions.PagedResult<ProductResponseDTO, GetListProductResponseDTO>
                {
                    TotalRecords = totalRecords,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    Items = items,
                    Metadata = new GetListProductResponseDTO
                    {
                        TotalAll = totalRecords,
                        Selling = 0,
                        StopSelling = 0,
                        Pending = 0,
                        Rejected = 0
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPagedProductsForPortalAsync: {ex.Message}");
                throw;
            }
        }


        /// <summary>
        /// Apply complex filters (Category, Material, Season, Size, Color, Price, SKU, ClosureType, AllowReturn)
        /// </summary>
        private async Task<List<Guid>> ApplyComplexFiltersAsync(GetListProductRequestDTO request, List<Guid> productsWithSellingVariants)
        {
            var hasComplexFilters = request.CategoryFilter.HasValue ||
                                  request.MaterialFilter.HasValue ||
                                  request.SeasonFilter.HasValue ||
                                  request.SizeFilter.HasValue ||
                                  request.ColourFilter.HasValue ||
                                  request.MinPriceFilter.HasValue ||
                                  request.MaxPriceFilter.HasValue ||
                                  !string.IsNullOrEmpty(request.SKUFilter) ||
                                  request.ClosureTypeFilter.HasValue ||
                                  request.AllowReturnFilter.HasValue;

            if (!hasComplexFilters)
            {
                return productsWithSellingVariants;
            }

            // Get all product details for filtering
            var allProductDetails = await _productDetailRepository.Query()
                .Where(pd => productsWithSellingVariants.Contains(pd.ProductId))
                .Include(pd => pd.Size)
                .Include(pd => pd.Colour)
                .ToListAsync();

            // Apply variant-level filters
            if (!string.IsNullOrEmpty(request.SKUFilter))
            {
                allProductDetails = allProductDetails.Where(pd =>
                    pd.Sku.Contains(request.SKUFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (request.ClosureTypeFilter.HasValue)
            {
                allProductDetails = allProductDetails.Where(pd => pd.ClosureType == request.ClosureTypeFilter.Value).ToList();
            }

            if (request.AllowReturnFilter.HasValue)
            {
                allProductDetails = allProductDetails.Where(pd => pd.AllowReturn == request.AllowReturnFilter.Value).ToList();
            }

            if (request.SizeFilter.HasValue)
            {
                allProductDetails = allProductDetails.Where(pd => pd.SizeId == request.SizeFilter.Value).ToList();
            }

            if (request.ColourFilter.HasValue)
            {
                allProductDetails = allProductDetails.Where(pd => pd.ColourId == request.ColourFilter.Value).ToList();
            }

            // Apply price range filter
            if (request.MinPriceFilter.HasValue || request.MaxPriceFilter.HasValue)
            {
                if (request.MinPriceFilter.HasValue)
                {
                    allProductDetails = allProductDetails.Where(pd => pd.Price >= (float)request.MinPriceFilter.Value).ToList();
                }
                if (request.MaxPriceFilter.HasValue)
                {
                    allProductDetails = allProductDetails.Where(pd => pd.Price <= (float)request.MaxPriceFilter.Value).ToList();
                }
            }

            // Get filtered product IDs
            var filteredIds = allProductDetails.Select(pd => pd.ProductId).Distinct().ToList();

            // Apply product-level filters
            if (request.CategoryFilter.HasValue)
            {
                var categoryProductIds = await _productCategoryRepository.Query()
                    .Where(pc => pc.CategoryId == request.CategoryFilter.Value && filteredIds.Contains(pc.ProductId))
                    .Select(pc => pc.ProductId)
                    .Distinct()
                    .ToListAsync();
                filteredIds = filteredIds.Intersect(categoryProductIds).ToList();
            }

            if (request.MaterialFilter.HasValue)
            {
                var materialProductIds = await _productMaterialRepository.Query()
                    .Where(pm => pm.MaterialId == request.MaterialFilter.Value && filteredIds.Contains(pm.ProductId))
                    .Select(pm => pm.ProductId)
                    .Distinct()
                    .ToListAsync();
                filteredIds = filteredIds.Intersect(materialProductIds).ToList();
            }

            if (request.SeasonFilter.HasValue)
            {
                var seasonProductIds = await _productSeasonRepository.Query()
                    .Where(ps => ps.SeasonId == request.SeasonFilter.Value && filteredIds.Contains(ps.ProductId))
                    .Select(ps => ps.ProductId)
                    .Distinct()
                    .ToListAsync();
                filteredIds = filteredIds.Intersect(seasonProductIds).ToList();
            }

            return filteredIds;
        }

        /// <summary>
        /// Apply sorting to the main query
        /// </summary>
        private IQueryable<T> ApplySorting<T>(IQueryable<T> query, GetListProductRequestDTO request) where T : class
        {
            switch (request.SortField)
            {
                case EProductSortField.Name:
                    return request.SortDirection == ESortDirection.Desc
                        ? query.OrderByDescending(x => EF.Property<string>(x, "Name"))
                        : query.OrderBy(x => EF.Property<string>(x, "Name"));

                case EProductSortField.Price:
                    // Note: Price sorting would need to be implemented differently since price is in variants
                    return query.OrderByDescending(x => EF.Property<DateTime>(x, "CreationTime"));

                case EProductSortField.CreationTime:
                    return request.SortDirection == ESortDirection.Desc
                        ? query.OrderByDescending(x => EF.Property<DateTime>(x, "CreationTime"))
                        : query.OrderBy(x => EF.Property<DateTime>(x, "CreationTime"));

                default:
                    return query.OrderByDescending(x => EF.Property<DateTime>(x, "CreationTime"));
            }
        }

        public async Task<ProductResponseDTO> GetProductWithDetailsAsync(Guid id)
        {
            try
            {
                // Main query with join
                var mainResult = await (from product in _repository.Query()
                                        join brand in _brandRepository.Query() on product.BrandId equals brand.Id
                                        where product.Id == id
                                        select new
                                        {
                                            product.Id,
                                            product.Name,
                                            product.BrandId,
                                            BrandName = brand.Name,
                                            product.Thumbnail,
                                            product.Description,
                                            product.CreationTime,
                                            product.LastModificationTime,
                                            product.CreatedBy,
                                            product.UpdatedBy
                                        }).FirstOrDefaultAsync();

                if (mainResult == null)
                {
                    return new ProductResponseDTO
                    {
                        Message = "Không tìm thấy sản phẩm",
                        ResponseStatus = BaseStatus.Error
                    };
                }

                // Batch queries for related data
                var variants = await _productDetailRepository.Query()
                    .Where(pd => pd.ProductId == id)
                    .Include(pd => pd.Size)
                    .Include(pd => pd.Colour)
                    .ToListAsync();

                var materialIds = await _productMaterialRepository.Query()
                    .Where(pm => pm.ProductId == id)
                    .Select(pm => pm.MaterialId)
                    .ToListAsync();

                var categoryIds = await _productCategoryRepository.Query()
                    .Where(pc => pc.ProductId == id)
                    .Select(pc => pc.CategoryId)
                    .ToListAsync();

                var seasonIds = await _productSeasonRepository.Query()
                    .Where(ps => ps.ProductId == id)
                    .Select(ps => ps.SeasonId)
                    .ToListAsync();

                var images = await _imageRepository.Query()
                    .Where(i => i.ProductId == id)
                    .ToListAsync();

                // Get related names
                var materials = await _materialRepository.Query()
                    .Where(m => materialIds.Contains(m.Id))
                    .ToListAsync();

                var categories = await _categoryRepository.Query()
                    .Where(c => categoryIds.Contains(c.Id))
                    .ToListAsync();

                var seasons = await _seasonRepository.Query()
                    .Where(s => seasonIds.Contains(s.Id))
                    .ToListAsync();

                // Build response
                var response = new ProductResponseDTO
                {
                    Id = mainResult.Id,
                    Name = mainResult.Name,
                    BrandId = mainResult.BrandId,
                    BrandName = mainResult.BrandName,
                    Thumbnail = mainResult.Thumbnail,
                    Description = mainResult.Description,
                    CreationTime = mainResult.CreationTime,
                    LastModificationTime = mainResult.LastModificationTime,
                    CreatedBy = mainResult.CreatedBy,
                    UpdatedBy = mainResult.UpdatedBy,
                    ResponseStatus = BaseStatus.Success,
                    Message = "Thành công"
                };

                // Set related collections
                response.SizeIds = variants.Select(v => v.SizeId).Distinct().ToList();
                response.SizeValues = variants.Select(v => v.Size?.Value ?? string.Empty).Distinct().ToList();
                response.ColourIds = variants.Select(v => v.ColourId).Distinct().ToList();
                response.ColourNames = variants.Select(v => v.Colour?.Name ?? string.Empty).Distinct().ToList();
                response.MaterialIds = materialIds;
                response.MaterialNames = materials.Select(m => m.Name).ToList();
                response.CategoryIds = categoryIds;
                response.CategoryNames = categories.Select(c => c.Name).ToList();
                response.SeasonIds = seasonIds;
                response.SeasonNames = seasons.Select(s => s.Name).ToList();

                // Set variants with proper mapping (manual mapping to ensure SizeName and ColourName are set)
                response.ProductVariants = variants.Select(v => new ProductDetailGrid
                {
                    Id = v.Id,
                    ProductId = v.ProductId,
                    Sku = v.Sku,
                    SizeId = v.SizeId,
                    SizeName = v.Size?.Value ?? string.Empty,
                    ColourId = v.ColourId,
                    ColourName = v.Colour?.Name ?? string.Empty,
                    Price = v.Price,
                    Discount = null, // Will be set below
                    OutOfStock = v.OutOfStock,
                    StockHeight = v.StockHeight,
                    ClosureType = v.ClosureType,
                    SellNumber = v.SellNumber,
                    ViewNumber = v.ViewNumber,
                    AllowReturn = v.AllowReturn,
                    Status = v.Status,
                    InventoryQuantity = 0, // Will be calculated below
                    Weight = v.Weight,
                    Length = v.Length,
                    Width = v.Width,
                    Height = v.Height,
                    MaxBuyPerOrder = v.MaxBuyPerOrder
                }).ToList();

                // Calculate inventory quantity for each variant
                var variantIds = variants.Select(v => v.Id).ToList();
                var inventoryQuantityByVariantId = await _inventoryBatchRepository.Query()
                    .Where(b => variantIds.Contains(b.ProductDetailId) && b.Status == EInventoryBatchStatus.Approved)
                    .GroupBy(b => b.ProductDetailId)
                    .ToDictionaryAsync(g => g.Key, g => g.Sum(x => x.Quantity));

                // Get active promotions for variants
                var now = DateTime.Now;
                var activePromotionsDict = await _promotionDetailRepository.Query()
                    .Where(pd => variantIds.Contains(pd.ProductDetailId) &&
                               pd.Promotion != null &&
                               pd.Promotion.StartDate <= now &&
                               pd.Promotion.EndDate >= now)
                    .Include(pd => pd.Promotion)
                    .GroupBy(pd => pd.ProductDetailId)
                    .ToDictionaryAsync(g => g.Key, g => g.OrderByDescending(pd => pd.Discount).First());

                // Update variants with inventory quantity and discount
                foreach (var variant in response.ProductVariants)
                {
                    variant.InventoryQuantity = inventoryQuantityByVariantId.TryGetValue(variant.Id ?? Guid.Empty, out var qty) ? qty : 0;

                    var activePromotion = activePromotionsDict.TryGetValue(variant.Id ?? Guid.Empty, out var promo) ? promo : null;
                    variant.Discount = activePromotion?.Discount;
                }

                response.Media = _mapper.Map<List<ProductMediaUpload>>(images);

                // Set min/max prices and discount
                if (variants.Any())
                {
                    response.MinPrice = variants.Min(v => v.Price);
                    response.MaxPrice = variants.Max(v => v.Price);

                    // Calculate MaxDiscount from variants
                    response.MaxDiscount = response.ProductVariants.Any() ?
                        response.ProductVariants.Max(v => v.Discount ?? 0f) : 0f;

                    response.SaleNumber = variants.Sum(v => v.SellNumber ?? 0);
                }

                // Calculate rating and rating breakdown from reviews
                var reviews = await _reviewRepository.Query()
                    .Where(r => variantIds.Contains(r.ProductDetailId) && !r.IsHidden)
                    .ToListAsync();

                if (reviews.Any())
                {
                    // Tính trung bình cộng của tất cả reviews
                    var averageRating = reviews.Average(r => r.Rating);
                    response.Rating = (decimal)averageRating;
                    response.Rating1 = reviews.Count(r => Math.Round(r.Rating) == 1);
                    response.Rating2 = reviews.Count(r => Math.Round(r.Rating) == 2);
                    response.Rating3 = reviews.Count(r => Math.Round(r.Rating) == 3);
                    response.Rating4 = reviews.Count(r => Math.Round(r.Rating) == 4);
                    response.Rating5 = reviews.Count(r => Math.Round(r.Rating) == 5);

                    // Debug logging
                    _logger.LogInformation($"Product {id}: Reviews count = {reviews.Count}, Average rating = {averageRating}, Final rating = {response.Rating}");
                }
                else
                {
                    response.Rating = 0;
                    response.Rating1 = 0;
                    response.Rating2 = 0;
                    response.Rating3 = 0;
                    response.Rating4 = 0;
                    response.Rating5 = 0;
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProductWithDetailsAsync: {ex.Message}");
                return new ProductResponseDTO
                {
                    Message = $"Lỗi khi lấy thông tin sản phẩm: {ex.Message}",
                    ResponseStatus = BaseStatus.Error
                };
            }
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

            // Lấy sản phẩm bán chạy với thông tin ProductDetail được bán nhiều nhất
            var soldByProductDetail = await _orderDetailRepository.Query()
                .Include(od => od.Order)
                .Include(od => od.ProductDetail)
                .ThenInclude(pd => pd.Size)
                .Include(od => od.ProductDetail)
                .ThenInclude(pd => pd.Colour)
                .Where(od => od.Order != null && od.Order.CreationTime >= startOfWeek && od.Order.CreationTime < endOfWeek)
                .GroupBy(od => new
                {
                    ProductId = od.ProductDetail.ProductId,
                    ProductDetailId = od.ProductDetail.Id,
                    SizeValue = od.ProductDetail.Size.Value,
                    ColourName = od.ProductDetail.Colour.Name,
                    Price = od.ProductDetail.Price,
                    Discount = od.Discount
                })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    ProductDetailId = g.Key.ProductDetailId,
                    SizeValue = g.Key.SizeValue,
                    ColourName = g.Key.ColourName,
                    Price = g.Key.Price,
                    Discount = g.Key.Discount,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(take)
                .ToListAsync();

            var productIds = soldByProductDetail.Select(x => x.ProductId).Distinct().ToList();
            var products = await _repository.Query()
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p);

            var results = new List<BestSellerItemDTO>();
            foreach (var item in soldByProductDetail)
            {
                if (products.TryGetValue(item.ProductId, out var product))
                {
                    results.Add(new BestSellerItemDTO
                    {
                        ProductId = product.Id,
                        ProductDetailId = item.ProductDetailId,
                        Name = product.Name,
                        Thumbnail = product.Thumbnail,
                        TotalSold = item.TotalSold,
                        Price = item.Price,
                        Discount = item.Discount,
                        SizeValue = item.SizeValue,
                        ColourName = item.ColourName
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
                        StockHeight = v.StockHeight,

                        // Thông tin vận chuyển
                        Weight = v.Weight,
                        Length = v.Length,
                        Width = v.Width,
                        Height = v.Height,

                        // Giới hạn mua hàng
                        MaxBuyPerOrder = v.MaxBuyPerOrder
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

        public async Task<List<TopRatedProductDTO>> GetTopRatedProductsAsync(int take = 12)
        {
            // Aggregate average rating by product via ProductDetail
            var ratingStats = await _reviewRepository.Query()
                .Include(r => r.ProductDetail)
                .Where(r => !r.IsHidden && r.ProductDetail != null)
                .GroupBy(r => r.ProductDetail.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    AverageRating = g.Average(x => x.Rating),
                    ReviewCount = g.Count()
                })
                .OrderByDescending(x => x.AverageRating)
                .ThenByDescending(x => x.ReviewCount)
                .Take(take)
                .ToListAsync();

            var productIds = ratingStats.Select(x => x.ProductId).ToList();
            if (!productIds.Any()) return new List<TopRatedProductDTO>();

            var products = await _repository.Query()
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p);

            var minMaxPrices = await _productDetailRepository.Query()
                .Where(pd => productIds.Contains(pd.ProductId))
                .GroupBy(pd => pd.ProductId)
                .Select(g => new { ProductId = g.Key, MinPrice = g.Min(x => x.Price), MaxPrice = g.Max(x => x.Price) })
                .ToListAsync();
            var priceDict = minMaxPrices.ToDictionary(x => x.ProductId, x => (Min: (float?)x.MinPrice, Max: (float?)x.MaxPrice));

            var result = new List<TopRatedProductDTO>();
            foreach (var stat in ratingStats)
            {
                if (products.TryGetValue(stat.ProductId, out var product))
                {
                    result.Add(new TopRatedProductDTO
                    {
                        ProductId = product.Id,
                        Name = product.Name,
                        Thumbnail = product.Thumbnail,
                        AverageRating = (float)Math.Round(stat.AverageRating, 2),
                        ReviewCount = stat.ReviewCount,
                        MinPrice = priceDict.ContainsKey(product.Id) ? priceDict[product.Id].Min : null,
                        MaxPrice = priceDict.ContainsKey(product.Id) ? priceDict[product.Id].Max : null
                    });
                }
            }

            // Ensure the original order by rating is preserved
            var orderMap = ratingStats.Select((x, idx) => new { x.ProductId, idx }).ToDictionary(x => x.ProductId, x => x.idx);
            result = result.OrderBy(r => orderMap.ContainsKey(r.ProductId) ? orderMap[r.ProductId] : int.MaxValue).ToList();
            return result;
        }

        public async Task<Dictionary<Guid, List<ProductResponseDTO>>> GetHeaderProductsAsync()
        {
            // Lấy tất cả categories
            var categories = await _categoryRepository.Query()
                .Where(c => c.Status == true)
                .Take(5) // Chỉ lấy 5 categories đầu tiên
                .ToListAsync();

            var result = new Dictionary<Guid, List<ProductResponseDTO>>();

            foreach (var category in categories)
            {
                // Step 1: Lấy sản phẩm có variants đang bán (giống như Portal API)
                var productsWithSellingVariants = await _productDetailRepository.Query()
                    .Where(pd => pd.Status == EProductStatus.Selling)
                    .Select(pd => pd.ProductId)
                    .Distinct()
                    .ToListAsync();

                // Step 2: Lấy 4 sản phẩm đầu tiên của mỗi category với thông tin brand và chỉ lấy những sản phẩm có variants đang bán
                var mainQuery = from product in _repository.Query()
                                join brand in _brandRepository.Query() on product.BrandId equals brand.Id
                                join productCategory in _productCategoryRepository.Query() on product.Id equals productCategory.ProductId
                                where productCategory.CategoryId == category.Id
                                      && productsWithSellingVariants.Contains(product.Id)
                                select new
                                {
                                    product.Id,
                                    product.Name,
                                    product.BrandId,
                                    BrandName = brand.Name,
                                    product.Thumbnail,
                                    product.CreationTime,
                                    product.LastModificationTime,
                                    product.CreatedBy,
                                    product.UpdatedBy
                                };

                var mainResults = await mainQuery
                    .OrderByDescending(x => x.CreationTime)
                    .Take(4)
                    .ToListAsync();

                var productIds = mainResults.Select(x => x.Id).ToList();

                if (!productIds.Any())
                {
                    result[category.Id] = new List<ProductResponseDTO>();
                    continue;
                }

                // Batch queries for related data (giống như GetPagedProductsAsync) - chỉ lấy variants đang bán
                var variantsDict = await _productDetailRepository.Query()
                    .Where(pd => productIds.Contains(pd.ProductId) && pd.Status == EProductStatus.Selling)
                    .Include(pd => pd.Size)
                    .Include(pd => pd.Colour)
                    .GroupBy(pd => pd.ProductId)
                    .ToDictionaryAsync(g => g.Key, g => g.ToList());

                // Get all variant IDs for promotion query
                var allVariantIds = variantsDict.Values
                    .SelectMany(v => v)
                    .Select(v => v.Id)
                    .Distinct()
                    .ToList();

                // Get active promotions for all variants
                var now = DateTime.Now;
                var activePromotionsDict = await _promotionDetailRepository.Query()
                    .Where(pd => allVariantIds.Contains(pd.ProductDetailId) &&
                               pd.Promotion != null &&
                               pd.Promotion.StartDate <= now &&
                               pd.Promotion.EndDate >= now)
                    .Include(pd => pd.Promotion)
                    .GroupBy(pd => pd.ProductDetailId)
                    .ToDictionaryAsync(g => g.Key, g => g.OrderByDescending(pd => pd.Discount).First());

                var materialIdsDict = await _productMaterialRepository.Query()
                    .Where(pm => productIds.Contains(pm.ProductId))
                    .GroupBy(pm => pm.ProductId)
                    .ToDictionaryAsync(g => g.Key, g => g.Select(pm => pm.MaterialId).ToList());

                var categoryIdsDict = await _productCategoryRepository.Query()
                    .Where(pc => productIds.Contains(pc.ProductId))
                    .GroupBy(pc => pc.ProductId)
                    .ToDictionaryAsync(g => g.Key, g => g.Select(pc => pc.CategoryId).ToList());

                var seasonIdsDict = await _productSeasonRepository.Query()
                    .Where(ps => productIds.Contains(ps.ProductId))
                    .GroupBy(ps => ps.ProductId)
                    .ToDictionaryAsync(g => g.Key, g => g.Select(ps => ps.SeasonId).ToList());

                var imageIdsDict = await _imageRepository.Query()
                    .Where(i => productIds.Contains(i.ProductId))
                    .GroupBy(i => i.ProductId)
                    .ToDictionaryAsync(g => g.Key, g => g.ToList());

                var allMaterialIds = materialIdsDict.Values.SelectMany(x => x).Distinct().ToList();
                var allCategoryIds = categoryIdsDict.Values.SelectMany(x => x).Distinct().ToList();
                var allSeasonIds = seasonIdsDict.Values.SelectMany(x => x).Distinct().ToList();

                var materialsDict = await _materialRepository.Query()
                    .Where(m => allMaterialIds.Contains(m.Id))
                    .ToDictionaryAsync(m => m.Id, m => m.Name);

                var categoriesDict = await _categoryRepository.Query()
                    .Where(c => allCategoryIds.Contains(c.Id))
                    .ToDictionaryAsync(c => c.Id, c => c.Name);

                var seasonsDict = await _seasonRepository.Query()
                    .Where(s => allSeasonIds.Contains(s.Id))
                    .ToDictionaryAsync(s => s.Id, s => s.Name);

                // Build response (giống như GetPagedProductsAsync)
                var productDtos = mainResults.Select(main =>
                {
                    var variants = variantsDict.TryGetValue(main.Id, out var variantList) ? variantList : new List<ProductDetail>();
                    var materialIds = materialIdsDict.TryGetValue(main.Id, out var matIds) ? matIds : new List<Guid>();
                    var categoryIds = categoryIdsDict.TryGetValue(main.Id, out var catIds) ? catIds : new List<Guid>();
                    var seasonIds = seasonIdsDict.TryGetValue(main.Id, out var seaIds) ? seaIds : new List<Guid>();
                    var images = imageIdsDict.TryGetValue(main.Id, out var imgList) ? imgList : new List<Image>();

                    return new ProductResponseDTO
                    {
                        Id = main.Id,
                        Name = main.Name,
                        BrandId = main.BrandId,
                        BrandName = main.BrandName,
                        Thumbnail = main.Thumbnail,
                        CreationTime = main.CreationTime,
                        LastModificationTime = main.LastModificationTime,
                        CreatedBy = main.CreatedBy,
                        UpdatedBy = main.UpdatedBy,

                        // Set related collections
                        SizeIds = variants.Select(v => v.SizeId).Distinct().ToList(),
                        SizeValues = variants.Select(v => v.Size?.Value ?? string.Empty).Distinct().ToList(),
                        ColourIds = variants.Select(v => v.ColourId).Distinct().ToList(),
                        ColourNames = variants.Select(v => v.Colour?.Name ?? string.Empty).Distinct().ToList(),
                        MaterialIds = materialIds,
                        MaterialNames = materialIds.Select(id => materialsDict.GetValueOrDefault(id, string.Empty)).ToList(),
                        CategoryIds = categoryIds,
                        CategoryNames = categoryIds.Select(id => categoriesDict.GetValueOrDefault(id, string.Empty)).ToList(),
                        SeasonIds = seasonIds,
                        SeasonNames = seasonIds.Select(id => seasonsDict.GetValueOrDefault(id, string.Empty)).ToList(),
                        // Set variants with discount
                        ProductVariants = variants.Select(v =>
                        {
                            var dto = _mapper.Map<ProductDetailGrid>(v);

                            // Set discount for this variant
                            var activePromotion = activePromotionsDict.TryGetValue(v.Id, out var promo) ? promo : null;
                            dto.Discount = activePromotion?.Discount;

                            return dto;
                        }).ToList(),
                        // Calculate MaxDiscount for the product (maximum discount among all variants)
                        MaxDiscount = variants.Any() ? variants.Max(v =>
                        {
                            var activePromotion = activePromotionsDict.TryGetValue(v.Id, out var promo) ? promo : null;
                            return activePromotion?.Discount ?? 0f;
                        }) : 0f,
                        Media = _mapper.Map<List<ProductMediaUpload>>(images),
                        // Set min/max prices
                        MinPrice = variants.Any() ? variants.Min(v => v.Price) : null,
                        MaxPrice = variants.Any() ? variants.Max(v => v.Price) : null
                    };
                }).ToList();

                result[category.Id] = productDtos;
            }

            return result;
        }

        public async Task<PagingExtensions.PagedResult<ProductSearchResponseDTO>> SearchProductsAsync(ProductSearchRequestDTO request)
        {
            try
            {
                var query = _productDetailRepository.Query()
                    .Include(pd => pd.Product)
                    .Include(pd => pd.Size)
                    .Include(pd => pd.Colour)
                    .Include(pd => pd.Product.Brand)
                    .Where(pd => pd.Status == EProductStatus.Selling) // Chỉ lấy sản phẩm đang bán
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(request.SearchKeyword))
                {
                    var keyword = request.SearchKeyword.ToLower();
                    query = query.Where(pd =>
                        pd.Product.Name.ToLower().Contains(keyword) ||
                        pd.Sku.ToLower().Contains(keyword));
                }

                if (!string.IsNullOrEmpty(request.SKUFilter))
                {
                    query = query.Where(pd => pd.Sku.ToLower().Contains(request.SKUFilter.ToLower()));
                }

                if (!string.IsNullOrEmpty(request.NameFilter))
                {
                    query = query.Where(pd => pd.Product.Name.ToLower().Contains(request.NameFilter.ToLower()));
                }

                if (request.BrandId.HasValue)
                {
                    query = query.Where(pd => pd.Product.BrandId == request.BrandId.Value);
                }

                if (request.CategoryId.HasValue)
                {
                    var productIdsWithCategory = await _productCategoryRepository.Query()
                        .Where(pc => pc.CategoryId == request.CategoryId.Value)
                        .Select(pc => pc.ProductId)
                        .Distinct()
                        .ToListAsync();
                    query = query.Where(pd => productIdsWithCategory.Contains(pd.ProductId));
                }

                if (request.MinPrice.HasValue)
                {
                    query = query.Where(pd => pd.Price >= (float)request.MinPrice.Value);
                }

                if (request.MaxPrice.HasValue)
                {
                    query = query.Where(pd => pd.Price <= (float)request.MaxPrice.Value);
                }

                if (request.SizeId.HasValue)
                {
                    query = query.Where(pd => pd.SizeId == request.SizeId.Value);
                }

                if (request.ColourId.HasValue)
                {
                    query = query.Where(pd => pd.ColourId == request.ColourId.Value);
                }

                // Calculate stock quantity and filter only products with stock > 0
                var productDetailsWithStock = await (from pd in query
                                                     select new
                                                     {
                                                         ProductDetail = pd,
                                                         StockQuantity = (from ib in _inventoryBatchRepository.Query()
                                                                          where ib.ProductDetailId == pd.Id && ib.Status == EInventoryBatchStatus.Approved
                                                                          select ib.Quantity).Sum()
                                                     }).ToListAsync();

                // Filter only products with stock > 0
                if (request.InStockOnly == true)
                {
                    productDetailsWithStock = productDetailsWithStock.Where(x => x.StockQuantity > 0).ToList();
                }

                var totalCount = productDetailsWithStock.Count();

                var pagedResults = productDetailsWithStock
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                var mainResults = pagedResults.Select(x => x.ProductDetail).ToList();
                var stockQuantityDict = pagedResults.ToDictionary(x => x.ProductDetail.Id, x => x.StockQuantity);

                var productIds = mainResults.Select(pd => pd.ProductId).Distinct().ToList();
                var productDetailIds = mainResults.Select(pd => pd.Id).Distinct().ToList();

                // Batch queries for related data
                var materialIdsDict = await _productMaterialRepository.Query()
                    .Where(pm => productIds.Contains(pm.ProductId))
                    .GroupBy(pm => pm.ProductId)
                    .ToDictionaryAsync(g => g.Key, g => g.Select(pm => pm.MaterialId).ToList());

                var categoryIdsDict = await _productCategoryRepository.Query()
                    .Where(pc => productIds.Contains(pc.ProductId))
                    .GroupBy(pc => pc.ProductId)
                    .ToDictionaryAsync(g => g.Key, g => g.Select(pc => pc.CategoryId).ToList());

                var seasonIdsDict = await _productSeasonRepository.Query()
                    .Where(ps => productIds.Contains(ps.ProductId))
                    .GroupBy(ps => ps.ProductId)
                    .ToDictionaryAsync(g => g.Key, g => g.Select(ps => ps.SeasonId).ToList());

                var allMaterialIds = materialIdsDict.Values.SelectMany(x => x).Distinct().ToList();
                var allCategoryIds = categoryIdsDict.Values.SelectMany(x => x).Distinct().ToList();
                var allSeasonIds = seasonIdsDict.Values.SelectMany(x => x).Distinct().ToList();

                var materialsDict = await _materialRepository.Query()
                    .Where(m => allMaterialIds.Contains(m.Id))
                    .ToDictionaryAsync(m => m.Id, m => m.Name);

                var categoriesDict = await _categoryRepository.Query()
                    .Where(c => allCategoryIds.Contains(c.Id))
                    .ToDictionaryAsync(c => c.Id, c => c.Name);

                var seasonsDict = await _seasonRepository.Query()
                    .Where(s => allSeasonIds.Contains(s.Id))
                    .ToDictionaryAsync(s => s.Id, s => s.Name);

                // Get ProductDetail Images
                var imageUrlsDict = await _imageRepository.Query()
                    .Where(i => productIds.Contains(i.ProductId)) // Images liên kết với Product, không phải ProductDetail
                    .GroupBy(i => i.ProductId)
                    .ToDictionaryAsync(g => g.Key, g => g.Select(i => i.URL).ToList());

                // Get active promotions for these product details
                var now = DateTime.Now;
                var activePromotionsDict = await _promotionDetailRepository.Query()
                    .Include(pd => pd.Promotion)
                    .Where(pd => productDetailIds.Contains(pd.ProductDetailId) &&
                                pd.Promotion.StartDate <= now &&
                                pd.Promotion.EndDate >= now)
                    .GroupBy(pd => pd.ProductDetailId)
                    .ToDictionaryAsync(g => g.Key, g => g.OrderByDescending(pd => pd.Discount).First());

                // Build response
                var items = mainResults.Select(pd =>
                {
                    var materialIds = materialIdsDict.TryGetValue(pd.ProductId, out var matIds) ? matIds : new List<Guid>();
                    var categoryIds = categoryIdsDict.TryGetValue(pd.ProductId, out var catIds) ? catIds : new List<Guid>();
                    var seasonIds = seasonIdsDict.TryGetValue(pd.ProductId, out var seaIds) ? seaIds : new List<Guid>();
                    var imageUrls = imageUrlsDict.TryGetValue(pd.ProductId, out var imgUrls) ? imgUrls : new List<string>();

                    // Get promotion info
                    var activePromotion = activePromotionsDict.TryGetValue(pd.Id, out var promo) ? promo : null;
                    var originalPrice = (decimal)pd.Price;
                    var salePrice = activePromotion != null ? originalPrice * (1 - (decimal)activePromotion.Discount / 100) : (decimal?)null;
                    var maxDiscount = activePromotion?.Discount;

                    // Calculate rating for this product detail
                    var reviews = _reviewRepository.Query()
                        .Where(r => r.ProductDetailId == pd.Id && !r.IsHidden)
                        .ToList();

                    decimal rating = 0;
                    int rating1 = 0, rating2 = 0, rating3 = 0, rating4 = 0, rating5 = 0;

                    if (reviews.Any())
                    {
                        rating = (decimal)reviews.Average(r => r.Rating);
                        rating1 = reviews.Count(r => Math.Round(r.Rating) == 1);
                        rating2 = reviews.Count(r => Math.Round(r.Rating) == 2);
                        rating3 = reviews.Count(r => Math.Round(r.Rating) == 3);
                        rating4 = reviews.Count(r => Math.Round(r.Rating) == 4);
                        rating5 = reviews.Count(r => Math.Round(r.Rating) == 5);
                    }

                    return new ProductSearchResponseDTO
                    {
                        ProductDetailId = pd.Id,
                        ProductId = pd.ProductId,
                        SKU = pd.Sku,
                        ProductName = pd.Product.Name,
                        BrandName = pd.Product.Brand.Name,
                        SizeValue = pd.Size.Value,
                        ColourName = pd.Colour.Name,
                        Price = originalPrice,
                        SalePrice = salePrice,
                        StockQuantity = stockQuantityDict.GetValueOrDefault(pd.Id, 0),
                        OutOfStock = stockQuantityDict.GetValueOrDefault(pd.Id, 0) == 0 ? 1 : 0,
                        Thumbnail = pd.Product.Thumbnail,
                        Description = pd.Product.Description ?? string.Empty,
                        Rating = rating,
                        SaleNumber = pd.SellNumber ?? 0,
                        IsActive = pd.Status == EProductStatus.Selling,
                        AllowReturn = pd.AllowReturn,
                        MaxDiscount = maxDiscount != null ? (decimal)maxDiscount : (decimal?)null,
                        Weight = pd.Weight,
                        Dimensions = $"{pd.Length}x{pd.Width}x{pd.Height}",
                        Material = string.Join(", ", materialIds.Select(id => materialsDict.GetValueOrDefault(id, string.Empty))),
                        Season = string.Join(", ", seasonIds.Select(id => seasonsDict.GetValueOrDefault(id, string.Empty))),
                        CategoryName = string.Join(", ", categoryIds.Select(id => categoriesDict.GetValueOrDefault(id, string.Empty))),
                        ImageUrls = imageUrls,

                        // Shoe-specific fields
                        StockHeight = pd.StockHeight,
                        Length = pd.Length,
                        Width = pd.Width,
                        Height = pd.Height,
                        ClosureType = pd.ClosureType.ToString(),

                        // Rating breakdown
                        Rating1 = rating1,
                        Rating2 = rating2,
                        Rating3 = rating3,
                        Rating4 = rating4,
                        Rating5 = rating5
                    };
                }).ToList();

                return new PagingExtensions.PagedResult<ProductSearchResponseDTO>
                {
                    Items = items,
                    TotalRecords = totalCount,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products: {Message}", ex.Message);
                return new PagingExtensions.PagedResult<ProductSearchResponseDTO>();
            }
        }

        public async Task<ProductSearchResponseDTO?> GetProductBySkuAsync(string sku)
        {
            try
            {
                var productDetail = await _productDetailRepository.Query()
                    .Include(pd => pd.Product)
                    .Include(pd => pd.Size)
                    .Include(pd => pd.Colour)
                    .Include(pd => pd.Product.Brand)
                    .FirstOrDefaultAsync(pd => pd.Sku == sku);

                if (productDetail == null) return null;

                // Get related data
                var materialIds = await _productMaterialRepository.Query()
                    .Where(pm => pm.ProductId == productDetail.ProductId)
                    .Select(pm => pm.MaterialId)
                    .ToListAsync();

                var categoryIds = await _productCategoryRepository.Query()
                    .Where(pc => pc.ProductId == productDetail.ProductId)
                    .Select(pc => pc.CategoryId)
                    .ToListAsync();

                var seasonIds = await _productSeasonRepository.Query()
                    .Where(ps => ps.ProductId == productDetail.ProductId)
                    .Select(ps => ps.SeasonId)
                    .ToListAsync();

                var materials = await _materialRepository.Query()
                    .Where(m => materialIds.Contains(m.Id))
                    .ToListAsync();

                var categories = await _categoryRepository.Query()
                    .Where(c => categoryIds.Contains(c.Id))
                    .ToListAsync();

                var seasons = await _seasonRepository.Query()
                    .Where(s => seasonIds.Contains(s.Id))
                    .ToListAsync();

                // Get Product Images
                var imageUrls = await _imageRepository.Query()
                    .Where(i => i.ProductId == productDetail.ProductId)
                    .Select(i => i.URL)
                    .ToListAsync();

                // Calculate stock quantity
                var totalReceived = await _inventoryBatchRepository.Query()
                    .Where(ib => ib.ProductDetailId == productDetail.Id && ib.Status == EInventoryBatchStatus.Approved)
                    .SumAsync(ib => ib.Quantity);

                var totalUsed = await _inventoryTransactionRepository.Query()
                    .Where(it => _inventoryBatchRepository.Query()
                        .Any(ib => ib.Id == it.InventoryBatchId && ib.ProductDetailId == productDetail.Id && ib.Status == EInventoryBatchStatus.Approved))
                    .SumAsync(it => it.Quantity);

                var stockQuantity = totalReceived - totalUsed;

                // Get active promotion for this product detail
                var now = DateTime.Now;
                var activePromotion = await _promotionDetailRepository.Query()
                    .Include(pd => pd.Promotion)
                    .Where(pd => pd.ProductDetailId == productDetail.Id &&
                                pd.Promotion.StartDate <= now &&
                                pd.Promotion.EndDate >= now)
                    .OrderByDescending(pd => pd.Discount)
                    .FirstOrDefaultAsync();

                var originalPrice = (decimal)productDetail.Price;
                var salePrice = activePromotion != null ? originalPrice * (1 - (decimal)activePromotion.Discount / 100) : (decimal?)null;
                var maxDiscount = activePromotion?.Discount;

                // Calculate rating for this product detail
                var reviews = _reviewRepository.Query()
                    .Where(r => r.ProductDetailId == productDetail.Id && !r.IsHidden)
                    .ToList();

                decimal rating = 0;
                int rating1 = 0, rating2 = 0, rating3 = 0, rating4 = 0, rating5 = 0;

                if (reviews.Any())
                {
                    rating = (decimal)reviews.Average(r => r.Rating);
                    rating1 = reviews.Count(r => Math.Round(r.Rating) == 1);
                    rating2 = reviews.Count(r => Math.Round(r.Rating) == 2);
                    rating3 = reviews.Count(r => Math.Round(r.Rating) == 3);
                    rating4 = reviews.Count(r => Math.Round(r.Rating) == 4);
                    rating5 = reviews.Count(r => Math.Round(r.Rating) == 5);
                }

                return new ProductSearchResponseDTO
                {
                    ProductDetailId = productDetail.Id,
                    ProductId = productDetail.ProductId,
                    SKU = productDetail.Sku,
                    ProductName = productDetail.Product.Name,
                    BrandName = productDetail.Product.Brand.Name,
                    SizeValue = productDetail.Size.Value,
                    ColourName = productDetail.Colour.Name,
                    Price = originalPrice,
                    SalePrice = salePrice,
                    StockQuantity = stockQuantity,
                    OutOfStock = stockQuantity == 0 ? 1 : 0,
                    Thumbnail = productDetail.Product.Thumbnail,
                    Description = productDetail.Product.Description ?? string.Empty,
                    Rating = rating,
                    SaleNumber = productDetail.SellNumber ?? 0,
                    IsActive = productDetail.Status == EProductStatus.Selling,
                    AllowReturn = productDetail.AllowReturn,
                    MaxDiscount = maxDiscount != null ? (decimal)maxDiscount : (decimal?)null,
                    Weight = productDetail.Weight,
                    Dimensions = $"{productDetail.Length}x{productDetail.Width}x{productDetail.Height}",
                    Material = string.Join(", ", materials.Select(m => m.Name)),
                    Season = string.Join(", ", seasons.Select(s => s.Name)),
                    CategoryName = string.Join(", ", categories.Select(c => c.Name)),
                    ImageUrls = imageUrls,

                    // Shoe-specific fields
                    StockHeight = productDetail.StockHeight,
                    Length = productDetail.Length,
                    Width = productDetail.Width,
                    Height = productDetail.Height,
                    ClosureType = productDetail.ClosureType.ToString(),

                    // Rating breakdown
                    Rating1 = rating1,
                    Rating2 = rating2,
                    Rating3 = rating3,
                    Rating4 = rating4,
                    Rating5 = rating5
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product by SKU {Sku}: {Message}", sku, ex.Message);
                return null;
            }
        }





        public async Task<bool> DeleteProductDetailAsync(Guid id)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var result = await _productDetailRepository.DeleteProductDetailAsync(id);
                await _unitOfWork.CommitAsync();

                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error deleting product detail {Id}: {Message}", id, ex.Message);
                return false;
            }
        }

        public async Task<List<ProductResponseDTO>> GetRelatedProductsAsync(Guid productId, int pageSize = 4)
        {
            try
            {
                // Lấy categories của sản phẩm hiện tại
                var categoryIds = await _productCategoryRepository.Query()
                    .Where(pc => pc.ProductId == productId)
                    .Select(pc => pc.CategoryId)
                    .ToListAsync();

                List<Guid> productIds = new List<Guid>();

                if (categoryIds.Any())
                {
                    // Lấy sản phẩm từ cùng categories và có tồn kho > 0
                    productIds = await _productCategoryRepository.Query()
                        .Where(pc => categoryIds.Contains(pc.CategoryId) && pc.ProductId != productId)
                        .Join(_productDetailRepository.Query(), pc => pc.ProductId, pd => pd.ProductId, (pc, pd) => new { pc.ProductId, pd.Status })
                        .Where(x => x.Status == EProductStatus.Selling)
                        .Join(_inventoryBatchRepository.Query(), x => x.ProductId, ib => ib.ProductDetailId, (x, ib) => new { x.ProductId, ib.Status, ib.Quantity })
                        .Where(x => x.Status == EInventoryBatchStatus.Approved && x.Quantity > 0)
                        .Select(x => x.ProductId)
                        .Distinct()
                        .Take(pageSize)
                        .ToListAsync();
                }

                // Nếu không đủ, bổ sung sản phẩm ngẫu nhiên có tồn kho > 0
                if (productIds.Count < pageSize)
                {
                    var additionalIds = await _repository.Query()
                        .Where(p => p.Id != productId && !productIds.Contains(p.Id))
                        .Join(_productDetailRepository.Query(), p => p.Id, pd => pd.ProductId, (p, pd) => new { p.Id, pd.Status })
                        .Where(x => x.Status == EProductStatus.Selling)
                        .Join(_inventoryBatchRepository.Query(), x => x.Id, ib => ib.ProductDetailId, (x, ib) => new { x.Id, ib.Status, ib.Quantity })
                        .Where(x => x.Status == EInventoryBatchStatus.Approved && x.Quantity > 0)
                        .Select(x => x.Id)
                        .Distinct()
                        .OrderBy(p => Guid.NewGuid())
                        .Take(pageSize - productIds.Count)
                        .ToListAsync();

                    productIds.AddRange(additionalIds);
                }

                if (!productIds.Any())
                    return new List<ProductResponseDTO>();

                // Build main query with joins (y nguyên từ GetPagedProductsForPortalAsync)
                var productQuery = _repository.Query().Where(p => productIds.Contains(p.Id));
                var brandQuery = _brandRepository.Query();

                var mainQuery = from product in productQuery
                                join brand in brandQuery on product.BrandId equals brand.Id
                                select new
                                {
                                    product.Id,
                                    product.Name,
                                    product.BrandId,
                                    BrandName = brand.Name,
                                    product.Thumbnail,
                                    product.CreationTime,
                                    product.LastModificationTime,
                                    product.CreatedBy,
                                    product.UpdatedBy
                                };

                var mainResults = await mainQuery.ToListAsync();

                // Batch queries for related data (y nguyên từ GetPagedProductsForPortalAsync)
                var variantsDict = await _productDetailRepository.Query()
                    .Where(pd => productIds.Contains(pd.ProductId) && pd.Status == EProductStatus.Selling)
                    .Include(pd => pd.Size)
                    .Include(pd => pd.Colour)
                    .GroupBy(pd => pd.ProductId)
                    .ToDictionaryAsync(g => g.Key, g => g.ToList());

                // Compute inventory quantity per variant
                var allVariantIds = variantsDict.Values
                    .SelectMany(v => v)
                    .Select(v => v.Id)
                    .Distinct()
                    .ToList();

                var inventoryQuantityByVariantId = await _inventoryBatchRepository.Query()
                    .Where(b => allVariantIds.Contains(b.ProductDetailId) && b.Status == EInventoryBatchStatus.Approved)
                    .GroupBy(b => b.ProductDetailId)
                    .ToDictionaryAsync(g => g.Key, g => g.Sum(x => x.Quantity));

                // Get active promotions for all variants
                var now = DateTime.Now;
                var activePromotionsDict = await _promotionDetailRepository.Query()
                    .Where(pd => allVariantIds.Contains(pd.ProductDetailId) &&
                               pd.Promotion != null &&
                               pd.Promotion.StartDate <= now &&
                               pd.Promotion.EndDate >= now)
                    .Include(pd => pd.Promotion)
                    .GroupBy(pd => pd.ProductDetailId)
                    .ToDictionaryAsync(g => g.Key, g => g.OrderByDescending(pd => pd.Discount).First());

                var materialIdsDict = await _productMaterialRepository.Query()
                    .Where(pm => productIds.Contains(pm.ProductId))
                    .GroupBy(pm => pm.ProductId)
                    .ToDictionaryAsync(g => g.Key, g => g.Select(pm => pm.MaterialId).ToList());

                var categoryIdsDict = await _productCategoryRepository.Query()
                    .Where(pc => productIds.Contains(pc.ProductId))
                    .GroupBy(pc => pc.ProductId)
                    .ToDictionaryAsync(g => g.Key, g => g.Select(pc => pc.CategoryId).ToList());

                var seasonIdsDict = await _productSeasonRepository.Query()
                    .Where(ps => productIds.Contains(ps.ProductId))
                    .GroupBy(ps => ps.ProductId)
                    .ToDictionaryAsync(g => g.Key, g => g.Select(ps => ps.SeasonId).ToList());

                var imageIdsDict = await _imageRepository.Query()
                    .Where(i => productIds.Contains(i.ProductId))
                    .GroupBy(i => i.ProductId)
                    .ToDictionaryAsync(g => g.Key, g => g.ToList());

                var allMaterialIds = materialIdsDict.Values.SelectMany(x => x).Distinct().ToList();
                var allCategoryIds = categoryIdsDict.Values.SelectMany(x => x).Distinct().ToList();
                var allSeasonIds = seasonIdsDict.Values.SelectMany(x => x).Distinct().ToList();

                var materialsDict = await _materialRepository.Query()
                    .Where(m => allMaterialIds.Contains(m.Id))
                    .ToDictionaryAsync(m => m.Id, m => m.Name);

                var categoriesDict = await _categoryRepository.Query()
                    .Where(c => allCategoryIds.Contains(c.Id))
                    .ToDictionaryAsync(c => c.Id, c => c.Name);

                var seasonsDict = await _seasonRepository.Query()
                    .Where(s => allSeasonIds.Contains(s.Id))
                    .ToDictionaryAsync(s => s.Id, s => s.Name);

                // Build response (y nguyên từ GetPagedProductsForPortalAsync)
                var items = mainResults.Select(main =>
                {
                    var variants = variantsDict.TryGetValue(main.Id, out var variantList) ? variantList : new List<ProductDetail>();
                    var materialIds = materialIdsDict.TryGetValue(main.Id, out var matIds) ? matIds : new List<Guid>();
                    var categoryIds = categoryIdsDict.TryGetValue(main.Id, out var catIds) ? catIds : new List<Guid>();
                    var seasonIds = seasonIdsDict.TryGetValue(main.Id, out var seaIds) ? seaIds : new List<Guid>();
                    var images = imageIdsDict.TryGetValue(main.Id, out var imgList) ? imgList : new List<Image>();

                    return new ProductResponseDTO
                    {
                        Id = main.Id,
                        Name = main.Name,
                        BrandId = main.BrandId,
                        BrandName = main.BrandName,
                        Thumbnail = main.Thumbnail,
                        CreationTime = main.CreationTime,
                        LastModificationTime = main.LastModificationTime,
                        CreatedBy = main.CreatedBy,
                        UpdatedBy = main.UpdatedBy,

                        // Set related collections
                        SizeIds = variants.Select(v => v.SizeId).Distinct().ToList(),
                        SizeValues = variants.Select(v => v.Size?.Value ?? string.Empty).Distinct().ToList(),
                        ColourIds = variants.Select(v => v.ColourId).Distinct().ToList(),
                        ColourNames = variants.Select(v => v.Colour?.Name ?? string.Empty).Distinct().ToList(),
                        MaterialIds = materialIds,
                        MaterialNames = materialIds.Select(id => materialsDict.GetValueOrDefault(id, string.Empty)).ToList(),
                        CategoryIds = categoryIds,
                        CategoryNames = categoryIds.Select(id => categoriesDict.GetValueOrDefault(id, string.Empty)).ToList(),
                        SeasonIds = seasonIds,
                        SeasonNames = seasonIds.Select(id => seasonsDict.GetValueOrDefault(id, string.Empty)).ToList(),
                        // Set variants with inventory quantity and discount
                        ProductVariants = variants.Select(v =>
                        {
                            var dto = _mapper.Map<ProductDetailGrid>(v);
                            dto.InventoryQuantity = inventoryQuantityByVariantId.TryGetValue(v.Id, out var qty) ? qty : 0;

                            // Set discount for this variant
                            var activePromotion = activePromotionsDict.TryGetValue(v.Id, out var promo) ? promo : null;
                            dto.Discount = activePromotion?.Discount;

                            return dto;
                        }).ToList(),
                        // Calculate MaxDiscount for the product (maximum discount among all variants)
                        MaxDiscount = variants.Any() ? variants.Max(v =>
                        {
                            var activePromotion = activePromotionsDict.TryGetValue(v.Id, out var promo) ? promo : null;
                            return activePromotion?.Discount ?? 0f;
                        }) : 0f,
                        Media = _mapper.Map<List<ProductMediaUpload>>(images)
                    };
                }).ToList();

                return items;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting related products for product {ProductId}: {Message}", productId, ex.Message);
                return new List<ProductResponseDTO>();
            }
        }

        public async Task<bool> CheckVariantDependenciesAsync(Guid variantId)
        {
            try
            {
                // Kiểm tra xem biến thể có được sử dụng trong OrderDetail không
                var hasOrderDetails = await _orderDetailRepository.Query()
                    .AnyAsync(od => od.ProductDetailId == variantId);

                if (hasOrderDetails)
                    return true;

                // Kiểm tra xem biến thể có được sử dụng trong CartDetail không
                var hasCartDetails = await _cartDetailRepository.Query()
                    .AnyAsync(cd => cd.ProductDetailId == variantId);

                if (hasCartDetails)
                    return true;

                // Kiểm tra xem biến thể có được sử dụng trong PromotionDetail không
                var hasPromotionDetails = await _promotionDetailRepository.Query()
                    .AnyAsync(pd => pd.ProductDetailId == variantId);

                if (hasPromotionDetails)
                    return true;

                // Kiểm tra xem biến thể có được sử dụng trong InventoryBatch không
                var hasInventoryBatches = await _inventoryBatchRepository.Query()
                    .AnyAsync(ib => ib.ProductDetailId == variantId);

                if (hasInventoryBatches)
                    return true;


                // Nếu không có phụ thuộc nào, có thể xóa
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking variant dependencies for variant {VariantId}: {Message}", variantId, ex.Message);
                return true; // Trả về true để an toàn, không cho xóa khi có lỗi
            }
        }

    }
}