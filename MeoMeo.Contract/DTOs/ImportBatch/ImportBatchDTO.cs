using System;
using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs.ImportBatch
{
    public class ImportBatchDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public DateTime ImportDate { get; set; }
        public string Note { get; set; }
        public bool HasApprovedInventoryBatch { get; set; } = false; // Có sản phẩm đã được duyệt
    }

    public class ImportBatchResponseDTO : BaseResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public DateTime ImportDate { get; set; }
        public string Note { get; set; }
    }

    public class GetListImportBatchRequestDTO : BasePaging
    {
        public string? CodeFilter { get; set; }
        public string? NoteFilter { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class GetListImportBatchResponseDTO
    {
        public int TotalAll { get; set; }
    }
}
