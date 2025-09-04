# GHN Integration for MeoMeo Shop

## Tổng quan

Dự án đã được tích hợp với API Giao Hàng Nhanh (GHN) để cung cấp các tính năng:

-   Tính toán phí vận chuyển tự động
-   Tạo đơn hàng vận chuyển GHN
-   Quản lý địa chỉ giao hàng với dữ liệu GHN

## Tính năng đã triển khai

### 1. Tooltip/Dialog phí vận chuyển

-   Hiển thị chi tiết cách tính phí vận chuyển
-   Thông tin về trọng lượng, thể tích, kích thước
-   Giải thích các bước tính toán
-   Có thể mở bằng cách click vào icon info bên cạnh phí vận chuyển

### 2. Tích hợp API tạo đơn hàng GHN

-   Tự động tạo đơn hàng GHN khi checkout
-   Sử dụng thông tin thực tế từ giỏ hàng (trọng lượng, kích thước)
-   Hỗ trợ cả thanh toán trước và thanh toán khi nhận hàng (COD)

### 3. Service GHN

-   `IGhnService` - Interface cho các thao tác GHN
-   `GhnService` - Implementation của service
-   Các phương thức:
    -   `CreateOrderAsync` - Tạo đơn hàng GHN
    -   `CalculateShippingFeeAsync` - Tính phí vận chuyển
    -   `GetProvincesAsync` - Lấy danh sách tỉnh/thành
    -   `GetDistrictsAsync` - Lấy danh sách quận/huyện
    -   `GetWardsAsync` - Lấy danh sách phường/xã
    -   `GetAvailableServicesAsync` - Lấy danh sách dịch vụ vận chuyển

## Cấu hình

### File appsettings.json

```json
{
    "Ghn": {
        "BaseUrl": "https://dev-online-gateway.ghn.vn/shiip/public-api",
        "Token": "your-ghn-token",
        "ShopId": 197393,
        "FromDistrictId": 3440,
        "FromWardCode": "13009",
        "DefaultItemWeightGram": 500,
        "DefaultLengthCm": 15,
        "DefaultWidthCm": 15,
        "DefaultHeightCm": 15
    }
}
```

### Các tham số cấu hình:

-   `BaseUrl`: URL API GHN (sử dụng dev cho test, production cho live)
-   `Token`: Token xác thực GHN
-   `ShopId`: ID shop trên GHN
-   `FromDistrictId`: ID quận/huyện của shop
-   `FromWardCode`: Mã phường/xã của shop
-   `DefaultItemWeightGram`: Trọng lượng mặc định (gram)
-   `DefaultLengthCm`, `DefaultWidthCm`, `DefaultHeightCm`: Kích thước mặc định (cm)

## Cách sử dụng

### 1. Tính phí vận chuyển

Phí vận chuyển được tính tự động dựa trên:

-   Trọng lượng thực tế của sản phẩm trong giỏ hàng
-   Kích thước thực tế của sản phẩm
-   Địa chỉ giao hàng (tỉnh/thành, quận/huyện, phường/xã)

### 2. Xem chi tiết phí vận chuyển

-   Click vào icon info (ℹ️) bên cạnh dòng "Phí vận chuyển (GHN)"
-   Modal sẽ hiển thị:
    -   Tổng trọng lượng
    -   Tổng thể tích
    -   Kích thước lớn nhất
    -   Cách tính toán
    -   Lưu ý

### 3. Tạo đơn hàng GHN

-   Khi checkout, hệ thống sẽ tự động:
    -   Tạo đơn hàng trong hệ thống nội bộ
    -   Tạo đơn hàng vận chuyển GHN
    -   Hiển thị mã đơn hàng GHN nếu thành công

## DTOs và Models

### GhnCreateOrderRequestDTO

Chứa thông tin để tạo đơn hàng GHN:

-   Thông tin người gửi (shop)
-   Thông tin người nhận (khách hàng)
-   Thông tin hàng hóa (trọng lượng, kích thước, giá trị)
-   Dịch vụ vận chuyển

### GhnCreateOrderResponseDTO

Phản hồi từ API GHN:

-   Mã đơn hàng GHN
-   Thời gian dự kiến giao hàng
-   Chi tiết phí vận chuyển
-   Mã vận chuyển

## Lưu ý quan trọng

### 1. Môi trường

-   **Development**: Sử dụng `https://dev-online-gateway.ghn.vn/shiip/public-api`
-   **Production**: Sử dụng `https://online-gateway.ghn.vn/shiip/public-api`

### 2. Xử lý lỗi

-   Nếu không thể tạo đơn hàng GHN, đơn hàng nội bộ vẫn được tạo
-   Hiển thị cảnh báo cho người dùng
-   Log lỗi để debug

### 3. Hiệu suất

-   API calls được thực hiện bất đồng bộ
-   Sử dụng service pattern để tái sử dụng code
-   Cache dữ liệu địa chỉ khi có thể

## Troubleshooting

### Lỗi thường gặp:

1. **Token không hợp lệ**: Kiểm tra cấu hình GHN Token
2. **Shop ID không đúng**: Xác nhận Shop ID trên GHN
3. **Địa chỉ không hợp lệ**: Kiểm tra thông tin tỉnh/quận/phường
4. **API rate limit**: GHN có giới hạn số lượng request

### Debug:

-   Kiểm tra console logs
-   Xác nhận cấu hình trong appsettings
-   Test API GHN trực tiếp

## Tài liệu tham khảo

-   [GHN API Documentation](https://api.ghn.vn/home/docs/detail?id=123)
-   [GHN Create Order API](https://api.ghn.vn/home/docs/detail?id=123)
-   [GHN Master Data API](https://api.ghn.vn/home/docs/detail?id=92)
