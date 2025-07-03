using AutoMapper;
using MeoMeo.Application.IServices;
﻿using MeoMeo.Application.IServices;
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
