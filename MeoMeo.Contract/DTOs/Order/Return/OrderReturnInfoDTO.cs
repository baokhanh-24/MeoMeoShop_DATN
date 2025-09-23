using System;
using System.Collections.Generic;
using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs.Order.Return
{
    public class OrderReturnInfoResponseDTO : BaseResponse
    {
        public bool CanReturn { get; set; }
        public List<string> ReturnableProducts { get; set; } = new List<string>();
        public List<string> NonReturnableProducts { get; set; } = new List<string>();
    }
}
