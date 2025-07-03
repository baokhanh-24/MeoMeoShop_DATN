using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using MeoMeo.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MeoMeo.Application.Services
{
    public class InventoryBatchService : IInventoryBatchServices
    {
        private readonly IIventoryBtachReposiory _inventoryBatchRepository;
        private readonly IMapper _mapper;
        public InventoryBatchService(IIventoryBtachReposiory iventoryBtachReposiory, IMapper mapper)
        {
            _inventoryBatchRepository = iventoryBtachReposiory;
            _mapper = mapper;
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            var inventoryBatch = await _inventoryBatchRepository.GetByIdAsync(id);
            if (inventoryBatch == null)
            {
                return false;
            }
            await _inventoryBatchRepository.DeleteInventoryBatchAsync(id);
            return true;
        }
        public async Task<PagingExtensions.PagedResult<InventoryBatchDTO>> GetAllAsync(GetListInventoryBatchRequestDTO request)
        {
            try
            {
                var query = _inventoryBatchRepository.Query();
                if (!string.IsNullOrEmpty(request.CodeFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.Code, $"%{request.CodeFilter}%"));
                }
                if (!string.IsNullOrEmpty(request.NoteFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.Note, $"%{request.NoteFilter}%"));
                }
                if (request.StatusFilter != null)
                {
                    query = query.Where(c => c.Status == request.StatusFilter);
                }
                var filtedInventorryBatch = await _inventoryBatchRepository.GetPagedAsync(query,request.PageIndex,request.PageSize);
                var dtoItems = _mapper.Map<List<InventoryBatchDTO>>(filtedInventorryBatch.Items);
                return new PagingExtensions.PagedResult<InventoryBatchDTO>
                {
                    TotalRecords = filtedInventorryBatch.TotalRecords,
                    PageIndex = filtedInventorryBatch.PageIndex,
                    PageSize = filtedInventorryBatch.PageSize,
                    Items = dtoItems
                };
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<InventoryBatchResponseDTO> GetByIdAsync(Guid id)
        {
            var inventoryBatch =  await _inventoryBatchRepository.GetByIdAsync(id);
            if (inventoryBatch == null) 
            {
                return new InventoryBatchResponseDTO() { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy Id" };
            }
            return new InventoryBatchResponseDTO
            {
                Id = inventoryBatch.Id,
                ProductDetailId = inventoryBatch.ProductDetailId,
                OriginalPrice = inventoryBatch.OriginalPrice,
                Code = inventoryBatch.Code,
                Quantity = inventoryBatch.Quantity,
                Note = inventoryBatch.Note,
                Status = inventoryBatch.Status,
                ResponseStatus = BaseStatus.Success,
                Message = ""
            };
        }

        public async Task<InventoryBatchResponseDTO> UpdateAsync(Guid id, InventoryBatchDTO dto)
        {
            var inventoryBatch = await _inventoryBatchRepository.GetByIdAsync(id);
            if (inventoryBatch == null)
            {
                return new InventoryBatchResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy Id trên để cập nhật" };
            }
            _mapper.Map(dto, inventoryBatch);
            await _inventoryBatchRepository.UpdateAsync(inventoryBatch);
            return new InventoryBatchResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Cập nhật thành công" };
        }
    }
}
