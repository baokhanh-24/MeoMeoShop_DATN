# 📧 Email System Integration - MeoMeo Shop

## 🎯 Tổng quan

Hệ thống email đã được tích hợp hoàn chỉnh vào MeoMeo Shop với các tính năng:

-   ✅ **Email thông báo nhân viên**: Tự động gửi email khi tạo/cập nhật employee
-   ✅ **Email reset password**: Gửi link đặt lại mật khẩu cho user
-   ✅ **Email xác nhận đơn hàng**: Gửi email xác nhận khi khách hàng đặt hàng
-   ✅ **Email báo cáo**: Gửi báo cáo hàng ngày/tuần cho admin

## 🔧 Cấu hình SMTP

### 1. Cập nhật appsettings.json

```json
{
    "EmailSettings": {
        "SmtpServer": "smtp.gmail.com",
        "SmtpPort": 587,
        "SmtpUsername": "your-email@gmail.com",
        "SmtpPassword": "your-app-password",
        "FromEmail": "noreply@meomeoshop.com",
        "FromName": "MeoMeo Shop",
        "EnableSsl": true,
        "UseDefaultCredentials": false,
        "BaseUrl": "http://localhost:3000"
    }
}
```

### 2. Cấu hình Gmail (Khuyến nghị)

1. **Bật 2-Factor Authentication** cho Gmail account
2. **Tạo App Password**:
    - Vào Google Account Settings
    - Security → 2-Step Verification → App passwords
    - Tạo password mới cho "Mail"
3. **Sử dụng App Password** thay vì password thường

### 3. Các SMTP Provider khác

#### Outlook/Hotmail

```json
{
    "SmtpServer": "smtp-mail.outlook.com",
    "SmtpPort": 587,
    "EnableSsl": true
}
```

#### Yahoo Mail

```json
{
    "SmtpServer": "smtp.mail.yahoo.com",
    "SmtpPort": 587,
    "EnableSsl": true
}
```

## 📋 API Endpoints

### Password Reset

-   `POST /api/PasswordReset/request-reset` - Yêu cầu reset password
-   `POST /api/PasswordReset/reset` - Đặt lại password với token
-   `POST /api/PasswordReset/validate-token` - Kiểm tra token hợp lệ

### Reports

-   `POST /api/Reports/send-daily-report` - Gửi báo cáo hàng ngày
-   `POST /api/Reports/send-weekly-report` - Gửi báo cáo hàng tuần

## 🎨 Email Templates

### 1. Employee Notification

-   **Trigger**: Khi tạo/cập nhật employee
-   **Template**: HTML với gradient header, thông tin employee
-   **Features**: Responsive design, branding MeoMeo Shop

### 2. Password Reset

-   **Trigger**: Khi user yêu cầu reset password
-   **Template**: HTML với button reset, cảnh báo bảo mật
-   **Features**: Token expiry 24h, security warnings

### 3. Order Confirmation

-   **Trigger**: Khi khách hàng đặt hàng thành công
-   **Template**: HTML với thông tin đơn hàng, tổng tiền
-   **Features**: Order details, delivery info, branding

### 4. Daily/Weekly Reports

-   **Trigger**: Manual hoặc scheduled
-   **Template**: HTML với thống kê chi tiết
-   **Features**: Charts, comparisons, top products

## 🔄 Tích hợp tự động

### Employee Management

```csharp
// Tự động gửi email khi tạo employee
await _emailService.SendEmployeeNotificationAsync(
    employee.Email,
    employee.Name,
    "tạo mới"
);

// Tự động gửi email khi cập nhật employee
await _emailService.SendEmployeeNotificationAsync(
    employee.Email,
    employee.Name,
    "cập nhật"
);
```

### Order Processing

```csharp
// Tự động gửi email xác nhận đơn hàng
await _emailService.SendOrderConfirmationEmailAsync(
    order.CustomerEmail,
    order.CustomerName,
    order.Code,
    order.TotalPrice
);
```

## 🛠️ Services Architecture

### Core Services

-   `IEmailService` - Core email functionality
-   `IPasswordResetService` - Password reset logic
-   `IReportService` - Report generation and sending

### Dependencies

-   `MailKit` - SMTP client
-   `MimeKit` - Email message creation
-   `Microsoft.Extensions.Logging` - Error logging

## 🚀 Usage Examples

### 1. Gửi email đơn giản

```csharp
await _emailService.SendEmailAsync(
    "user@example.com",
    "Subject",
    "<h1>Hello World!</h1>",
    true // isHtml
);
```

### 2. Request Password Reset

```csharp
var result = await _passwordResetService.RequestPasswordResetAsync("user@example.com");
```

### 3. Send Daily Report

```csharp
var result = await _reportService.SendDailyReportAsync("admin@meomeoshop.com");
```

## 🔒 Security Features

-   **Token-based reset**: Secure random tokens
-   **Token expiry**: 24-hour expiration
-   **One-time use**: Tokens invalidated after use
-   **Email validation**: Proper email format checking
-   **Error handling**: Graceful failure without breaking main flow

## 📊 Monitoring & Logging

-   **Success logging**: Email sent successfully
-   **Error logging**: Failed email attempts with details
-   **Performance tracking**: Email send duration
-   **Retry mechanism**: Automatic retry on failure

## 🎯 Next Steps

1. **Cấu hình SMTP settings** trong appsettings.json
2. **Test email functionality** với các endpoints
3. **Setup scheduled reports** (có thể dùng Hangfire)
4. **Monitor email delivery** và adjust templates
5. **Add email analytics** tracking

## 🆘 Troubleshooting

### Common Issues

1. **Authentication failed**: Kiểm tra username/password
2. **SSL errors**: Đảm bảo EnableSsl = true
3. **Port blocked**: Thử port 465 hoặc 25
4. **Rate limiting**: Gmail có giới hạn 100 emails/day

### Debug Mode

```csharp
// Enable detailed logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug);
});
```

---

**📧 Email System đã sẵn sàng sử dụng!**

Hãy cấu hình SMTP settings và test các tính năng email.
