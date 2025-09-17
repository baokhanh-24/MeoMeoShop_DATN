using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons.Enums;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeoMeo.Contract.Commons;

namespace MeoMeo.Application.Services
{
    public class InventoryStatisticsService : IInventoryStatisticsService
    {
        private readonly IProductsDetailRepository _productDetailRepository;
        private readonly IProductRepository _productRepository;
        private readonly IColourRepository _colourRepository;
        private readonly ISizeRepository _sizeRepository;
        private readonly IIventoryBatchReposiory _inventoryBatchRepository;
        private readonly IInventoryTranSactionRepository _inventoryTransactionRepository;

        public InventoryStatisticsService(
            IProductsDetailRepository productDetailRepository,
            IProductRepository productRepository,
            IColourRepository colourRepository,
            ISizeRepository sizeRepository,
            IIventoryBatchReposiory inventoryBatchRepository,
            IInventoryTranSactionRepository inventoryTransactionRepository)
        {
            _productDetailRepository = productDetailRepository;
            _productRepository = productRepository;
            _colourRepository = colourRepository;
            _sizeRepository = sizeRepository;
            _inventoryBatchRepository = inventoryBatchRepository;
            _inventoryTransactionRepository = inventoryTransactionRepository;
        }

        public async Task<GetInventoryStatisticsResponseDTO> GetInventoryStatisticsAsync(GetInventoryStatisticsRequestDTO request)
        {
            try
            {
                // Lấy tất cả product details với thông tin liên quan (Product, Colour, Size)
                var query = from pd in _productDetailRepository.Query()
                            join p in _productRepository.Query() on pd.ProductId equals p.Id
                            join c in _colourRepository.Query() on pd.ColourId equals c.Id
                            join s in _sizeRepository.Query() on pd.SizeId equals s.Id
                            where pd.Status == EProductStatus.Selling // Chỉ lấy sản phẩm đang bán
                            select new
                            {
                                pd.Id,
                                pd.Sku,
                                p.Name,
                                p.Thumbnail,
                                ColourName = c.Name,
                                SizeValue = s.Value,
                                pd.OutOfStock // Ngưỡng cảnh báo hết hàng
                            };

                // Áp dụng bộ lọc theo tên sản phẩm
                if (!string.IsNullOrEmpty(request.NameFilter))
                {
                    query = query.Where(x => x.Name.Contains(request.NameFilter));
                }

                // Áp dụng bộ lọc theo SKU
                if (!string.IsNullOrEmpty(request.SKUFilter))
                {
                    query = query.Where(x => x.Sku.Contains(request.SKUFilter));
                }

                var totalRecords = await query.CountAsync();

                // Lấy dữ liệu theo phân trang
                var productDetails = await query
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                var result = new List<InventoryStatisticsItemDTO>();

                foreach (var pd in productDetails)
                {
                    // Tính tồn kho hiện tại từ các batch đã được duyệt
                    var currentStock = await _inventoryBatchRepository.Query()
                        .Where(ib => ib.ProductDetailId == pd.Id && ib.Status == EInventoryBatchStatus.Approved)
                        .SumAsync(ib => ib.Quantity);

                    // Lấy thời gian cập nhật cuối từ giao dịch tồn kho
                    var lastUpdated = await _inventoryTransactionRepository.Query()
                        .Where(it => it.InventoryBatch.ProductDetailId == pd.Id)
                        .OrderByDescending(it => it.CreationTime)
                        .Select(it => it.CreationTime)
                        .FirstOrDefaultAsync();

                    // Xác định trạng thái tồn kho
                    string stockStatus;
                    if (currentStock <= 0)
                        stockStatus = "OutOfStock"; // Hết hàng
                    else if (currentStock <= pd.OutOfStock)
                        stockStatus = "LowStock"; // Sắp hết hàng
                    else
                        stockStatus = "InStock"; // Còn hàng

                    // Áp dụng bộ lọc theo trạng thái tồn kho
                    if (!string.IsNullOrEmpty(request.StockStatusFilter) && stockStatus != request.StockStatusFilter)
                        continue;

                    result.Add(new InventoryStatisticsItemDTO
                    {
                        ProductDetailId = pd.Id,
                        SKU = pd.Sku,
                        ProductName = pd.Name,
                        ColourName = pd.ColourName,
                        SizeValue = pd.SizeValue,
                        ImageUrl = pd.Thumbnail ?? "",
                        CurrentStock = currentStock,
                        OutOfStockThreshold = pd.OutOfStock,
                        StockStatus = stockStatus,
                        LastUpdated = lastUpdated
                    });
                }

                // Tính toán thống kê tổng quan
                var allProductDetails = await _productDetailRepository.Query()
                    .Where(pd => pd.Status == EProductStatus.Selling)
                    .ToListAsync();

                var summary = new InventoryStatisticsSummaryDTO
                {
                    TotalProducts = allProductDetails.Count
                };

                // Đếm số lượng sản phẩm theo từng trạng thái tồn kho
                foreach (var pd in allProductDetails)
                {
                    var stock = await _inventoryBatchRepository.Query()
                        .Where(ib => ib.ProductDetailId == pd.Id && ib.Status == EInventoryBatchStatus.Approved)
                        .SumAsync(ib => ib.Quantity);

                    if (stock <= 0)
                        summary.ProductsOutOfStock++; // Sản phẩm hết hàng
                    else if (stock <= pd.OutOfStock)
                        summary.ProductsLowStock++; // Sản phẩm sắp hết hàng
                    else
                        summary.ProductsInStock++; // Sản phẩm còn hàng
                }

                return new GetInventoryStatisticsResponseDTO
                {
                    Items = result,
                    Summary = summary,
                    TotalRecords = totalRecords,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    ResponseStatus = BaseStatus.Success,
                    Message = "Lấy thống kê tồn kho thành công"
                };
            }
            catch (Exception ex)
            {
                return new GetInventoryStatisticsResponseDTO
                {
                    Items = new List<InventoryStatisticsItemDTO>(),
                    Summary = new InventoryStatisticsSummaryDTO(),
                    TotalRecords = 0,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    ResponseStatus = BaseStatus.Error,
                    Message = ex.Message
                };
            }
        }

        public async Task<GetInventoryHistoryResponseDTO> GetInventoryHistoryAsync(GetInventoryHistoryRequestDTO request)
        {
            try
            {
                // Lấy danh sách giao dịch tồn kho với thông tin InventoryBatch
                var query = _inventoryTransactionRepository.Query()
                    .Include(it => it.InventoryBatch) // Include để tránh N+1 query
                    .Where(it => it.InventoryBatch.ProductDetailId == request.ProductDetailId)
                    .OrderByDescending(it => it.CreationTime); // Sắp xếp mới nhất trước

                var totalRecords = await query.CountAsync();

                // Lấy dữ liệu theo phân trang (theo thứ tự mới nhất trước)
                var pageTransactionsDesc = await query
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                var result = new List<InventoryHistoryItemDTO>();

                if (pageTransactionsDesc.Count == 0)
                {
                    return new GetInventoryHistoryResponseDTO
                    {
                        Items = result,
                        TotalRecords = totalRecords,
                        PageIndex = request.PageIndex,
                        PageSize = request.PageSize
                    };
                }

                // Thời điểm cũ nhất trong trang hiện tại
                var oldestOnPageTime = pageTransactionsDesc.Last().CreationTime;

                // Tồn kho sau tất cả giao dịch trước thời điểm cũ nhất của trang hiện tại
                // Đây là điểm bắt đầu để cộng dồn đúng theo chiều thời gian (cũ -> mới)
                var startingStock = await _inventoryTransactionRepository.Query()
                    .Where(it => it.InventoryBatch.ProductDetailId == request.ProductDetailId
                                 && it.CreationTime < oldestOnPageTime)
                    .Select(it => it.Type == EInventoryTranctionType.Import ? it.Quantity : -it.Quantity)
                    .SumAsync();

                // Cộng dồn theo thứ tự thời gian (cũ -> mới) trong phạm vi của trang
                var runningStock = startingStock;
                foreach (var transaction in pageTransactionsDesc.OrderBy(t => t.CreationTime))
                {
                    var delta = transaction.Type == EInventoryTranctionType.Import ? transaction.Quantity : -transaction.Quantity;
                    runningStock += delta;

                    result.Add(new InventoryHistoryItemDTO
                    {
                        Date = transaction.CreationTime,
                        TransactionType = transaction.Type,
                        QuantityChange = delta,
                        StockAfter = runningStock,
                        Note = transaction.Note ?? string.Empty
                    });
                }

                // Hiển thị giao dịch mới nhất trước
                result = result.OrderByDescending(r => r.Date).ToList();

                return new GetInventoryHistoryResponseDTO
                {
                    Items = result,
                    TotalRecords = totalRecords,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize
                };
            }
            catch (Exception ex)
            {
                return new GetInventoryHistoryResponseDTO()
                {
                    Items = new List<InventoryHistoryItemDTO>(),
                    TotalRecords = 0,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    ResponseStatus = BaseStatus.Error,
                    Message = ex.Message
                };

            }
        }
    }
}
