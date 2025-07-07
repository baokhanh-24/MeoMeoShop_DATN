using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
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
        async Task<CreateOrUpdateInventoryTranSactionResponse> IIventoryTranSactionServices.CreateAsync(InventoryTranSactionDTO dto)
        {
            CreateOrUpdateInventoryTranSactionResponse response = new CreateOrUpdateInventoryTranSactionResponse();
            var inventoryTransaction = new InventoryTransaction
            {
                Id = Guid.NewGuid(),
                InventoryBatchId = dto.InventoryBatchId,
                Quantity = dto.Quantity,
                CreationTime = dto.CreationTime,
                CreateBy = dto.CreateBy,
                Type = dto.Type,
                Note = dto.Note
            };
            await _iventoryTranSactionRepository.CreateAsync(inventoryTransaction);
            response.Id = inventoryTransaction.Id;
            response.InventoryBatchId = inventoryTransaction.InventoryBatchId;
            response.Quantity = inventoryTransaction.Quantity;
            response.CreationTime = inventoryTransaction.CreationTime;
            response.CreateBy = inventoryTransaction.CreateBy;
            response.Type = inventoryTransaction.Type;
            response.Note = inventoryTransaction.Note;
            response.Message = "Tạo lịch sử lô nhập thành công.";
            response.ResponseStatus = BaseStatus.Success;
            return response;
        }

        async Task<CreateOrUpdateInventoryTranSactionResponse> IIventoryTranSactionServices.GetByIdAsync(Guid id)
        {
            CreateOrUpdateInventoryTranSactionResponse response = new CreateOrUpdateInventoryTranSactionResponse();
            var getTransaction = await _iventoryTranSactionRepository.GetByIdAsync(id);
            if (getTransaction == null)
            {
                response.Message = "Không tìm thấy lịch sử lô nhập này.";
                response.ResponseStatus = BaseStatus.Error;
                return response;
            }
            response.InventoryBatchId = getTransaction.InventoryBatchId;
            response.Quantity = getTransaction.Quantity;
            response.CreationTime = getTransaction.CreationTime;
            response.CreateBy = getTransaction.CreateBy;
            response.Type = getTransaction.Type;
            response.Note = getTransaction.Note;
            response.Id = getTransaction.Id;
            response.Message = "Lấy lịch sử lô nhập thành công.";
            response.ResponseStatus = BaseStatus.Success;
            return response;
        }

        async Task<CreateOrUpdateInventoryTranSactionResponse> IIventoryTranSactionServices.UpdateAsync(Guid id, InventoryTranSactionDTO dto)
        {
            CreateOrUpdateInventoryTranSactionResponse response = new CreateOrUpdateInventoryTranSactionResponse();
            var getTransaction = await _iventoryTranSactionRepository.GetByIdAsync(id);
            if(getTransaction == null)
            {
                response.Message = "Không tìm thấy lịch sử lô nhập này.";
                response.ResponseStatus = BaseStatus.Error;
                return response;
            }
            getTransaction.InventoryBatchId = dto.InventoryBatchId;
            getTransaction.Quantity = dto.Quantity;
            getTransaction.CreationTime = dto.CreationTime;
            getTransaction.CreateBy = dto.CreateBy;
            getTransaction.Type = dto.Type;
            getTransaction.Note = dto.Note;
            await _iventoryTranSactionRepository.UpdateAsync(id, getTransaction);
            response.Id = getTransaction.Id;
            response.InventoryBatchId = getTransaction.InventoryBatchId;
            response.Quantity = getTransaction.Quantity;
            response.CreationTime = getTransaction.CreationTime;
            response.CreateBy = getTransaction.CreateBy;
            response.Type = getTransaction.Type;
            response.Note = getTransaction.Note;
            response.Message = "Cập nhật lịch sử lô nhập thành công.";
            response.ResponseStatus = BaseStatus.Success;
            return response;
        }
    }
}
