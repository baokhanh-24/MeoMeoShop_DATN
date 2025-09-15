# 📦 Cập nhật Trạng thái Đơn hàng - MeoMeo Shop

## 🎯 Tổng quan

Đã bổ sung thêm **3 trạng thái mới** cho hệ thống quản lý đơn hàng để hỗ trợ quy trình hoàn trả hàng:

### ✅ **Các trạng thái đã có:**

-   `Pending` - Chờ xác nhận
-   `Confirmed` - Đã xác nhận
-   `InTransit` - Đang vận chuyển
-   `Canceled` - Đã hủy
-   `Completed` - Hoàn thành

### 🆕 **Các trạng thái mới:**

-   `PendingReturn` - Chờ xác nhận hoàn hàng
-   `Returned` - Đã hoàn hàng
-   `RejectReturned` - Từ chối cho phép hoàn hàng

## 🔄 **Quy trình trạng thái đơn hàng:**

```
Pending → Confirmed → InTransit → Completed
   ↓           ↓           ↓
Canceled    Canceled    Canceled
   ↓
PendingReturn → Returned → Completed
   ↓
RejectReturned
```

## 📋 **Các thay đổi đã thực hiện:**

### 1. **Enum EOrderStatus** ✅

```csharp
public enum EOrderStatus
{
    [Display(Name = "Chờ xác nhận")]
    Pending,
    [Display(Name = "Đã xác nhận")]
    Confirmed,
    [Display(Name = "Đang vận chuyển")]
    InTransit,
    [Display(Name = "Đã hủy")]
    Canceled,
    [Display(Name = "Hoàn thành")]
    Completed,
    [Display(Name = "Chờ xác nhận hoàn hàng")]
    PendingReturn,
    [Display(Name = "Đã hoàn hàng")]
    Returned,
    [Display(Name = "Từ chối cho phép hoàn hàng")]
    RejectReturned,
}
```

### 2. **DTO GetListOrderResponseDTO** ✅

```csharp
public class GetListOrderResponseDTO : BaseResponse
{
    public int TotalAll { get; set; }
    public int Pending { get; set; }
    public int Confirmed { get; set; }
    public int InTransit { get; set; }
    public int Canceled { get; set; }
    public int Completed { get; set; }
    public int PendingReturn { get; set; }      // NEW
    public int Returned { get; set; }           // NEW
    public int RejectReturned { get; set; }     // NEW
}
```

### 3. **OrderService** ✅

-   Cập nhật `GetListOrderAsync()` để tính toán số lượng các trạng thái mới
-   Cập nhật `GetListOrderByCustomerAsync()` để hỗ trợ trạng thái mới

### 4. **OrderList.razor (CMS)** ✅

#### **Tabs mới:**

```html
<TabPane Key="PendingReturn" Tab=@($"Chờ hoàn hàng
({metaData.PendingReturn})")/> <TabPane Key="Returned" Tab=@($"Đã hoàn hàng
({metaData.Returned})")/> <TabPane Key="RejectReturned" Tab=@($"Từ chối hoàn
hàng ({metaData.RejectReturned})")/>
```

#### **Action Buttons mới:**

```html
<!-- Bulk Actions -->
else if (filter.OrderStatusFilter == EOrderStatus.PendingReturn) {
<button OnClick="()=>ShowPopUpConfirm(EOrderStatus.Returned)">
    <i class="fa-solid fa-check"></i> Chấp nhận hoàn trả
</button>
<button OnClick="()=>ShowPopUpConfirm(EOrderStatus.RejectReturned)">
    <i class="fa-solid fa-times"></i> Từ chối hoàn trả
</button>
} else if (filter.OrderStatusFilter == EOrderStatus.Returned) {
<button OnClick="()=>ShowPopUpConfirm(EOrderStatus.Completed)">
    <i class="fa-solid fa-check-double"></i> Hoàn tất hoàn trả
</button>
}

<!-- Individual Actions -->
else if (context.Status == EOrderStatus.PendingReturn) {
<Tooltip Title="Chấp nhận hoàn trả">
    <button OnClick="@(() => OnClickConfirm(EOrderStatus.Returned, context))">
        <i class="fa-solid fa-check text-success"></i>
    </button>
</Tooltip>
<Tooltip Title="Từ chối hoàn trả">
    <button
        OnClick="@(() => OnClickConfirm(EOrderStatus.RejectReturned, context))"
    >
        <i class="fa-solid fa-times text-danger"></i>
    </button>
</Tooltip>
} else if (context.Status == EOrderStatus.Returned) {
<Tooltip Title="Hoàn tất hoàn trả">
    <button OnClick="@(() => OnClickConfirm(EOrderStatus.Completed, context))">
        <i class="fa-solid fa-check-double text-success"></i>
    </button>
</Tooltip>
}
```

#### **Status Display:**

```csharp
string GetStatusName(EOrderStatus status)
{
    return status switch
    {
        EOrderStatus.Pending => "Chờ xác nhận",
        EOrderStatus.Confirmed => "Đã xác nhận",
        EOrderStatus.InTransit => "Đang giao",
        EOrderStatus.Canceled => "Đã huỷ",
        EOrderStatus.Completed => "Hoàn thành",
        EOrderStatus.PendingReturn => "Chờ xác nhận hoàn hàng",    // NEW
        EOrderStatus.Returned => "Đã hoàn hàng",                   // NEW
        EOrderStatus.RejectReturned => "Từ chối cho phép hoàn hàng", // NEW
        _ => status.ToString()
    };
}
```

## 🎨 **UI/UX Improvements:**

### **Icons & Colors:**

-   ✅ **Green** (`text-success`) cho các action tích cực (Chấp nhận, Hoàn thành)
-   ❌ **Red** (`text-danger`) cho các action tiêu cực (Từ chối, Hủy)
-   🔄 **Blue** (`text-primary`) cho các action trung tính (Vận chuyển)

### **Tooltips:**

-   Mỗi button đều có tooltip mô tả rõ ràng chức năng
-   Icons FontAwesome phù hợp với từng action

### **Modal Confirmations:**

-   Text confirmation được cập nhật cho các trạng thái mới
-   Logic xử lý lý do hủy đơn được giữ nguyên

## 🔧 **API Endpoints:**

Các API endpoints hiện tại đã hỗ trợ đầy đủ các trạng thái mới:

-   `GET /api/Orders/get-list-order` - Lấy danh sách đơn hàng với metadata mới
-   `PUT /api/Orders/update-status-order` - Cập nhật trạng thái đơn hàng
-   `GET /api/Orders/get-order-by-id/{id}` - Lấy chi tiết đơn hàng

## 📊 **Dashboard & Statistics:**

Các trạng thái mới sẽ được tính toán trong:

-   **Order Statistics** - Thống kê số lượng đơn hàng theo trạng thái
-   **Dashboard** - Hiển thị tổng quan các trạng thái
-   **Reports** - Báo cáo chi tiết theo trạng thái

## 🚀 **Cách sử dụng:**

### **1. Xem đơn hàng theo trạng thái:**

-   Click vào tab tương ứng (Chờ hoàn hàng, Đã hoàn hàng, Từ chối hoàn hàng)
-   Sử dụng filter dropdown để lọc theo trạng thái

### **2. Xử lý đơn hàng hoàn trả:**

-   **Chờ hoàn hàng**: Có thể chấp nhận hoặc từ chối
-   **Đã hoàn hàng**: Có thể hoàn tất quy trình
-   **Từ chối hoàn hàng**: Trạng thái cuối cùng, không thể thay đổi

### **3. Bulk Actions:**

-   Chọn nhiều đơn hàng cùng trạng thái
-   Sử dụng các button bulk action ở trên cùng

## 🔒 **Business Rules:**

1. **PendingReturn** → Chỉ có thể chuyển thành `Returned` hoặc `RejectReturned`
2. **Returned** → Chỉ có thể chuyển thành `Completed`
3. **RejectReturned** → Trạng thái cuối cùng, không thể thay đổi
4. **Completed** → Trạng thái hoàn thành, không thể thay đổi

## 🎯 **Next Steps:**

1. **Test các trạng thái mới** trong môi trường development
2. **Cập nhật documentation** cho team
3. **Training** cho staff về quy trình hoàn trả mới
4. **Monitor** performance với các trạng thái mới

---

**🎉 Hệ thống quản lý đơn hàng đã được nâng cấp hoàn chỉnh!**

Tất cả các trạng thái mới đã được tích hợp vào OrderPage với đầy đủ chức năng quản lý.
