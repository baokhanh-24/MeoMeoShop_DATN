using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs
{
    public class CreateOrUpdateCustomersBankDTO
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid BankId { get; set; }
        public string AccountNumber { get; set; }
        public string Beneficiary { get; set; }
        public int Status { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public DateTime? LastModifiedTime { get; set; }
    }
}
