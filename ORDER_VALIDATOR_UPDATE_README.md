# 🔒 OrderValidator - Business Rules Update

## 🎯 Tổng quan

Đã cập nhật `OrderValidator.cs` để hỗ trợ đầy đủ các trạng thái đơn hàng mới và đảm bảo tính nhất quán trong quy trình chuyển đổi trạng thái.

## 🔄 **Cập nhật Allowed Transitions**

### **Trước khi cập nhật:**

```csharp
private static readonly Dictionary<EOrderStatus, List<EOrderStatus>> _allowedTransitions = new()
{
    [EOrderStatus.Pending] = new() { EOrderStatus.Confirmed, EOrderStatus.Canceled },
    [EOrderStatus.Confirmed] = new() { EOrderStatus.InTransit, EOrderStatus.Canceled },
    [EOrderStatus.InTransit] = new() { EOrderStatus.Completed, EOrderStatus.Canceled },
    [EOrderStatus.Completed] = new(),
    [EOrderStatus.Canceled] = new()
};
```

### **Sau khi cập nhật:**

```csharp
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
```

## 📋 **Quy tắc chuyển đổi trạng thái:**

### **1. Trạng thái cơ bản:**

-   **Pending** → `Confirmed`, `Canceled`, `PendingReturn`
-   **Confirmed** → `InTransit`, `Canceled`, `PendingReturn`
-   **InTransit** → `Completed`, `Canceled`, `PendingReturn`
-   **Completed** → _(không thể chuyển)_
-   **Canceled** → _(không thể chuyển)_

### **2. Trạng thái hoàn trả:**

-   **PendingReturn** → `Returned`, `RejectReturned`
-   **Returned** → `Completed`
-   **RejectReturned** → _(không thể chuyển)_

## 🔧 **Cải tiến Error Messages**

### **Trước:**

```csharp
return $"Không thể chuyển trạng thái từ {from} sang {to}.";
// Ví dụ: "Không thể chuyển trạng thái từ PendingReturn sang Completed."
```

### **Sau:**

```csharp
var fromDisplay = GetStatusDisplayName(from);
var toDisplay = GetStatusDisplayName(to);
return $"Không thể chuyển trạng thái từ '{fromDisplay}' sang '{toDisplay}'.";
// Ví dụ: "Không thể chuyển trạng thái từ 'Chờ xác nhận hoàn hàng' sang 'Hoàn thành'."
```

## 🎨 **Status Display Names**

Thêm hàm `GetStatusDisplayName()` để hiển thị tên trạng thái bằng tiếng Việt:

```csharp
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
```

## 🔒 **Business Rules Validation**

### **1. CanTransition() Method:**

```csharp
public static bool CanTransition(EOrderStatus from, EOrderStatus to)
```

-   Kiểm tra xem có thể chuyển từ trạng thái `from` sang `to` không
-   Trả về `true` nếu hợp lệ, `false` nếu không hợp lệ

### **2. GetErrorMessage() Method:**

```csharp
public static string? GetErrorMessage(EOrderStatus from, EOrderStatus to)
```

-   Trả về thông báo lỗi chi tiết nếu không thể chuyển đổi
-   Trả về `null` nếu chuyển đổi hợp lệ
-   Hiển thị tên trạng thái bằng tiếng Việt

## 🚀 **Cách sử dụng:**

### **1. Kiểm tra chuyển đổi hợp lệ:**

```csharp
if (OrderValidator.CanTransition(EOrderStatus.Pending, EOrderStatus.Confirmed))
{
    // Có thể chuyển từ Pending sang Confirmed
}
```

### **2. Lấy thông báo lỗi:**

```csharp
var errorMessage = OrderValidator.GetErrorMessage(EOrderStatus.Pending, EOrderStatus.Completed);
if (errorMessage != null)
{
    // Hiển thị lỗi: "Không thể chuyển trạng thái từ 'Chờ xác nhận' sang 'Hoàn thành'."
}
```

### **3. Validation trong OrderService:**

```csharp
public async Task<BaseResponse> UpdateStatusOrderAsync(UpdateStatusOrderRequestDTO request)
{
    foreach (var orderId in request.OrderIds)
    {
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        if (!OrderValidator.CanTransition(order.Status, request.Status))
        {
            var errorMessage = OrderValidator.GetErrorMessage(order.Status, request.Status);
            return new BaseResponse
            {
                ResponseStatus = BaseStatus.Error,
                Message = errorMessage
            };
        }
    }
    // Tiếp tục xử lý...
}
```

## 📊 **Flow Diagram:**

```
Pending ──┬── Confirmed ──┬── InTransit ──┬── Completed
          │               │               │
          ├── Canceled    ├── Canceled    ├── Canceled
          │               │               │
          └── PendingReturn ──┬── Returned ── Completed
                              │
                              └── RejectReturned
```

## 🔍 **Test Cases:**

### **Valid Transitions:**

-   ✅ `Pending` → `Confirmed`
-   ✅ `Pending` → `Canceled`
-   ✅ `Pending` → `PendingReturn`
-   ✅ `Confirmed` → `InTransit`
-   ✅ `InTransit` → `Completed`
-   ✅ `PendingReturn` → `Returned`
-   ✅ `PendingReturn` → `RejectReturned`
-   ✅ `Returned` → `Completed`

### **Invalid Transitions:**

-   ❌ `Pending` → `Completed` (phải qua Confirmed trước)
-   ❌ `Completed` → `Canceled` (không thể thay đổi)
-   ❌ `Canceled` → `Confirmed` (không thể thay đổi)
-   ❌ `PendingReturn` → `Completed` (phải qua Returned trước)
-   ❌ `RejectReturned` → `Returned` (không thể thay đổi)

## 🎯 **Benefits:**

1. **Consistency**: Đảm bảo tính nhất quán trong quy trình chuyển đổi trạng thái
2. **User Experience**: Thông báo lỗi rõ ràng bằng tiếng Việt
3. **Maintainability**: Dễ dàng thêm/sửa quy tắc business
4. **Validation**: Ngăn chặn các chuyển đổi trạng thái không hợp lệ
5. **Documentation**: Code tự document với tên trạng thái rõ ràng

---

**🔒 OrderValidator đã được cập nhật hoàn chỉnh!**

Tất cả các quy tắc business cho việc chuyển đổi trạng thái đơn hàng đã được implement đầy đủ.
