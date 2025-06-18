using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.Services
{
    public class InventoryTranSactionService : IIventoryTranSactionServices
    {
        private readonly IInventoryTranSactionRepository _iventoryTranSactionRepository;
        public InventoryTranSactionService(IInventoryTranSactionRepository inventoryTranSactionRepository)
        {
            _iventoryTranSactionRepository = inventoryTranSactionRepository;
        }
        public Task<InventoryTransaction> CreateAsync(InventoryTranSactionDTO dto)
        {
            try
            {
                var inventoryTransaction = new InventoryTransaction
                {
                    Id = dto.Id,
                    InventoryBatchId = dto.InventoryBatchId,
                    Quantity = dto.Quantity,
                    CreationTime = dto.CreationTime,
                    CreateBy = dto.CreateBy,
                    Type = dto.Type,
                    Note = dto.Note
                };
                return _iventoryTranSactionRepository.CreateAsync(inventoryTransaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the inventory transaction.", ex);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var phat = await _iventoryTranSactionRepository.GetByIdAsync(id);
            if (phat == null)
            {
                return false;
            }
            await _iventoryTranSactionRepository.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<InventoryTransaction>> GetAllAsync()
        {
            return await _iventoryTranSactionRepository.GetAllAsync();
        }

        public async Task<InventoryTransaction> GetByIdAsync(Guid id)
        {
            var phat = await _iventoryTranSactionRepository.GetByIdAsync(id);
            return phat;
        }

        public async Task<InventoryTransaction> UpdateAsync(Guid id, InventoryTranSactionDTO dto)
        {
            var phat = await _iventoryTranSactionRepository.GetByIdAsync(id);
            if (phat == null)
            {
                throw new Exception("Inventory transaction not found.");
            }
            phat.InventoryBatchId = dto.InventoryBatchId;
            phat.Quantity = dto.Quantity;
            phat.CreationTime = dto.CreationTime;
            phat.CreateBy = dto.CreateBy;
            phat.Type = dto.Type;
            phat.Note = dto.Note;
            return await _iventoryTranSactionRepository.UpdateAsync(id, phat);
        }
    }
}
