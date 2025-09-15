using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Domain.IRepositories;
using MeoMeo.Domain.Commons.Enums;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Microsoft.EntityFrameworkCore;

namespace MeoMeo.Application.Services
{
    public class ExcelReportService : IExcelReportService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly ILogger<ExcelReportService> _logger;

        public ExcelReportService(
            IOrderRepository orderRepository,
            ICustomerRepository customerRepository,
            IEmployeeRepository employeeRepository,
            IProductRepository productRepository,
            IOrderDetailRepository orderDetailRepository,
            ILogger<ExcelReportService> logger)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _employeeRepository = employeeRepository;
            _productRepository = productRepository;
            _orderDetailRepository = orderDetailRepository;
            _logger = logger;

            // Set EPPlus license context for EPPlus 8+
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<byte[]> GenerateDailyReportExcelAsync()
        {
            try
            {
                var today = DateTime.Today;
                var yesterday = today.AddDays(-1);

                // Get data
                var todayOrders = await _orderRepository.Query()
                    .Where(o => o.CreationTime >= today && o.CreationTime < today.AddDays(1))
                    .ToListAsync();

                var yesterdayOrders = await _orderRepository.Query()
                    .Where(o => o.CreationTime >= yesterday && o.CreationTime < today)
                    .ToListAsync();

                var newCustomersToday = await _customerRepository.Query()
                    .Where(c => c.CreationTime >= today && c.CreationTime < today.AddDays(1))
                    .ToListAsync();

                var totalProducts = await _productRepository.Query().CountAsync();

                // Create Excel package
                using var package = new ExcelPackage();

                // Summary Sheet
                var summarySheet = package.Workbook.Worksheets.Add("Tổng quan");
                CreateSummarySheet(summarySheet, today, todayOrders, yesterdayOrders, newCustomersToday, totalProducts);

                // Orders Sheet
                var ordersSheet = package.Workbook.Worksheets.Add("Đơn hàng");
                CreateOrdersSheet(ordersSheet, todayOrders);

                // Customers Sheet
                var customersSheet = package.Workbook.Worksheets.Add("Khách hàng");
                CreateCustomersSheet(customersSheet, newCustomersToday);

                return package.GetAsByteArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating daily report Excel");
                throw;
            }
        }

        public async Task<byte[]> GenerateWeeklyReportExcelAsync()
        {
            try
            {
                var endDate = DateTime.Today;
                var startDate = endDate.AddDays(-7);

                // Get data
                var weekOrders = await _orderRepository.Query()
                    .Where(o => o.CreationTime >= startDate && o.CreationTime < endDate.AddDays(1))
                    .ToListAsync();

                var lastWeekOrders = await _orderRepository.Query()
                    .Where(o => o.CreationTime >= startDate.AddDays(-7) && o.CreationTime < startDate)
                    .ToListAsync();

                var newCustomersThisWeek = await _customerRepository.Query()
                    .Where(c => c.CreationTime >= startDate && c.CreationTime < endDate.AddDays(1))
                    .ToListAsync();

                var totalEmployees = await _employeeRepository.Query().CountAsync();

                // Get top products
                var topProducts = await GetTopProductsAsync(startDate, endDate.AddDays(1));

                // Create Excel package
                using var package = new ExcelPackage();

                // Summary Sheet
                var summarySheet = package.Workbook.Worksheets.Add("Tổng quan");
                CreateWeeklySummarySheet(summarySheet, startDate, endDate, weekOrders, lastWeekOrders, newCustomersThisWeek, totalEmployees);

                // Orders Sheet
                var ordersSheet = package.Workbook.Worksheets.Add("Đơn hàng");
                CreateOrdersSheet(ordersSheet, weekOrders);

                // Customers Sheet
                var customersSheet = package.Workbook.Worksheets.Add("Khách hàng");
                CreateCustomersSheet(customersSheet, newCustomersThisWeek);

                // Top Products Sheet
                var topProductsSheet = package.Workbook.Worksheets.Add("Sản phẩm bán chạy");
                CreateTopProductsSheet(topProductsSheet, topProducts);

                return package.GetAsByteArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating weekly report Excel");
                throw;
            }
        }

        private void CreateSummarySheet(ExcelWorksheet sheet, DateTime date, List<MeoMeo.Domain.Entities.Order> todayOrders, List<MeoMeo.Domain.Entities.Order> yesterdayOrders, List<MeoMeo.Domain.Entities.Customers> newCustomers, int totalProducts)
        {
            // Header
            sheet.Cells[1, 1].Value = "BÁO CÁO HÀNG NGÀY";
            sheet.Cells[1, 1, 1, 4].Merge = true;
            sheet.Cells[1, 1].Style.Font.Size = 16;
            sheet.Cells[1, 1].Style.Font.Bold = true;
            sheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));

            sheet.Cells[2, 1].Value = $"Ngày: {date:dd/MM/yyyy}";
            sheet.Cells[2, 1, 2, 4].Merge = true;
            sheet.Cells[2, 1].Style.Font.Size = 12;
            sheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Order Statistics
            int row = 4;
            sheet.Cells[row, 1].Value = "THỐNG KÊ ĐƠN HÀNG";
            sheet.Cells[row, 1].Style.Font.Bold = true;
            sheet.Cells[row, 1].Style.Font.Size = 14;

            row++;
            sheet.Cells[row, 1].Value = "Đơn hàng hôm nay:";
            sheet.Cells[row, 2].Value = todayOrders.Count;
            sheet.Cells[row, 1].Style.Font.Bold = true;

            row++;
            sheet.Cells[row, 1].Value = "Đơn hàng hôm qua:";
            sheet.Cells[row, 2].Value = yesterdayOrders.Count;
            sheet.Cells[row, 1].Style.Font.Bold = true;

            row++;
            sheet.Cells[row, 1].Value = "Tổng giá trị hôm nay:";
            sheet.Cells[row, 2].Value = todayOrders.Sum(o => o.TotalPrice);
            sheet.Cells[row, 2].Style.Numberformat.Format = "#,##0";
            sheet.Cells[row, 1].Style.Font.Bold = true;

            row++;
            sheet.Cells[row, 1].Value = "Tổng giá trị hôm qua:";
            sheet.Cells[row, 2].Value = yesterdayOrders.Sum(o => o.TotalPrice);
            sheet.Cells[row, 2].Style.Numberformat.Format = "#,##0";
            sheet.Cells[row, 1].Style.Font.Bold = true;

            // Customer Statistics
            row += 2;
            sheet.Cells[row, 1].Value = "THỐNG KÊ KHÁCH HÀNG";
            sheet.Cells[row, 1].Style.Font.Bold = true;
            sheet.Cells[row, 1].Style.Font.Size = 14;

            row++;
            sheet.Cells[row, 1].Value = "Khách hàng mới hôm nay:";
            sheet.Cells[row, 2].Value = newCustomers.Count;
            sheet.Cells[row, 1].Style.Font.Bold = true;

            // Product Statistics
            row += 2;
            sheet.Cells[row, 1].Value = "THỐNG KÊ SẢN PHẨM";
            sheet.Cells[row, 1].Style.Font.Bold = true;
            sheet.Cells[row, 1].Style.Font.Size = 14;

            row++;
            sheet.Cells[row, 1].Value = "Tổng số sản phẩm:";
            sheet.Cells[row, 2].Value = totalProducts;
            sheet.Cells[row, 1].Style.Font.Bold = true;

            // Growth Analysis
            row += 2;
            sheet.Cells[row, 1].Value = "PHÂN TÍCH TĂNG TRƯỞNG";
            sheet.Cells[row, 1].Style.Font.Bold = true;
            sheet.Cells[row, 1].Style.Font.Size = 14;

            var orderGrowth = todayOrders.Count - yesterdayOrders.Count;
            var revenueGrowth = todayOrders.Sum(o => o.TotalPrice) - yesterdayOrders.Sum(o => o.TotalPrice);

            row++;
            sheet.Cells[row, 1].Value = "Tăng trưởng đơn hàng:";
            sheet.Cells[row, 2].Value = orderGrowth;
            sheet.Cells[row, 2].Style.Font.Color.SetColor(orderGrowth >= 0 ? Color.Green : Color.Red);
            sheet.Cells[row, 1].Style.Font.Bold = true;

            row++;
            sheet.Cells[row, 1].Value = "Tăng trưởng doanh thu:";
            sheet.Cells[row, 2].Value = revenueGrowth;
            sheet.Cells[row, 2].Style.Numberformat.Format = "#,##0";
            sheet.Cells[row, 2].Style.Font.Color.SetColor(revenueGrowth >= 0 ? Color.Green : Color.Red);
            sheet.Cells[row, 1].Style.Font.Bold = true;

            // Auto-fit columns
            sheet.Cells.AutoFitColumns();
        }

        private void CreateWeeklySummarySheet(ExcelWorksheet sheet, DateTime startDate, DateTime endDate, List<MeoMeo.Domain.Entities.Order> weekOrders, List<MeoMeo.Domain.Entities.Order> lastWeekOrders, List<MeoMeo.Domain.Entities.Customers> newCustomers, int totalEmployees)
        {
            // Header
            sheet.Cells[1, 1].Value = "BÁO CÁO HÀNG TUẦN";
            sheet.Cells[1, 1, 1, 4].Merge = true;
            sheet.Cells[1, 1].Style.Font.Size = 16;
            sheet.Cells[1, 1].Style.Font.Bold = true;
            sheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));

            sheet.Cells[2, 1].Value = $"Tuần: {startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}";
            sheet.Cells[2, 1, 2, 4].Merge = true;
            sheet.Cells[2, 1].Style.Font.Size = 12;
            sheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Order Statistics
            int row = 4;
            sheet.Cells[row, 1].Value = "THỐNG KÊ ĐƠN HÀNG";
            sheet.Cells[row, 1].Style.Font.Bold = true;
            sheet.Cells[row, 1].Style.Font.Size = 14;

            row++;
            sheet.Cells[row, 1].Value = "Đơn hàng tuần này:";
            sheet.Cells[row, 2].Value = weekOrders.Count;
            sheet.Cells[row, 1].Style.Font.Bold = true;

            row++;
            sheet.Cells[row, 1].Value = "Đơn hàng tuần trước:";
            sheet.Cells[row, 2].Value = lastWeekOrders.Count;
            sheet.Cells[row, 1].Style.Font.Bold = true;

            row++;
            sheet.Cells[row, 1].Value = "Tổng giá trị tuần này:";
            sheet.Cells[row, 2].Value = weekOrders.Sum(o => o.TotalPrice);
            sheet.Cells[row, 2].Style.Numberformat.Format = "#,##0";
            sheet.Cells[row, 1].Style.Font.Bold = true;

            row++;
            sheet.Cells[row, 1].Value = "Tổng giá trị tuần trước:";
            sheet.Cells[row, 2].Value = lastWeekOrders.Sum(o => o.TotalPrice);
            sheet.Cells[row, 2].Style.Numberformat.Format = "#,##0";
            sheet.Cells[row, 1].Style.Font.Bold = true;

            row++;
            sheet.Cells[row, 1].Value = "Đơn hàng trung bình/ngày:";
            sheet.Cells[row, 2].Value = Math.Round(weekOrders.Count / 7.0, 1);
            sheet.Cells[row, 1].Style.Font.Bold = true;

            // Customer Statistics
            row += 2;
            sheet.Cells[row, 1].Value = "THỐNG KÊ KHÁCH HÀNG";
            sheet.Cells[row, 1].Style.Font.Bold = true;
            sheet.Cells[row, 1].Style.Font.Size = 14;

            row++;
            sheet.Cells[row, 1].Value = "Khách hàng mới tuần này:";
            sheet.Cells[row, 2].Value = newCustomers.Count;
            sheet.Cells[row, 1].Style.Font.Bold = true;

            // Employee Statistics
            row += 2;
            sheet.Cells[row, 1].Value = "THỐNG KÊ NHÂN VIÊN";
            sheet.Cells[row, 1].Style.Font.Bold = true;
            sheet.Cells[row, 1].Style.Font.Size = 14;

            row++;
            sheet.Cells[row, 1].Value = "Tổng số nhân viên:";
            sheet.Cells[row, 2].Value = totalEmployees;
            sheet.Cells[row, 1].Style.Font.Bold = true;

            // Growth Analysis
            row += 2;
            sheet.Cells[row, 1].Value = "PHÂN TÍCH TĂNG TRƯỞNG";
            sheet.Cells[row, 1].Style.Font.Bold = true;
            sheet.Cells[row, 1].Style.Font.Size = 14;

            var orderGrowth = weekOrders.Count - lastWeekOrders.Count;
            var revenueGrowth = weekOrders.Sum(o => o.TotalPrice) - lastWeekOrders.Sum(o => o.TotalPrice);

            row++;
            sheet.Cells[row, 1].Value = "Tăng trưởng đơn hàng:";
            sheet.Cells[row, 2].Value = orderGrowth;
            sheet.Cells[row, 2].Style.Font.Color.SetColor(orderGrowth >= 0 ? Color.Green : Color.Red);
            sheet.Cells[row, 1].Style.Font.Bold = true;

            row++;
            sheet.Cells[row, 1].Value = "Tăng trưởng doanh thu:";
            sheet.Cells[row, 2].Value = revenueGrowth;
            sheet.Cells[row, 2].Style.Numberformat.Format = "#,##0";
            sheet.Cells[row, 2].Style.Font.Color.SetColor(revenueGrowth >= 0 ? Color.Green : Color.Red);
            sheet.Cells[row, 1].Style.Font.Bold = true;

            // Auto-fit columns
            sheet.Cells.AutoFitColumns();
        }

        private void CreateOrdersSheet(ExcelWorksheet sheet, List<MeoMeo.Domain.Entities.Order> orders)
        {
            // Header
            sheet.Cells[1, 1].Value = "Mã đơn hàng";
            sheet.Cells[1, 2].Value = "Khách hàng";
            sheet.Cells[1, 3].Value = "Ngày tạo";
            sheet.Cells[1, 4].Value = "Trạng thái";
            sheet.Cells[1, 5].Value = "Tổng tiền";

            // Style header
            var headerRange = sheet.Cells[1, 1, 1, 5];
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));
            headerRange.Style.Font.Color.SetColor(Color.White);

            // Data
            int row = 2;
            foreach (var order in orders)
            {
                sheet.Cells[row, 1].Value = order.Code;
                sheet.Cells[row, 2].Value = order.CustomerName;
                sheet.Cells[row, 3].Value = order.CreationTime.ToString("dd/MM/yyyy HH:mm");
                sheet.Cells[row, 4].Value = GetStatusName(order.Status);
                sheet.Cells[row, 5].Value = order.TotalPrice;
                sheet.Cells[row, 5].Style.Numberformat.Format = "#,##0";
                row++;
            }

            // Auto-fit columns
            sheet.Cells.AutoFitColumns();
        }

        private void CreateCustomersSheet(ExcelWorksheet sheet, List<MeoMeo.Domain.Entities.Customers> customers)
        {
            // Header
            sheet.Cells[1, 1].Value = "Mã khách hàng";
            sheet.Cells[1, 2].Value = "Tên khách hàng";
            sheet.Cells[1, 3].Value = "Email";
            sheet.Cells[1, 4].Value = "Số điện thoại";
            sheet.Cells[1, 5].Value = "Ngày tạo";

            // Style header
            var headerRange = sheet.Cells[1, 1, 1, 5];
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));
            headerRange.Style.Font.Color.SetColor(Color.White);

            // Data
            int row = 2;
            foreach (var customer in customers)
            {
                sheet.Cells[row, 1].Value = customer.Code;
                sheet.Cells[row, 2].Value = customer.Name;
                sheet.Cells[row, 3].Value = customer.User?.Email ?? "";
                sheet.Cells[row, 4].Value = customer.PhoneNumber;
                sheet.Cells[row, 5].Value = customer.CreationTime.ToString("dd/MM/yyyy HH:mm");
                row++;
            }

            // Auto-fit columns
            sheet.Cells.AutoFitColumns();
        }

        private void CreateTopProductsSheet(ExcelWorksheet sheet, List<dynamic> topProducts)
        {
            // Header
            sheet.Cells[1, 1].Value = "STT";
            sheet.Cells[1, 2].Value = "Tên sản phẩm";
            sheet.Cells[1, 3].Value = "Số lượng bán";
            sheet.Cells[1, 4].Value = "Doanh thu";

            // Style header
            var headerRange = sheet.Cells[1, 1, 1, 4];
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));
            headerRange.Style.Font.Color.SetColor(Color.White);

            // Data
            int row = 2;
            int stt = 1;
            foreach (var product in topProducts)
            {
                sheet.Cells[row, 1].Value = stt;
                sheet.Cells[row, 2].Value = product.ProductName;
                sheet.Cells[row, 3].Value = product.QuantitySold;
                sheet.Cells[row, 4].Value = product.Revenue;
                sheet.Cells[row, 4].Style.Numberformat.Format = "#,##0";
                row++;
                stt++;
            }

            // Auto-fit columns
            sheet.Cells.AutoFitColumns();
        }

        private async Task<List<dynamic>> GetTopProductsAsync(DateTime startDate, DateTime endDate)
        {
            var topProducts = await _orderDetailRepository.Query()
                .Where(od => od.Order.CreationTime >= startDate && od.Order.CreationTime < endDate)
                .GroupBy(od => od.ProductName)
                .Select(g => new
                {
                    ProductName = g.Key,
                    QuantitySold = g.Sum(od => od.Quantity),
                    Revenue = g.Sum(od => od.Quantity * od.Price)
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(10)
                .ToListAsync();

            return topProducts.Cast<dynamic>().ToList();
        }

        private string GetStatusName(EOrderStatus status)
        {
            return status switch
            {
                EOrderStatus.Pending => "Chờ xác nhận",
                EOrderStatus.Confirmed => "Đã xác nhận",
                EOrderStatus.InTransit => "Đang vận chuyển",
                EOrderStatus.Canceled => "Đã hủy",
                EOrderStatus.Completed => "Hoàn thành",
                EOrderStatus.PendingReturn => "Chờ hoàn hàng",
                EOrderStatus.Returned => "Đã hoàn hàng",
                EOrderStatus.RejectReturned => "Từ chối hoàn hàng",
                _ => status.ToString()
            };
        }
    }
}
