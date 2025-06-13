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
    public class InventoryBatchService : IInventoryBatchServices
    {
        private readonly MeoMeoDbContext _meoDbContext;
        public InventoryBatchService(MeoMeoDbContext meoDbContext)
        {
            _meoDbContext = meoDbContext;
        }
        public async Task<InventoryBatchDTO> CreateAsync(InventoryBatchDTO dto)
        {
            var newInventoryBatch = new InventoryBatch
            {
                Id = Guid.NewGuid(),
                ProductDetailId = dto.ProductDetailId,
                OriginalPrice = dto.OriginalPrice,
                Code = dto.Code,
                Quantity = dto.Quantity,
                Note = dto.Note,
                Status = dto.Status
            };

            _meoDbContext.Add(newInventoryBatch);
            await _meoDbContext.SaveChangesAsync();

            return new InventoryBatchDTO
            {
                Id = newInventoryBatch.Id, 
                ProductDetailId = dto.ProductDetailId,
                OriginalPrice = dto.OriginalPrice,
                Code = dto.Code,
                Quantity = dto.Quantity,
                Note = dto.Note,
                Status = dto.Status
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var batch = await _meoDbContext.inventoryBatches.FindAsync(id);
            if(batch == null)
            {
                return false;
            }
            _meoDbContext.inventoryBatches.Remove(batch);
            await _meoDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<InventoryBatchDTO>> GetAllAsync()
        {
            return await _meoDbContext.inventoryBatches.Select(x => new InventoryBatchDTO
            {
                Id = x.Id,
                ProductDetailId = x.ProductDetailId,
                OriginalPrice = x.OriginalPrice,
                Code = x.Code,
                Quantity = x.Quantity,
                Note = x.Note,
                Status = x.Status
            }).ToListAsync();
        }

        public async Task<InventoryBatchDTO> GetByIdAsync(Guid id)
        {
            var x = await _meoDbContext.inventoryBatches.FindAsync(id);
            if(x == null)
            {
                return null;
            }
            return new InventoryBatchDTO
            {
                Id = x.Id,
                ProductDetailId = x.ProductDetailId,
                OriginalPrice = x.OriginalPrice,
                Code = x.Code,
                Quantity = x.Quantity,
                Note = x.Note,
                Status = x.Status
            };
        }

        public async Task<InventoryBatchDTO> UpdateAsync(Guid id, InventoryBatchDTO dto)
        {
            var batch = await _meoDbContext.inventoryBatches.FindAsync(id);
            if (batch == null) 
            {
                throw new Exception("Not found");
            }
            batch.ProductDetailId = dto.ProductDetailId;
            batch.OriginalPrice = dto.OriginalPrice;
            batch.Code = dto.Code;
            batch.Quantity = dto.Quantity;
            batch.Note = dto.Note;
            batch.Status = dto.Status;
            await _meoDbContext.SaveChangesAsync();

            return new InventoryBatchDTO
            {
                Id = batch.Id,
                ProductDetailId = batch.ProductDetailId,
                OriginalPrice = batch.OriginalPrice,
                Code = batch.Code,
                Quantity = batch.Quantity,
                Note = batch.Note,
                Status = batch.Status
            };
        }
    }
}
