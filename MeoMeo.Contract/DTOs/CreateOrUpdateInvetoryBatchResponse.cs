using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs
{
    public class CreateOrUpdateInvetoryBatchResponse : BaseResponse
    {
        public Guid Id { get; set; }
        public Guid ProductDetailId { get; set; }
        public float OriginalPrice { get; set; }
        public string Code { get; set; }
        public int Quantity { get; set; }
        public string Note { get; set; }
        public EInventoryBatchStatus Status { get; set; }
    }
}
