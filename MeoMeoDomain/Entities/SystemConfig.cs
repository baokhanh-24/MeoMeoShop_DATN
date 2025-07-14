using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Domain.Entities
{
    public class SystemConfig : BaseEntityAudited
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public ESystemConfigType Type { get; set; }
    }

}
