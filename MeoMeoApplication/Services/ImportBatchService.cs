using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.ImportBatch;
using MeoMeo.Domain.Commons.Enums;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeoMeo.Domain.Commons;

namespace MeoMeo.Application.Services
{
    public class ImportBatchService : IImportBatchServices
    {
        private readonly IImportBatchRepository _importBatchRepository;
        private readonly IMapper _mapper;

        public ImportBatchService(IImportBatchRepository importBatchRepository, IMapper mapper)
        {
            _importBatchRepository = importBatchRepository;
            _mapper = mapper;
        }

        public async Task<ImportBatchResponseDTO> CreateAsync(ImportBatchDTO dto)
        {
            try
            {
                var importBatch = new ImportBatch
                {
                    Id = Guid.NewGuid(),
                    Code = dto.Code,
                    ImportDate = dto.ImportDate,
                    Note = dto.Note
                };

                await _importBatchRepository.AddAsync(importBatch);

                return new ImportBatchResponseDTO
                {
                    ResponseStatus = BaseStatus.Success,
                    Message = "Tạo lô nhập thành công",
                    Id = importBatch.Id,
                    Code = importBatch.Code,
                    ImportDate = importBatch.ImportDate,
                    Note = importBatch.Note
                };
            }
            catch (Exception ex)
            {
                return new ImportBatchResponseDTO()
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = ex.Message,
                };
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var importBatch = await _importBatchRepository.Query()
                .Include(ib => ib.InventoryBatches)
                .FirstOrDefaultAsync(ib => ib.Id == id);

            if (importBatch == null)
            {
                return false;
            }

            // Check if any inventory batch in this import batch is approved
            var hasApprovedInventoryBatch = importBatch.InventoryBatches?
                .Any(ib => ib.Status == EInventoryBatchStatus.Approved) ?? false;

            if (hasApprovedInventoryBatch)
            {
                return false; // Cannot delete import batch that has approved inventory batches
            }

            await _importBatchRepository.DeleteAsync(id);
            return true;
        }

        public async Task<PagingExtensions.PagedResult<ImportBatchDTO, GetListImportBatchResponseDTO>> GetAllAsync(GetListImportBatchRequestDTO request)
        {
            var metaDataValue = new GetListImportBatchResponseDTO();
            try
            {
                var query = _importBatchRepository.Query();

                // Count total
                metaDataValue.TotalAll = await _importBatchRepository.Query().CountAsync();

                // Apply filters
                if (!string.IsNullOrEmpty(request.CodeFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.Code, $"%{request.CodeFilter}%"));
                }
                if (!string.IsNullOrEmpty(request.NoteFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.Note, $"%{request.NoteFilter}%"));
                }
                if (request.FromDate.HasValue)
                {
                    query = query.Where(c => c.ImportDate >= request.FromDate.Value);
                }
                if (request.ToDate.HasValue)
                {
                    query = query.Where(c => c.ImportDate <= request.ToDate.Value);
                }

                var filteredImportBatches = await _importBatchRepository.GetPagedAsync(query, request.PageIndex, request.PageSize);

                // Get import batch IDs for the current page
                var importBatchIds = filteredImportBatches.Items.Select(ib => ib.Id).ToList();

                // Load import batches with inventory batches to check approved status
                var importBatchesWithInventoryBatches = await _importBatchRepository.Query()
                    .Include(ib => ib.InventoryBatches)
                    .Where(ib => importBatchIds.Contains(ib.Id))
                    .ToListAsync();

                var dtoItems = _mapper.Map<List<ImportBatchDTO>>(filteredImportBatches.Items);

                // Calculate HasApprovedInventoryBatch for each import batch
                foreach (var dto in dtoItems)
                {
                    var importBatch = importBatchesWithInventoryBatches.FirstOrDefault(ib => ib.Id == dto.Id);
                    if (importBatch?.InventoryBatches != null)
                    {
                        dto.HasApprovedInventoryBatch = importBatch.InventoryBatches.Any(ib => ib.Status == EInventoryBatchStatus.Approved);
                    }
                }

                return new PagingExtensions.PagedResult<ImportBatchDTO, GetListImportBatchResponseDTO>
                {
                    TotalRecords = filteredImportBatches.TotalRecords,
                    PageIndex = filteredImportBatches.PageIndex,
                    PageSize = filteredImportBatches.PageSize,
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

        public async Task<ImportBatchResponseDTO> GetByIdAsync(Guid id)
        {
            var importBatch = await _importBatchRepository.GetByIdAsync(id);
            if (importBatch == null)
            {
                return new ImportBatchResponseDTO() { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy Id" };
            }
            return new ImportBatchResponseDTO
            {
                Id = importBatch.Id,
                Code = importBatch.Code,
                ImportDate = importBatch.ImportDate,
                Note = importBatch.Note,
                ResponseStatus = BaseStatus.Success,
                Message = ""
            };
        }

        public async Task<ImportBatchDetailDTO> GetDetailByIdAsync(Guid id)
        {
            var importBatch = await _importBatchRepository.GetImportBatchWithDetailsAsync(id);
            if (importBatch == null)
            {
                return new ImportBatchDetailDTO();
            }

            var products = importBatch.InventoryBatches.Select(ib => new ImportBatchProductDTO
            {
                InventoryBatchId = ib.Id,
                ProductDetailId = ib.ProductDetailId,
                SKU = ib.ProductDetail?.Sku ?? string.Empty,
                ProductName = ib.ProductDetail?.Product?.Name ?? string.Empty,
                ColourName = ib.ProductDetail?.Colour?.Name ?? string.Empty,
                SizeValue = ib.ProductDetail?.Size?.Value ?? string.Empty,
                Thumbnail = ib.ProductDetail?.Product?.Thumbnail ?? string.Empty,
                OriginalPrice = ib.OriginalPrice,
                Quantity = ib.Quantity,
                Status = ib.Status
            }).ToList();

            return new ImportBatchDetailDTO
            {
                Id = importBatch.Id,
                Code = importBatch.Code,
                ImportDate = importBatch.ImportDate,
                Note = importBatch.Note,
                Products = products
            };
        }

        public async Task<ImportBatchResponseDTO> UpdateAsync(Guid id, ImportBatchDTO dto)
        {
            var importBatch = await _importBatchRepository.GetByIdAsync(id);
            if (importBatch == null)
            {
                return new ImportBatchResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy Id để cập nhật" };
            }
            _mapper.Map(dto, importBatch);
            await _importBatchRepository.UpdateAsync(importBatch);
            return new ImportBatchResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Cập nhật thành công" };
        }


    }
}
