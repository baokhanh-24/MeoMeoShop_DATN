using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface ICartDetaillService
    {
        Task<IEnumerable<CartDetail>> GetAllCartDetaillAsync();
        Task<CartDetail> GetCartDetaillByIdAsync(Guid id);
        Task<CartDetailResponseDTO> CreateCartDetaillAsync(CartDetailDTO cartDetailDTO);
        Task<CartDetailResponseDTO> UpdataCartDetaillAsync(CartDetailDTO cartDetailDTO);
        Task<CartDetailResponseDTO> DeleteCartDetaillAsync(Guid id);

    }
}
