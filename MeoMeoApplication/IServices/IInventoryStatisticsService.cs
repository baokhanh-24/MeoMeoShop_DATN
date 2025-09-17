using MeoMeo.Contract.DTOs;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IInventoryStatisticsService
    {
        Task<GetInventoryStatisticsResponseDTO> GetInventoryStatisticsAsync(GetInventoryStatisticsRequestDTO request);
        Task<GetInventoryHistoryResponseDTO> GetInventoryHistoryAsync(GetInventoryHistoryRequestDTO request);
    }
}
