# MeoMeo Shop CMS - Hướng dẫn đăng nhập

## Tổng quan

Hệ thống CMS (Content Management System) của MeoMeo Shop cho phép Admin và Employee quản lý cửa hàng trực tuyến.

## Thông tin đăng nhập mặc định

### 👑 Admin (Quản trị viên)

-   **Tài khoản:** `admin@meomeo.com`
-   **Mật khẩu:** `Admin@12345`
-   **Quyền hạn:** Toàn quyền quản lý hệ thống

### 👤 Employee (Nhân viên)

-   **Tài khoản:** `employee@meomeo.com`
-   **Mật khẩu:** `Employee@12345`
-   **Quyền hạn:** Quản lý đơn hàng, sản phẩm, khách hàng

## Cách đăng nhập

1. Truy cập vào trang CMS: `https://your-domain/cms/login`
2. Nhập thông tin đăng nhập từ bảng trên
3. Nhấn nút "Đăng nhập CMS"
4. Hệ thống sẽ kiểm tra quyền và chuyển hướng đến Dashboard

## Tính năng chính

### Dashboard

-   Thống kê doanh thu theo thời gian
-   Top sản phẩm bán chạy
-   Thống kê đơn hàng và tồn kho
-   Biểu đồ trực quan

### Quản lý sản phẩm

-   Thêm/sửa/xóa sản phẩm
-   Quản lý chi tiết sản phẩm (size, màu sắc, chất liệu)
-   Quản lý tồn kho

### Quản lý đơn hàng

-   Xem danh sách đơn hàng
-   Xử lý đơn hàng tại quầy (POS)
-   Theo dõi trạng thái đơn hàng

### Quản lý khách hàng

-   Xem thông tin khách hàng
-   Quản lý địa chỉ giao hàng

### Quản lý nhân viên

-   Thêm/sửa/xóa nhân viên
-   Phân quyền cho nhân viên
-   Đổi mật khẩu

## Bảo mật

⚠️ **Lưu ý quan trọng:**

-   Đây là tài khoản mặc định để demo
-   Trong môi trường production, **BẮT BUỘC** phải thay đổi mật khẩu
-   Không sử dụng mật khẩu mặc định cho môi trường thực tế

## Tạo tài khoản mới

### Tạo Admin mới

```bash
POST /api/admin/create-admin
{
  "email": "newadmin@meomeo.com",
  "password": "NewPassword@123",
  "name": "Tên Admin",
  "phone": "0123456789",
  "address": "Địa chỉ"
}
```

### Tạo Employee mới

```bash
POST /api/employees/create-employee
{
  "email": "newemployee@meomeo.com",
  "password": "NewPassword@123",
  "name": "Tên Employee",
  "phone": "0987654321",
  "address": "Địa chỉ"
}
```

## Khởi tạo dữ liệu mặc định

Để tạo các tài khoản mặc định, chạy migration:

```bash
dotnet ef database update --project MeoMeo.EntityFrameworkCore --startup-project MeoMeoAPI
```

Hoặc gọi API để tạo:

```bash
POST /api/admin/setup-default-users
```

## Xử lý sự cố

### Không thể đăng nhập

1. Kiểm tra thông tin đăng nhập
2. Đảm bảo API đang chạy
3. Kiểm tra kết nối database

### Quên mật khẩu

1. Liên hệ Admin để reset mật khẩu
2. Hoặc sử dụng API reset password

### Lỗi phân quyền

1. Kiểm tra role của user trong database
2. Đảm bảo user có role "Admin" hoặc "Employee"

## Liên hệ hỗ trợ

Nếu gặp vấn đề, vui lòng liên hệ:

-   Email: support@meomeo.com
-   Hotline: 0123-456-789

---

**MeoMeo Shop CMS v1.0**  
© 2024 MeoMeo Shop. All rights reserved.
