namespace MeoMeo.Shared.IServices
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
        Task<bool> SendEmployeeNotificationAsync(string employeeEmail, string employeeName, string action);
        Task<bool> SendPasswordResetEmailAsync(string userEmail, string resetToken, string userName);
        Task<bool> SendNewPasswordEmailAsync(string userEmail, string newPassword, string userName);
        Task<bool> SendOrderConfirmationEmailAsync(string customerEmail, string customerName, string orderNumber, decimal totalAmount);
        Task<bool> SendDailyReportEmailAsync(string adminEmail, string reportData);
        Task<bool> SendWeeklyReportEmailAsync(string adminEmail, string reportData);
        Task<bool> SendReportEmailWithAttachmentAsync(string adminEmail, string subject, string body, byte[] attachmentData, string attachmentFileName);
    }
}
