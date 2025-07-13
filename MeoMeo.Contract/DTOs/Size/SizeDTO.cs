using MeoMeo.Domain.Commons.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs.Size
{
    public class SizeDTO
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public string Code { get; set; }
        public int Status { get; set; }
        public ESizeStatus StatusEnum
        {
            get => (ESizeStatus)Status;
            set => Status = (int)value;
        }
    }
}
