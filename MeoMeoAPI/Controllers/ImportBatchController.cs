using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs.ImportBatch;
using MeoMeo.Domain.Commons;
using Microsoft.AspNetCore.Mvc;

namespace MeoMeo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportBatchController : ControllerBase
    {
        private readonly IImportBatchServices _importBatchServices;

        public ImportBatchController(IImportBatchServices importBatchServices)
        {
            _importBatchServices = importBatchServices;
        }

        [HttpGet("get-all-import-batch-async")]
        public async Task<PagingExtensions.PagedResult<ImportBatchDTO, GetListImportBatchResponseDTO>> GetAllImportBatchAsync([FromQuery] GetListImportBatchRequestDTO request)
        {
            var result = await _importBatchServices.GetAllAsync(request);
            return result;
        }

        [HttpGet("get-import-batch-by-id-async/{id}")]
        public async Task<ImportBatchDTO> GetImportBatchByIdAsync(Guid id)
        {
            var result = await _importBatchServices.GetByIdAsync(id);
            return new ImportBatchDTO
            {
                Id = result.Id,
                Code = result.Code,
                ImportDate = result.ImportDate,
                Note = result.Note
            };
        }

        [HttpGet("get-import-batch-detail-async/{id}")]
        public async Task<ImportBatchDetailDTO> GetImportBatchDetailAsync(Guid id)
        {
            var result = await _importBatchServices.GetDetailByIdAsync(id);
            return result;
        }

        [HttpPost("create-import-batch-async")]
        public async Task<ImportBatchResponseDTO> CreateImportBatchAsync([FromBody] ImportBatchDTO dto)
        {
            var result = await _importBatchServices.CreateAsync(dto);
            return result;
        }

        [HttpPut("update-import-batch-async/{id}")]
        public async Task<ImportBatchResponseDTO> UpdateImportBatchAsync(Guid id, [FromBody] ImportBatchDTO dto)
        {
            var result = await _importBatchServices.UpdateAsync(id, dto);
            return result;
        }

        [HttpDelete("delete-import-batch-async/{id}")]
        public async Task<bool> DeleteImportBatchAsync(Guid id)
        {
            var result = await _importBatchServices.DeleteAsync(id);
            return result;
        }
    }
}
