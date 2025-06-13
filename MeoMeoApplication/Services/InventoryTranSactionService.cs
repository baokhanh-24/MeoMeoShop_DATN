using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
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
        private readonly MeoMeoDbContext _meoDbContext;
        public InventoryTranSactionService(MeoMeoDbContext meoMeoDbContext)
        {
            _meoDbContext = meoMeoDbContext;
        }
        public async Task<InventoryTranSactionDTO> CreateAsync(InventoryTranSactionDTO dto)
        {
            var newtrac = new InventoryTransaction
            {
                Id = Guid.NewGuid(),
                InventoryBatchId = dto.InventoryBatchId,
                Quantity = dto.Quantity,
                CreationTime = DateTime.UtcNow,
                CreateBy = dto.CreateBy,
                Type = dto.Type,
                Note = dto.Note
            };
            _meoDbContext.Add(newtrac);
            await _meoDbContext.SaveChangesAsync();
            return new InventoryTranSactionDTO
            {
                Id = newtrac.Id,
                InventoryBatchId = dto.InventoryBatchId,
                Quantity = dto.Quantity,
                CreationTime = newtrac.CreationTime,
                CreateBy = dto.CreateBy,
                Type = dto.Type,
                Note = dto.Note
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var transaction = await _meoDbContext.inventoryTransactions.FindAsync(id);
            if (transaction == null)
            {
                return false;
            }
            _meoDbContext.inventoryTransactions.Remove(transaction);
            await _meoDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<InventoryTranSactionDTO>> GetAllAsync()
        {
            return await _meoDbContext.inventoryTransactions.Select(tr => new InventoryTranSactionDTO
            {
                Id = tr.Id,
                InventoryBatchId = tr.InventoryBatchId,
                Quantity = tr.Quantity,
                CreationTime = tr.CreationTime,
                CreateBy = tr.CreateBy,
                Type = tr.Type,
                Note = tr.Note
            }).ToListAsync();
        }

        public async Task<InventoryTranSactionDTO> GetByIdAsync(Guid id)
        {
            var x = await _meoDbContext.inventoryTransactions.FindAsync(id);
            if (x == null)
            {
                return null;
            }
            return new InventoryTranSactionDTO
            {
                Id = x.Id,
                InventoryBatchId = x.InventoryBatchId,
                Quantity = x.Quantity,
                CreationTime = x.CreationTime,
                CreateBy = x.CreateBy,
                Type = x.Type,
                Note = x.Note
            };
        }

        public async Task<InventoryTranSactionDTO> UpdateAsync(Guid id, InventoryTranSactionDTO dto)
        {
            var x = await _meoDbContext.inventoryTransactions.FindAsync(id);
            if (x == null)
            {
                throw new Exception("Not found");
            }
            x.InventoryBatchId = dto.InventoryBatchId;
            x.Quantity = dto.Quantity;
            x.CreationTime = dto.CreationTime;
            x.CreateBy = dto.CreateBy;
            x.Type = dto.Type;
            x.Note = dto.Note;
            await _meoDbContext.SaveChangesAsync();
            return new InventoryTranSactionDTO
            {
                Id = x.Id,
                InventoryBatchId = x.InventoryBatchId,
                Quantity = x.Quantity,
                CreationTime = x.CreationTime,
                CreateBy = x.CreateBy,
                Type = x.Type,
                Note = x.Note
            };
        }
    }
}
