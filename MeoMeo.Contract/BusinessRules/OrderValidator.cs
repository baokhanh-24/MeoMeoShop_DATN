using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.BusinessRules;

public static class OrderValidator
{
    private static readonly Dictionary<EOrderStatus, List<EOrderStatus>> _allowedTransitions = new()
    {
        [EOrderStatus.Pending] = new() { EOrderStatus.Confirmed, EOrderStatus.Canceled },
        [EOrderStatus.Confirmed] = new() { EOrderStatus.InTransit, EOrderStatus.Canceled },
        [EOrderStatus.InTransit] = new() { EOrderStatus.Completed, EOrderStatus.Canceled },
        [EOrderStatus.Completed] = new(),
        [EOrderStatus.Canceled] = new()
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
            return $"Không thể chuyển trạng thái từ {from} sang {to}.";
        return null;
    }
}