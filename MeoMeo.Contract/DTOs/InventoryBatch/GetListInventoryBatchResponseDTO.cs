using MeoMeo.Contract.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs.InventoryBatch
{
    public class GetListInventoryBatchResponseDTO:BaseResponse
    {
        public int TotalAll { get; set; }
        public int Draft { get; set; }
        public int PendingApproval { get; set; }
        public int Aprroved { get; set; }
        public int Rejected { get; set; }
    }
}
