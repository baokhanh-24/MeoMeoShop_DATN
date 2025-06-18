using MeoMeo.Application.IServices;
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

        public Task<InventoryBatch> CreateAsync(InventoryBatchDTO dto)
        {
            var phat = new InventoryBatch
            {
                Id = Guid.NewGuid(),
                ProductDetailId = dto.ProductDetailId,
                OriginalPrice = dto.OriginalPrice,
                Code = dto.Code,
                Quantity = dto.Quantity,
                Note = dto.Note,
                Status = dto.Status
            };
            return _inventoryBatchRepository.CreateAsync(phat);
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

        public async Task<InventoryBatch> GetByIdAsync(Guid id)
        {
            return await _inventoryBatchRepository.GetByIdAsync(id);
        }

        public async Task<InventoryBatch> UpdateAsync(Guid id, InventoryBatchDTO dto)
        {
            var phat = await _inventoryBatchRepository.GetByIdAsync(id);
            if (phat == null)
            {
                throw new Exception("Inventory batch not found.");
            }
            phat.ProductDetailId = dto.ProductDetailId;
            phat.OriginalPrice = dto.OriginalPrice;
            phat.Code = dto.Code;
            phat.Quantity = dto.Quantity;
            phat.Note = dto.Note;
            phat.Status = dto.Status;
            return await _inventoryBatchRepository.UpdateAsync(id, phat);
        }
    }
}
