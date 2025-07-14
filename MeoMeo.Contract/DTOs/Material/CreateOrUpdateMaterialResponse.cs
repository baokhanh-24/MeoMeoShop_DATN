using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs.Material
{
    public class CreateOrUpdateMaterialResponse : BaseResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public EMaterialDurability Durability { get; set; }
        public bool WaterProof { get; set; }
        public EMaterialWeight Weight { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
    }
}
