# 📊 Excel Report System - MeoMeo Shop

## 🎯 Tổng quan

Hệ thống báo cáo Excel đã được tích hợp hoàn chỉnh vào MeoMeo Shop, cho phép tạo và gửi báo cáo chi tiết dưới dạng file Excel đính kèm email.

## 🔧 **Các thành phần chính:**

### **1. ExcelReportService**

-   ✅ **Generate Daily Report**: Tạo báo cáo hàng ngày với nhiều sheet
-   ✅ **Generate Weekly Report**: Tạo báo cáo hàng tuần với phân tích chi tiết
-   ✅ **Professional Formatting**: Định dạng Excel chuyên nghiệp với màu sắc và style
-   ✅ **Multiple Sheets**: Tổng quan, Đơn hàng, Khách hàng, Sản phẩm bán chạy

### **2. ReportService (Updated)**

-   ✅ **Excel Integration**: Sử dụng ExcelReportService thay vì text report
-   ✅ **Email with Attachment**: Gửi file Excel đính kèm qua email
-   ✅ **Error Handling**: Xử lý lỗi tốt với logging chi tiết

### **3. EmailService (Enhanced)**

-   ✅ **Attachment Support**: Hỗ trợ gửi file đính kèm
-   ✅ **Excel MIME Type**: Định dạng MIME đúng cho file Excel
-   ✅ **Professional Template**: Template email đẹp với thông tin attachment

## 📋 **Cấu trúc Excel Report:**

### **Daily Report:**

```
📊 BaoCaoHangNgay_YYYYMMDD.xlsx
├── 📈 Tổng quan
│   ├── Thống kê đơn hàng (hôm nay vs hôm qua)
│   ├── Thống kê khách hàng mới
│   ├── Thống kê sản phẩm
│   └── Phân tích tăng trưởng
├── 🛍️ Đơn hàng
│   ├── Mã đơn hàng
│   ├── Khách hàng
│   ├── Ngày tạo
│   ├── Trạng thái
│   └── Tổng tiền
└── 👥 Khách hàng
    ├── Mã khách hàng
    ├── Tên khách hàng
    ├── Email
    ├── Số điện thoại
    └── Ngày tạo
```

### **Weekly Report:**

```
📊 BaoCaoHangTuan_YYYYMMDD.xlsx
├── 📈 Tổng quan
│   ├── Thống kê đơn hàng (tuần này vs tuần trước)
│   ├── Thống kê khách hàng mới
│   ├── Thống kê nhân viên
│   └── Phân tích tăng trưởng
├── 🛍️ Đơn hàng
│   └── (Chi tiết đơn hàng trong tuần)
├── 👥 Khách hàng
│   └── (Khách hàng mới trong tuần)
└── 🏆 Sản phẩm bán chạy
    ├── STT
    ├── Tên sản phẩm
    ├── Số lượng bán
    └── Doanh thu
```

## 🎨 **Excel Formatting:**

### **1. Header Styling:**

```csharp
// Header với màu xanh và font trắng
headerRange.Style.Font.Bold = true;
headerRange.Style.Fill.PatternType = ExcelFillPatternType.Solid;
headerRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));
headerRange.Style.Font.Color.SetColor(Color.White);
```

### **2. Number Formatting:**

```csharp
// Format số tiền với dấu phẩy
sheet.Cells[row, 5].Style.Numberformat.Format = "#,##0";
```

### **3. Color Coding:**

```csharp
// Màu xanh cho tăng trưởng dương, đỏ cho âm
sheet.Cells[row, 2].Style.Font.Color.SetColor(growth >= 0 ? Color.Green : Color.Red);
```

### **4. Auto-fit Columns:**

```csharp
// Tự động điều chỉnh độ rộng cột
sheet.Cells.AutoFitColumns();
```

## 📧 **Email Template:**

### **HTML Email với Attachment Info:**

```html
<div class="attachment-info">
    <h3>📎 File đính kèm</h3>
    <p>
        Báo cáo chi tiết đã được đính kèm trong file Excel. Vui lòng mở file để
        xem thông tin đầy đủ.
    </p>
</div>
```

### **Professional Styling:**

-   ✅ **Gradient Header**: Header với gradient đẹp
-   ✅ **Attachment Info Box**: Box thông báo file đính kèm
-   ✅ **Responsive Design**: Tương thích với mọi email client
-   ✅ **Brand Colors**: Sử dụng màu sắc thương hiệu

## 🔧 **Technical Implementation:**

### **1. EPPlus Integration:**

```csharp
// Set license context
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// Create Excel package
using var package = new ExcelPackage();
var sheet = package.Workbook.Worksheets.Add("Sheet Name");

// Return as byte array
return package.GetAsByteArray();
```

### **2. Email with Attachment:**

```csharp
// Create multipart message
var multipart = new Multipart("mixed");

// Add HTML body
var htmlBody = new TextPart("html") { Text = htmlContent };
multipart.Add(htmlBody);

// Add Excel attachment
var attachment = new MimePart("application", "vnd.openxmlformats-officedocument.spreadsheetml.sheet")
{
    Content = new MimeContent(new MemoryStream(excelData)),
    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
    ContentTransferEncoding = ContentEncoding.Base64,
    FileName = fileName
};
multipart.Add(attachment);
```

### **3. Data Processing:**

```csharp
// Get orders with date range
var todayOrders = await _orderRepository.Query()
    .Where(o => o.CreationTime >= today && o.CreationTime < today.AddDays(1))
    .ToListAsync();

// Calculate statistics
var orderGrowth = todayOrders.Count - yesterdayOrders.Count;
var revenueGrowth = todayOrders.Sum(o => o.TotalAmount) - yesterdayOrders.Sum(o => o.TotalAmount);
```

## 📊 **Report Features:**

### **1. Daily Report Features:**

-   ✅ **Order Comparison**: So sánh đơn hàng hôm nay vs hôm qua
-   ✅ **Revenue Analysis**: Phân tích doanh thu với format số
-   ✅ **Customer Growth**: Thống kê khách hàng mới
-   ✅ **Product Count**: Tổng số sản phẩm trong hệ thống
-   ✅ **Growth Indicators**: Chỉ số tăng trưởng với màu sắc

### **2. Weekly Report Features:**

-   ✅ **Weekly Comparison**: So sánh tuần này vs tuần trước
-   ✅ **Average Orders**: Đơn hàng trung bình/ngày
-   ✅ **Employee Statistics**: Thống kê nhân viên
-   ✅ **Top Products**: Top 10 sản phẩm bán chạy
-   ✅ **Revenue Analysis**: Phân tích doanh thu chi tiết

### **3. Data Visualization:**

-   ✅ **Color Coding**: Màu xanh/đỏ cho tăng trưởng
-   ✅ **Professional Headers**: Header đẹp với màu thương hiệu
-   ✅ **Number Formatting**: Format số tiền với dấu phẩy
-   ✅ **Auto-fit Layout**: Tự động điều chỉnh layout

## 🚀 **API Endpoints:**

### **Reports Controller:**

```csharp
[HttpPost("send-daily-report")]
public async Task<IActionResult> SendDailyReport([FromBody] string adminEmail)

[HttpPost("send-weekly-report")]
public async Task<IActionResult> SendWeeklyReport([FromBody] string adminEmail)
```

### **Usage:**

```bash
# Send daily report
POST /api/Reports/send-daily-report
Content-Type: application/json
"admin@meomeoshop.com"

# Send weekly report
POST /api/Reports/send-weekly-report
Content-Type: application/json
"admin@meomeoshop.com"
```

## 🔒 **Security & Performance:**

### **1. Memory Management:**

-   ✅ **Using Statements**: Proper disposal of ExcelPackage
-   ✅ **Stream Handling**: Efficient memory usage với MemoryStream
-   ✅ **Async Operations**: Non-blocking operations

### **2. Error Handling:**

-   ✅ **Try-Catch Blocks**: Comprehensive error handling
-   ✅ **Logging**: Detailed logging cho debugging
-   ✅ **Graceful Failures**: Fallback khi có lỗi

### **3. Data Validation:**

-   ✅ **Null Checks**: Kiểm tra null trước khi xử lý
-   ✅ **Date Range Validation**: Validate date range hợp lệ
-   ✅ **Email Validation**: Validate email format

## 📱 **Email Client Compatibility:**

### **Supported Clients:**

-   ✅ **Gmail**: Full support với HTML và attachment
-   ✅ **Outlook**: Compatible với MIME format
-   ✅ **Apple Mail**: Native support
-   ✅ **Thunderbird**: Full compatibility
-   ✅ **Mobile Clients**: iOS Mail, Android Gmail

### **Attachment Handling:**

-   ✅ **MIME Type**: Correct Excel MIME type
-   ✅ **Base64 Encoding**: Proper encoding
-   ✅ **File Size**: Optimized file size
-   ✅ **Download**: Easy download và open

## 🎯 **Benefits:**

### **1. Professional Reports:**

-   ✅ **Excel Format**: Dễ đọc và phân tích
-   ✅ **Multiple Sheets**: Tổ chức dữ liệu tốt
-   ✅ **Professional Styling**: Giao diện đẹp
-   ✅ **Data Visualization**: Màu sắc và formatting

### **2. Email Integration:**

-   ✅ **Automatic Delivery**: Tự động gửi email
-   ✅ **Attachment Support**: File đính kèm an toàn
-   ✅ **Professional Template**: Email template đẹp
-   ✅ **Mobile Friendly**: Tương thích mobile

### **3. Business Intelligence:**

-   ✅ **Trend Analysis**: Phân tích xu hướng
-   ✅ **Growth Metrics**: Chỉ số tăng trưởng
-   ✅ **Performance Tracking**: Theo dõi hiệu suất
-   ✅ **Data Export**: Xuất dữ liệu dễ dàng

## 🔄 **Workflow:**

### **Daily Report Workflow:**

```
1. Cron Job triggers → 2. Generate Excel → 3. Send Email → 4. Admin receives report
```

### **Weekly Report Workflow:**

```
1. Weekly Schedule → 2. Generate Excel → 3. Send Email → 4. Admin receives report
```

## 📈 **Future Enhancements:**

### **Planned Features:**

-   🔄 **Custom Date Range**: Báo cáo theo khoảng thời gian tùy chỉnh
-   🔄 **Chart Integration**: Thêm biểu đồ vào Excel
-   🔄 **PDF Export**: Xuất báo cáo PDF
-   🔄 **Scheduled Reports**: Lên lịch báo cáo tự động
-   🔄 **Dashboard Integration**: Tích hợp với dashboard

---

**📊 Excel Report System đã hoàn thành!**

Hệ thống báo cáo Excel chuyên nghiệp với email đính kèm, cung cấp thông tin chi tiết và dễ phân tích cho quản lý.
