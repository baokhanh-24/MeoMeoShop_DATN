using MeoMeo.Contract.Commons;
using MeoMeo.Domain.IRepositories;
using MeoMeo.Shared.IServices;
using Microsoft.Extensions.Logging;
using System.Text;
using MeoMeo.Application.IServices;

namespace MeoMeo.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IProductRepository _productRepository;
        private readonly IEmailService _emailService;
        private readonly IExcelReportService _excelReportService;
        private readonly ILogger<ReportService> _logger;

        public ReportService(
            IOrderRepository orderRepository,
            ICustomerRepository customerRepository,
            IEmployeeRepository employeeRepository,
            IProductRepository productRepository,
            IEmailService emailService,
            IExcelReportService excelReportService,
            ILogger<ReportService> logger)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _employeeRepository = employeeRepository;
            _productRepository = productRepository;
            _emailService = emailService;
            _excelReportService = excelReportService;
            _logger = logger;
        }

        public async Task<BaseResponse> SendDailyReportAsync(string adminEmail)
        {
            try
            {
                // Generate Excel report
                var excelData = await _excelReportService.GenerateDailyReportExcelAsync();
                var fileName = $"BaoCaoHangNgay_{DateTime.Today:yyyyMMdd}.xlsx";

                // Send email with Excel attachment
                var emailSent = await _emailService.SendReportEmailWithAttachmentAsync(
                    adminEmail,
                    "Báo cáo hàng ngày",
                    "Đây là báo cáo hàng ngày từ hệ thống MeoMeo Shop.",
                    excelData,
                    fileName);

                if (!emailSent)
                {
                    return new BaseResponse
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Không thể gửi email báo cáo hàng ngày"
                    };
                }

                return new BaseResponse
                {
                    ResponseStatus = BaseStatus.Success,
                    Message = "Email báo cáo hàng ngày đã được gửi thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending daily report");
                return new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Có lỗi xảy ra khi gửi báo cáo hàng ngày"
                };
            }
        }

        public async Task<BaseResponse> SendWeeklyReportAsync(string adminEmail)
        {
            try
            {
                // Generate Excel report
                var excelData = await _excelReportService.GenerateWeeklyReportExcelAsync();
                var fileName = $"BaoCaoHangTuan_{DateTime.Today:yyyyMMdd}.xlsx";

                // Send email with Excel attachment
                var emailSent = await _emailService.SendReportEmailWithAttachmentAsync(
                    adminEmail,
                    "Báo cáo hàng tuần",
                    "Đây là báo cáo hàng tuần từ hệ thống MeoMeo Shop.",
                    excelData,
                    fileName);

                if (!emailSent)
                {
                    return new BaseResponse
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Không thể gửi email báo cáo hàng tuần"
                    };
                }

                return new BaseResponse
                {
                    ResponseStatus = BaseStatus.Success,
                    Message = "Email báo cáo hàng tuần đã được gửi thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending weekly report");
                return new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Có lỗi xảy ra khi gửi báo cáo hàng tuần"
                };
            }
        }

    }
}
