using MeoMeo.Contract.Commons;

namespace MeoMeo.Application.IServices
{
    public interface IReportService
    {
        Task<BaseResponse> SendDailyReportAsync(string adminEmail);
        Task<BaseResponse> SendWeeklyReportAsync(string adminEmail);
    }
}
