using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;

namespace MeoMeo.Shared.IServices
{
    public interface IBankClientService
    {
        Task<PagingExtensions.PagedResult<BankDTO>> GetAllBankAsync(GetListBankRequestDTO request);

    }
}
