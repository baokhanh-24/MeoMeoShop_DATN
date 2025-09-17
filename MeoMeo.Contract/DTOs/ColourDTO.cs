using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs
{
    public class ColourDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Status { get; set; }
        public EColourStatus StatusEnum
        {
            get => (EColourStatus)Status;
            set => Status = (int)value;
        }
    }
}
