using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.InventoryBatch;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Commons.Enums;
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
        private readonly IIventoryBatchReposiory _inventoryBatchRepository;
        private readonly IMapper _mapper;
        public InventoryBatchService(IIventoryBatchReposiory iventoryBtachReposiory, IMapper mapper)
        {
            _inventoryBatchRepository = iventoryBtachReposiory;
            _mapper = mapper;
        }
        public async Task<InventoryBatchResponseDTO> CreateAsync(List<InventoryBatchDTO> dto)
        {
            foreach (var dtos in dto)
            {
                var inventoryBatch = new InventoryBatch
                {
                    Id = Guid.NewGuid(),
                    ProductDetailId = dtos.ProductDetailId,
                    OriginalPrice = dtos.OriginalPrice,
                    Code = dtos.Code,
                    Quantity = dtos.Quantity,
                    Note = dtos.Note,
                    Status = dtos.Status
                };

                await _inventoryBatchRepository.CreateAsync(inventoryBatch);
            }

            return new InventoryBatchResponseDTO
            {
                ResponseStatus = BaseStatus.Success,
                Message = "Thêm nhiều lô nhập thành công"
            };
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
        public async Task<PagingExtensions.PagedResult<InventoryBatchDTO, GetListInventoryBatchResponseDTO>> GetAllAsync(GetListInventoryBatchRequestDTO request)
        {
            var metaDataValue = new GetListInventoryBatchResponseDTO();
            try
            {
                var query = _inventoryBatchRepository.Query();
                var statusCounts = await _inventoryBatchRepository.Query().GroupBy(p => p.Status).Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                }).ToListAsync();
                metaDataValue.TotalAll = statusCounts.Sum(s => s.Count);
                metaDataValue.Draft = statusCounts.FirstOrDefault(s => s.Status == EInventoryBatchStatus.Draft)?.Count ?? 0;
                metaDataValue.PendingApproval = statusCounts.FirstOrDefault(s => s.Status == EInventoryBatchStatus.PendingApproval)?.Count ?? 0;
                metaDataValue.Aprroved = statusCounts.FirstOrDefault(s => s.Status == EInventoryBatchStatus.Aprroved)?.Count ?? 0;
                metaDataValue.Rejected = statusCounts.FirstOrDefault(s => s.Status == EInventoryBatchStatus.Rejected)?.Count ?? 0;
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
                metaDataValue.ResponseStatus = BaseStatus.Success;
                return new PagingExtensions.PagedResult<InventoryBatchDTO, GetListInventoryBatchResponseDTO>
                {
                    TotalRecords = filtedInventorryBatch.TotalRecords,
                    PageIndex = filtedInventorryBatch.PageIndex,
                    PageSize = filtedInventorryBatch.PageSize,
                    Items = dtoItems,
                    Metadata = metaDataValue,
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
