# Màn hình Đơn hàng tại Quầy (POS - Point of Sale)

## Tổng quan

Màn hình POS được thiết kế để nhân viên bán hàng tại cửa hàng tạo đơn hàng trực tiếp cho khách hàng một cách nhanh chóng và hiệu quả.

## Tính năng chính

### 1. Tìm kiếm và thêm sản phẩm

-   **Tìm kiếm bằng từ khóa**: Nhập tên sản phẩm, SKU, hoặc barcode
-   **Dropdown kết quả**: Hiển thị danh sách sản phẩm với hình ảnh, giá, và tình trạng tồn kho
-   **Thêm sản phẩm**: Click vào sản phẩm để thêm vào đơn hàng
-   **Kiểm tra tồn kho**: Hiển thị số lượng tồn kho và cảnh báo khi hết hàng

### 2. Quét barcode

-   **Camera quét**: Sử dụng camera để quét barcode sản phẩm
-   **Tự động tìm kiếm**: Sau khi quét, tự động tìm và thêm sản phẩm vào đơn hàng
-   **Hỗ trợ mobile**: Tối ưu cho thiết bị di động với camera sau

### 3. Quản lý đơn hàng

-   **Chỉnh sửa số lượng**: Thay đổi số lượng sản phẩm trong đơn hàng
-   **Xóa sản phẩm**: Xóa sản phẩm khỏi đơn hàng
-   **Xóa hàng loạt**: Chọn và xóa nhiều sản phẩm cùng lúc
-   **Validation**: Kiểm tra số lượng tồn kho và cảnh báo khi vượt quá

### 4. Thông tin khách hàng

-   **Chọn khách hàng**: Tìm và chọn khách hàng có sẵn
-   **Thêm khách hàng mới**: Tạo khách hàng mới ngay trong quá trình bán hàng
-   **Thông tin giao hàng**: Nhập địa chỉ giao hàng cho đơn hàng online

### 5. Tính toán và thanh toán

-   **Tính toán tự động**: Tự động tính tổng tiền, phí vận chuyển, giảm giá
-   **Mã giảm giá/Voucher**: Áp dụng mã giảm giá hoặc voucher
-   **Phương thức thanh toán**: Chọn tiền mặt hoặc chuyển khoản
-   **Hiển thị chi tiết**: Hiển thị chi tiết từng khoản phí và tổng cộng

### 6. In hóa đơn

-   **In trực tiếp**: In hóa đơn ngay sau khi tạo đơn hàng
-   **Mẫu hóa đơn đẹp**: Hóa đơn có thiết kế chuyên nghiệp với logo công ty
-   **Thông tin đầy đủ**: Bao gồm thông tin đơn hàng, khách hàng, sản phẩm, và tổng tiền

## Keyboard Shortcuts

| Phím tắt   | Chức năng                     |
| ---------- | ----------------------------- |
| `F1`       | Focus vào ô tìm kiếm sản phẩm |
| `F2`       | Khởi động quét barcode        |
| `Ctrl + S` | Lưu đơn hàng                  |
| `Ctrl + P` | In hóa đơn                    |
| `Ctrl + N` | Làm mới đơn hàng              |
| `Escape`   | Đóng dropdown/modals          |

## Quy trình sử dụng

### 1. Tạo đơn hàng mới

1. Chọn loại đơn hàng (Giao trực tiếp/Giao hàng)
2. Nhập thời gian đặt hàng
3. Chọn hoặc thêm khách hàng mới

### 2. Thêm sản phẩm

1. Sử dụng ô tìm kiếm hoặc quét barcode
2. Click vào sản phẩm từ dropdown kết quả
3. Điều chỉnh số lượng nếu cần

### 3. Xử lý đơn hàng

1. Nhập thông tin giao hàng (nếu cần)
2. Áp dụng mã giảm giá/voucher
3. Chọn phương thức thanh toán
4. Kiểm tra tổng tiền

### 4. Hoàn tất

1. Click "Lưu" để tạo đơn hàng
2. Click "In đơn" để in hóa đơn
3. Click "Hủy" để làm mới và tạo đơn mới

## Tính năng nâng cao

### 1. Responsive Design

-   Tối ưu cho desktop, tablet, và mobile
-   Layout tự động điều chỉnh theo kích thước màn hình
-   Touch-friendly cho thiết bị cảm ứng

### 2. Performance

-   Tìm kiếm nhanh với debounce
-   Lazy loading cho danh sách sản phẩm
-   Caching dữ liệu khách hàng và địa chỉ

### 3. Error Handling

-   Validation đầy đủ cho tất cả input
-   Thông báo lỗi rõ ràng và hướng dẫn sửa
-   Rollback khi có lỗi xảy ra

### 4. Accessibility

-   Hỗ trợ screen reader
-   Keyboard navigation
-   High contrast mode
-   Tooltip và help text

## Cấu hình

### 1. API Endpoints

-   Customer API: Quản lý khách hàng
-   Product API: Tìm kiếm sản phẩm
-   Order API: Tạo đơn hàng
-   Voucher API: Kiểm tra mã giảm giá
-   GHN API: Lấy thông tin địa chỉ

### 2. Permissions

-   `ORDER:POS_ORDER:CREATE`: Tạo đơn hàng
-   `ORDER:POS_ORDER:PRINT`: In hóa đơn
-   `CUSTOMER:CREATE`: Tạo khách hàng mới

### 3. Settings

-   Base URL API trong `appsettings.json`
-   Printer settings cho in hóa đơn
-   Camera permissions cho quét barcode

## Troubleshooting

### 1. Camera không hoạt động

-   Kiểm tra permissions camera
-   Sử dụng HTTPS (required cho camera)
-   Thử trên trình duyệt khác

### 2. In hóa đơn không hoạt động

-   Kiểm tra popup blocker
-   Cho phép popup cho domain
-   Kiểm tra printer settings

### 3. Tìm kiếm chậm

-   Kiểm tra kết nối mạng
-   Giảm số lượng kết quả tìm kiếm
-   Kiểm tra API response time

## Future Enhancements

### 1. Tính năng sắp tới

-   Tích hợp payment gateway
-   Inventory management
-   Customer loyalty program
-   Analytics và reporting

### 2. Mobile App

-   PWA (Progressive Web App)
-   Offline mode
-   Push notifications
-   Biometric authentication

### 3. AI Features

-   Product recommendation
-   Price optimization
-   Customer behavior analysis
-   Fraud detection
