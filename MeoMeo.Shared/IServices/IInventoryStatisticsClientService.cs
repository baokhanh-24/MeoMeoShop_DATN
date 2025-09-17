using MeoMeo.Contract.DTOs;
using System.Threading.Tasks;

namespace MeoMeo.Shared.IServices
{
    public interface IInventoryStatisticsClientService
    {
        Task<GetInventoryStatisticsResponseDTO> GetInventoryStatisticsAsync(GetInventoryStatisticsRequestDTO request);
        Task<GetInventoryHistoryResponseDTO> GetInventoryHistoryAsync(GetInventoryHistoryRequestDTO request);
    }
}
