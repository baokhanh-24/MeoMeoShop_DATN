using MeoMeo.Contract.Commons;

namespace MeoMeo.Application.IServices
{
    public interface IExcelReportService
    {
        Task<byte[]> GenerateDailyReportExcelAsync();
        Task<byte[]> GenerateWeeklyReportExcelAsync();
    }
}
