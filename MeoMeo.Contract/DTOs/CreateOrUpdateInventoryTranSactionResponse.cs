using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs
{
    public class CreateOrUpdateInventoryTranSactionResponse : BaseResponse
    {
        public Guid Id { get; set; }
        public Guid InventoryBatchId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreationTime { get; set; }
        public int CreateBy { get; set; }
        public EInventoryTranctionType Type { get; set; }
        public string Note { get; set; }
    }
}
