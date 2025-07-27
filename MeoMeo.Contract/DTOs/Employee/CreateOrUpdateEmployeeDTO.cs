using MeoMeo.Domain.Commons.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs
{
    public class CreateOrUpdateEmployeeDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBird { get; set; }
        public string Address { get; set; }
        public int Status { get; set; }
        public EEmployeesStatus StatusEnum
        {
            get => (EEmployeesStatus)Status;
            set => Status = (int)value;
        }
    }
}
