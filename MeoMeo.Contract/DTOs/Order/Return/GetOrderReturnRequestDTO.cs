using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;
using System;

namespace MeoMeo.Contract.DTOs.Order.Return
{
    public class GetOrderReturnRequestDTO : BasePaging
    {
        public string? CodeFilter { get; set; }
        public string? OrderCodeFilter { get; set; }
        public EOrderReturnStatus? StatusFilter { get; set; }
        public ERefundMethod? RefundMethodFilter { get; set; }
        public DateTime? FromDateFilter { get; set; }
        public DateTime? ToDateFilter { get; set; }
        public Guid? CustomerIdFilter { get; set; }
    }
}
