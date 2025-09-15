using MeoMeo.Domain.Commons.Enums;
using MeoMeo.Contract.DTOs.OrderDetail;
using MeoMeo.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeoMeo.Contract.BusinessRules;

public static class OrderValidator
{
    private static readonly Dictionary<EOrderStatus, List<EOrderStatus>> _allowedTransitions = new()
    {
        [EOrderStatus.Pending] = new() { EOrderStatus.Confirmed, EOrderStatus.Canceled, EOrderStatus.PendingReturn },
        [EOrderStatus.Confirmed] = new() { EOrderStatus.InTransit, EOrderStatus.Canceled, EOrderStatus.PendingReturn },
        [EOrderStatus.InTransit] = new() { EOrderStatus.Completed, EOrderStatus.Canceled, EOrderStatus.PendingReturn },
        [EOrderStatus.Completed] = new(),
        [EOrderStatus.Canceled] = new(),
        [EOrderStatus.PendingReturn] = new() { EOrderStatus.Returned, EOrderStatus.RejectReturned },
        [EOrderStatus.Returned] = new() { EOrderStatus.Completed },
        [EOrderStatus.RejectReturned] = new()
    };

    public static bool CanTransition(EOrderStatus from, EOrderStatus to) =>
        _allowedTransitions.TryGetValue(from, out var next) && next.Contains(to);

    public static string? GetErrorMessage(EOrderStatus from, EOrderStatus to)
    {
        if (from == to)
            return "Trạng thái hiện tại đã là trạng thái mong muốn.";
        if (!_allowedTransitions.ContainsKey(from))
            return "Trạng thái không hợp lệ.";
        if (!_allowedTransitions[from].Contains(to))
        {
            var fromDisplay = GetStatusDisplayName(from);
            var toDisplay = GetStatusDisplayName(to);
            return $"Không thể chuyển trạng thái từ '{fromDisplay}' sang '{toDisplay}'.";
        }
        return null;
    }

    private static string GetStatusDisplayName(EOrderStatus status)
    {
        return status switch
        {
            EOrderStatus.Pending => "Chờ xác nhận",
            EOrderStatus.Confirmed => "Đã xác nhận",
            EOrderStatus.InTransit => "Đang vận chuyển",
            EOrderStatus.Canceled => "Đã hủy",
            EOrderStatus.Completed => "Hoàn thành",
            EOrderStatus.PendingReturn => "Chờ xác nhận hoàn hàng",
            EOrderStatus.Returned => "Đã hoàn hàng",
            EOrderStatus.RejectReturned => "Từ chối cho phép hoàn hàng",
            _ => status.ToString()
        };
    }

    public static async Task<Dictionary<string, int>> ValidateInventoryAsync(
        List<OrderDetail> orderDetails,
        Func<Guid, Task<List<InventoryBatch>>> inventoryLookup,
        Func<Guid, Task<string>> skuLookup)
    {
        var insufficientStocks = new Dictionary<string, int>();

        foreach (var detail in orderDetails)
        {
            int requiredQty = detail.Quantity;
            var batches = await inventoryLookup(detail.ProductDetailId);

            int availableQty = batches
                .Where(b => b.Quantity > 0 && b.Status == EInventoryBatchStatus.Approved)
                .Sum(b => b.Quantity);

            if (availableQty < requiredQty)
            {
                var sku = await skuLookup(detail.ProductDetailId);
                if (insufficientStocks.ContainsKey(sku))
                    insufficientStocks[sku] += (requiredQty - availableQty);
                else
                    insufficientStocks[sku] = (requiredQty - availableQty);
            }
        }

        return insufficientStocks;
    }
}
