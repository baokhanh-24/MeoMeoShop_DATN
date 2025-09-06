using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MeoMeo.Contract.DTOs.ProductReview;
using MeoMeo.Domain.Commons;

namespace MeoMeo.Shared.IServices
{
    public interface IProductReviewClientService
    {
        Task<IEnumerable<ProductReviewDTO>> GetAllAsync();
        Task<PagingExtensions.PagedResult<ProductReviewDTO>> GetByProductDetailIdsAsync(GetListProductReviewDTO  request);
        Task<ProductReviewDTO> CreateAsync(ProductReviewCreateOrUpdateDTO dto);
    }
}


