using MeoMeo.Contract.DTOs.VietQR;
using System.Text.Json;

namespace MeoMeo.Shared.IServices
{
    public interface IVietQRClientService
    {
        Task<List<BankDTO>> GetBanksAsync();
    }
}