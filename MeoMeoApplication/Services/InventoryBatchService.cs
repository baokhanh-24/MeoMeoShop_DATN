using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
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
        public InventoryBatchService(IIventoryBtachReposiory iventoryBtachReposiory)
        {
            _inventoryBatchRepository = iventoryBtachReposiory;
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            var phat = await _inventoryBatchRepository.GetByIdAsync(id);
            if (phat == null)
            {
                return false;
            }
            await _inventoryBatchRepository.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<InventoryBatch>> GetAllAsync()
        {
            return await _inventoryBatchRepository.GetAllAsync();
        }


        async Task<CreateOrUpdateInvetoryBatchResponse> IInventoryBatchServices.CreateAsync(InventoryBatchDTO dto)
        {
            CreateOrUpdateInvetoryBatchResponse response = new CreateOrUpdateInvetoryBatchResponse();
            var inventoryBatch = new InventoryBatch
            {
                Id = Guid.NewGuid(),
                ProductDetailId = dto.ProductDetailId,
                OriginalPrice = dto.OriginalPrice,
                Code = dto.Code,
                Quantity = dto.Quantity,
                Note = dto.Note,
                Status = dto.Status
            };
            await _inventoryBatchRepository.CreateAsync(inventoryBatch);
            response.Id = inventoryBatch.Id;
            response.ProductDetailId = inventoryBatch.ProductDetailId;
            response.OriginalPrice = inventoryBatch.OriginalPrice;
            response.Code = inventoryBatch.Code;
            response.Quantity = inventoryBatch.Quantity;
            response.Note = inventoryBatch.Note;
            response.Status = inventoryBatch.Status;
            response.Message = "Tạo lô nhập thành công.";
            response.ResponseStatus = BaseStatus.Success;
            return response;
        }

        async Task<CreateOrUpdateInvetoryBatchResponse> IInventoryBatchServices.GetByIdAsync(Guid id)
        {
            CreateOrUpdateInvetoryBatchResponse response = new CreateOrUpdateInvetoryBatchResponse();
            var getBatch = await _inventoryBatchRepository.GetByIdAsync(id);
            if (getBatch == null)
            {
                response.Message = "Không tìm thấy lô nhập này.";
                response.ResponseStatus = BaseStatus.Error;
                return response;
            }
            response.Id = getBatch.Id;
            response.ProductDetailId = getBatch.ProductDetailId;
            response.OriginalPrice = getBatch.OriginalPrice;
            response.Code = getBatch.Code;
            response.Quantity = getBatch.Quantity;
            response.Note = getBatch.Note;
            response.Status = getBatch.Status;
            response.Message = "Lấy thông tin lô nhập thành công.";
            response.ResponseStatus = BaseStatus.Success;
            return response;
        }

        async Task<CreateOrUpdateInvetoryBatchResponse> IInventoryBatchServices.UpdateAsync(Guid id, InventoryBatchDTO dto)
        {
            CreateOrUpdateInvetoryBatchResponse response = new CreateOrUpdateInvetoryBatchResponse();
            var getBatch = await _inventoryBatchRepository.GetByIdAsync(id);
            if(getBatch == null)
            {
                response.Message = "Không tìm thấy lô nhập này.";
                response.ResponseStatus = BaseStatus.Error;
                return response;
            }
            getBatch.ProductDetailId = dto.ProductDetailId;
            getBatch.OriginalPrice = dto.OriginalPrice;
            getBatch.Code = dto.Code;
            getBatch.Quantity = dto.Quantity;
            getBatch.Note = dto.Note;
            getBatch.Status = dto.Status;
            await _inventoryBatchRepository.UpdateAsync(id, getBatch);
            return response;
        }
    }
}
