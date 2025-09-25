using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using MeoMeo.Shared.IServices;

namespace MeoMeo.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly EmailSettings _emailSettings;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _emailSettings = _configuration.GetSection("EmailSettings").Get<EmailSettings>() ?? new EmailSettings();
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                if (isHtml)
                {
                    message.Body = new TextPart("html") { Text = body };
                }
                else
                {
                    message.Body = new TextPart("plain") { Text = body };
                }

                using var client = new SmtpClient();
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

                if (!_emailSettings.UseDefaultCredentials)
                {
                    await client.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                }

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Email sent successfully to {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {toEmail}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendEmployeeNotificationAsync(string employeeEmail, string employeeName, string action)
        {
            var subject = $"Thông báo {action} nhân viên - MeoMeo Shop";
            var body = GetEmployeeNotificationTemplate(employeeName, action);
            return await SendEmailAsync(employeeEmail, subject, body);
        }

        public async Task<bool> SendPasswordResetEmailAsync(string userEmail, string resetToken, string userName)
        {
            var subject = "Đặt lại mật khẩu - MeoMeo Shop";
            var resetLink = $"{_emailSettings.BaseUrl}/reset-password?token={resetToken}";
            var body = GetPasswordResetTemplate(userName, resetLink);
            return await SendEmailAsync(userEmail, subject, body);
        }

        public async Task<bool> SendNewPasswordEmailAsync(string userEmail, string newPassword, string userName)
        {
            var subject = "Mật khẩu mới - MeoMeo Shop";
            var body = GetNewPasswordTemplate(userName, newPassword);
            return await SendEmailAsync(userEmail, subject, body);
        }

        public async Task<bool> SendOrderConfirmationEmailAsync(string customerEmail, string customerName, string orderNumber, decimal totalAmount)
        {
            var subject = $"Xác nhận đơn hàng #{orderNumber} - MeoMeo Shop";
            var body = GetOrderConfirmationTemplate(customerName, orderNumber, totalAmount);
            return await SendEmailAsync(customerEmail, subject, body);
        }

        public async Task<bool> SendDailyReportEmailAsync(string adminEmail, string reportData)
        {
            var subject = $"Báo cáo hàng ngày - {DateTime.Now:dd/MM/yyyy} - MeoMeo Shop";
            var body = GetDailyReportTemplate(reportData);
            return await SendEmailAsync(adminEmail, subject, body);
        }

        public async Task<bool> SendWeeklyReportEmailAsync(string adminEmail, string reportData)
        {
            var subject = $"Báo cáo hàng tuần - Tuần {GetWeekOfYear(DateTime.Now)} - MeoMeo Shop";
            var body = GetWeeklyReportTemplate(reportData);
            return await SendEmailAsync(adminEmail, subject, body);
        }

        private string GetEmployeeNotificationTemplate(string employeeName, string action)
        {
            var actionText = action switch
            {
                "tạo mới" => "được tạo mới",
                "cập nhật" => "được cập nhật",
                "xóa" => "bị xóa",
                _ => action
            };

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Thông báo nhân viên</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .highlight {{ color: #667eea; font-weight: bold; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🏢 MeoMeo Shop</h1>
            <p>Hệ thống quản lý nhân viên</p>
        </div>
        <div class='content'>
            <h2>Thông báo quan trọng</h2>
            <p>Xin chào,</p>
            <p>Thông tin nhân viên <span class='highlight'>{employeeName}</span> đã {actionText} trong hệ thống.</p>
            <p>Vui lòng kiểm tra thông tin chi tiết trong hệ thống CMS.</p>
            <p>Trân trọng,<br>Đội ngũ MeoMeo Shop</p>
        </div>
        <div class='footer'>
            <p>Email này được gửi tự động từ hệ thống MeoMeo Shop</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetPasswordResetTemplate(string userName, string resetLink)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Đặt lại mật khẩu</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .button {{ background: #667eea; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; display: inline-block; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
        .warning {{ background: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 5px; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🔐 MeoMeo Shop</h1>
            <p>Đặt lại mật khẩu</p>
        </div>
        <div class='content'>
            <h2>Xin chào {userName}!</h2>
            <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.</p>
            <p>Nhấn vào nút bên dưới để đặt lại mật khẩu:</p>
            <a href='{resetLink}' class='button'>Đặt lại mật khẩu</a>
            <div class='warning'>
                <strong>⚠️ Lưu ý:</strong>
                <ul>
                    <li>Link này chỉ có hiệu lực trong 24 giờ</li>
                    <li>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này</li>
                    <li>Để bảo mật, không chia sẻ link này với ai khác</li>
                </ul>
            </div>
            <p>Trân trọng,<br>Đội ngũ MeoMeo Shop</p>
        </div>
        <div class='footer'>
            <p>Email này được gửi tự động từ hệ thống MeoMeo Shop</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetNewPasswordTemplate(string userName, string newPassword)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Mật khẩu mới</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .password-box {{ background: white; padding: 20px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #667eea; text-align: center; }}
        .password-text {{ font-size: 24px; font-weight: bold; color: #667eea; letter-spacing: 2px; font-family: monospace; }}
        .highlight {{ color: #667eea; font-weight: bold; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
        .warning {{ background: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 5px; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🔐 MeoMeo Shop</h1>
            <p>Mật khẩu mới</p>
        </div>
        <div class='content'>
            <h2>Xin chào {userName}!</h2>
            <p>Chúng tôi đã tạo mật khẩu mới cho tài khoản của bạn.</p>
            
            <div class='password-box'>
                <h3>Mật khẩu mới của bạn:</h3>
                <div class='password-text'>{newPassword}</div>
            </div>
            
            <div class='warning'>
                <strong>⚠️ Lưu ý quan trọng:</strong>
                <ul>
                    <li>Hãy đổi mật khẩu ngay sau khi đăng nhập</li>
                    <li>Không chia sẻ mật khẩu với ai khác</li>
                    <li>Để bảo mật, hãy sử dụng mật khẩu mạnh</li>
                    <li>Lưu mật khẩu ở nơi an toàn</li>
                </ul>
            </div>
            
            <p>Trân trọng,<br>Đội ngũ MeoMeo Shop</p>
        </div>
        <div class='footer'>
            <p>Email này được gửi tự động từ hệ thống MeoMeo Shop</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetOrderConfirmationTemplate(string customerName, string orderNumber, decimal totalAmount)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Xác nhận đơn hàng</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .order-info {{ background: white; padding: 20px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #667eea; }}
        .highlight {{ color: #667eea; font-weight: bold; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🛍️ MeoMeo Shop</h1>
            <p>Xác nhận đơn hàng</p>
        </div>
        <div class='content'>
            <h2>Cảm ơn bạn đã mua hàng!</h2>
            <p>Xin chào <span class='highlight'>{customerName}</span>,</p>
            <p>Chúng tôi đã nhận được đơn hàng của bạn và đang xử lý.</p>
            
            <div class='order-info'>
                <h3>Thông tin đơn hàng:</h3>
                <p><strong>Mã đơn hàng:</strong> <span class='highlight'>#{orderNumber}</span></p>
                <p><strong>Tổng tiền:</strong> <span class='highlight'>{totalAmount:N0} VNĐ</span></p>
                <p><strong>Ngày đặt:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>
            </div>

            <p>Chúng tôi sẽ gửi thông tin vận chuyển đến bạn trong thời gian sớm nhất.</p>
            <p>Trân trọng,<br>Đội ngũ MeoMeo Shop</p>
        </div>
        <div class='footer'>
            <p>Email này được gửi tự động từ hệ thống MeoMeo Shop</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetDailyReportTemplate(string reportData)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Báo cáo hàng ngày</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 800px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .report-section {{ background: white; padding: 20px; border-radius: 5px; margin: 20px 0; }}
        .highlight {{ color: #667eea; font-weight: bold; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>📊 MeoMeo Shop</h1>
            <p>Báo cáo hàng ngày - {DateTime.Now:dd/MM/yyyy}</p>
        </div>
        <div class='content'>
            <h2>Tổng quan hoạt động ngày</h2>
            <div class='report-section'>
                <h3>📈 Thống kê chính</h3>
                <pre style='white-space: pre-wrap; font-family: monospace;'>{reportData}</pre>
            </div>
            <p>Trân trọng,<br>Hệ thống báo cáo MeoMeo Shop</p>
        </div>
        <div class='footer'>
            <p>Email này được gửi tự động từ hệ thống MeoMeo Shop</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetWeeklyReportTemplate(string reportData)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Báo cáo hàng tuần</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 800px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .report-section {{ background: white; padding: 20px; border-radius: 5px; margin: 20px 0; }}
        .highlight {{ color: #667eea; font-weight: bold; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>📊 MeoMeo Shop</h1>
            <p>Báo cáo hàng tuần - Tuần {GetWeekOfYear(DateTime.Now)}</p>
        </div>
        <div class='content'>
            <h2>Tổng quan hoạt động tuần</h2>
            <div class='report-section'>
                <h3>📈 Thống kê tổng hợp</h3>
                <pre style='white-space: pre-wrap; font-family: monospace;'>{reportData}</pre>
            </div>
            <p>Trân trọng,<br>Hệ thống báo cáo MeoMeo Shop</p>
        </div>
        <div class='footer'>
            <p>Email này được gửi tự động từ hệ thống MeoMeo Shop</p>
        </div>
    </div>
</body>
</html>";
        }

        public async Task<bool> SendReportEmailWithAttachmentAsync(string adminEmail, string subject, string body, byte[] attachmentData, string attachmentFileName)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
                message.To.Add(new MailboxAddress("", adminEmail));
                message.Subject = subject;

                // Create multipart message
                var multipart = new Multipart("mixed");

                // Add HTML body
                var htmlBody = new TextPart("html") { Text = GetReportEmailTemplate(body) };
                multipart.Add(htmlBody);

                // Add Excel attachment
                var attachment = new MimePart("application", "vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    Content = new MimeContent(new MemoryStream(attachmentData)),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = attachmentFileName
                };
                multipart.Add(attachment);

                message.Body = multipart;

                using var client = new SmtpClient();
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

                if (!_emailSettings.UseDefaultCredentials)
                {
                    await client.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                }

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Report email with attachment sent successfully to {adminEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send report email with attachment to {adminEmail}: {ex.Message}");
                return false;
            }
        }

        private string GetReportEmailTemplate(string reportContent)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Báo cáo hệ thống</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 800px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .report-section {{ background: white; padding: 20px; border-radius: 5px; margin: 20px 0; }}
        .highlight {{ color: #667eea; font-weight: bold; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
        .attachment-info {{ background: #e8f4fd; padding: 15px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #667eea; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>📊 MeoMeo Shop</h1>
            <p>Báo cáo hệ thống</p>
        </div>
        <div class='content'>
            <div class='attachment-info'>
                <h3>📎 File đính kèm</h3>
                <p>Báo cáo chi tiết đã được đính kèm trong file Excel. Vui lòng mở file để xem thông tin đầy đủ.</p>
            </div>
            
            <div class='report-section'>
                <h3>📈 Tóm tắt</h3>
                <p>{reportContent}</p>
            </div>
            
            <p>Trân trọng,<br>Hệ thống báo cáo MeoMeo Shop</p>
        </div>
        <div class='footer'>
            <p>Email này được gửi tự động từ hệ thống MeoMeo Shop</p>
        </div>
    </div>
</body>
</html>";
        }
        private int GetWeekOfYear(DateTime date)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            return culture.Calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }
    }

    public class EmailSettings
    {
        public string SmtpServer { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587;
        public string SmtpUsername { get; set; } = "";
        public string SmtpPassword { get; set; } = "";
        public string FromEmail { get; set; } = "";
        public string FromName { get; set; } = "";
        public bool EnableSsl { get; set; } = true;
        public bool UseDefaultCredentials { get; set; } = false;
        public string BaseUrl { get; set; } = "http://localhost:3000";
    }
}
