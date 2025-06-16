using MeoMeo.Domain.Commons.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs
{
    public class CreateOrUpdateResetPasswordHistoryDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime ExpriedDate { get; set; }
        public string Code { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public DateTime? SuccessDate { get; set; }
        public EResetPasswordHistoryStatus Status { get; set; }
    }
}
