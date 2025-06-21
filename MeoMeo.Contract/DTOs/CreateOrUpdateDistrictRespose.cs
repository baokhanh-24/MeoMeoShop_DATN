using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs
{
    public class CreateOrUpdateDistrictRespose : BaseResponse
    {
        public Guid? Id { get; set; }
        public Guid ProvinceId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
