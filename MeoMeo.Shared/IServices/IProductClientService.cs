using MeoMeo.Contract.DTOs.Product;
using MeoMeo.Domain.Commons;

namespace MeoMeo.Shared.IServices
{
    public interface IProductClientService
    {
        public Task<PagingExtensions.PagedResult<ProductDTO>> GetAllProductAsync(GetListProductRequestDTO request);
    }
}
