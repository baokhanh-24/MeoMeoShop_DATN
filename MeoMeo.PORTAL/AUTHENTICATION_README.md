# Hệ thống Authentication - MeoMeo Portal

## Tổng quan

Hệ thống authentication đã được tích hợp hoàn chỉnh vào dự án Portal với các tính năng:

-   ✅ Đăng nhập/Đăng xuất với JWT Token
-   ✅ Refresh Token tự động
-   ✅ Bảo vệ routes với Authorization
-   ✅ User Menu với thông tin người dùng
-   ✅ Giao diện đẹp và responsive
-   ✅ Tự động thêm JWT token vào HTTP requests

## Cấu trúc Files

### Services

-   `IServices/IAuthService.cs` - Interface cho authentication service
-   `Services/AuthService.cs` - Implementation của authentication service
-   `Services/CustomAuthStateProvider.cs` - Authentication state provider
-   `Services/AuthenticationHttpMessageHandler.cs` - HTTP handler tự động thêm token

### Pages

-   `Components/Pages/Login.razor` - Trang đăng nhập
-   `Components/Pages/Register.razor` - Trang đăng ký

### Components

-   `Components/Layout/UserMenu.razor` - Menu người dùng với dropdown

## Cách sử dụng

### 1. Đăng nhập

-   Truy cập `/login` để đăng nhập
-   Nhập username và password
-   Hệ thống sẽ tự động lưu JWT token vào localStorage

### 2. Bảo vệ Routes

```csharp
@page "/protected-page"
@attribute [Authorize]

<h3>Trang được bảo vệ</h3>
<p>Chỉ người dùng đã đăng nhập mới thấy được nội dung này.</p>
```

### 3. Hiển thị nội dung theo trạng thái đăng nhập

```csharp
<AuthorizeView>
    <Authorized>
        <p>Xin chào @context.User.Identity?.Name!</p>
    </Authorized>
    <NotAuthorized>
        <p>Vui lòng <a href="/login">đăng nhập</a> để tiếp tục.</p>
    </NotAuthorized>
</AuthorizeView>
```

### 4. Lấy thông tin user từ claims

```csharp
@inject AuthenticationStateProvider AuthStateProvider

@code {
    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            var userName = user.FindFirst("UserName")?.Value;
            var fullName = user.FindFirst("FullName")?.Value;
            var email = user.FindFirst("Email")?.Value;
        }
    }
}
```

### 5. Đăng xuất

```csharp
@inject IAuthService AuthService
@inject CustomAuthStateProvider AuthStateProvider

private async Task HandleLogout()
{
    var refreshToken = await AuthService.GetRefreshTokenAsync();
    if (!string.IsNullOrEmpty(refreshToken))
    {
        var logoutRequest = new RefreshTokenRequest { RefreshToken = refreshToken };
        await AuthService.LogoutAsync(logoutRequest);
    }

    AuthStateProvider.NotifyUserLogout();
    NavigationManager.NavigateTo("/");
}
```

## JWT Claims

Token chứa các claims sau:

-   `UserName` - Tên đăng nhập
-   `UserId` - ID của user
-   `AdminOrUser` - Phân loại Admin/User
-   `Roles` - Danh sách roles (phân cách bằng ";")
-   `Email` - Email user
-   `Avatar` - Avatar user
-   `FullName` - Tên đầy đủ
-   `Permissions` - Danh sách permissions (phân cách bằng ";")

## Cấu hình

### 1. API Base URL

Cập nhật trong `appsettings.json`:

```json
{
    "ApiSettings": {
        "BaseUrl": "http://localhost:5092"
    }
}
```

### 2. Authentication Endpoints

-   Login: `POST /api/auths/connect-token`
-   Logout: `POST /api/auths/logout`
-   Refresh Token: `POST /api/auths/refresh-token`

## Tính năng nâng cao

### 1. Token Refresh Tự động

-   HTTP Handler tự động thêm JWT token vào requests
-   Tự động refresh token khi nhận được 401 Unauthorized
-   Tự động logout khi refresh token thất bại

### 2. Persistent Login

-   Token được lưu trong localStorage
-   Tự động đăng nhập khi refresh trang
-   Token có thời hạn 2 năm

### 3. Security Features

-   Password hashing với SHA256
-   Token revocation
-   Account locking support
-   Role-based access control

## Troubleshooting

### 1. Token không được thêm vào request

-   Kiểm tra AuthenticationHttpMessageHandler đã được đăng ký
-   Kiểm tra token có trong localStorage không

### 2. 401 Unauthorized liên tục

-   Kiểm tra refresh token có hợp lệ không
-   Kiểm tra API endpoint có đúng không
-   Kiểm tra JWT configuration trên server

### 3. UserMenu không hiển thị

-   Kiểm tra CascadingAuthenticationState đã được wrap
-   Kiểm tra CustomAuthStateProvider đã được đăng ký

## Tương lai

### Các tính năng có thể thêm:

-   [ ] Social login (Google, Facebook)
-   [ ] Two-factor authentication
-   [ ] Password reset
-   [ ] Email verification
-   [ ] Remember me functionality
-   [ ] Session management
-   [ ] Audit logging

## Liên hệ

Nếu có vấn đề hoặc cần hỗ trợ, vui lòng liên hệ team development.
