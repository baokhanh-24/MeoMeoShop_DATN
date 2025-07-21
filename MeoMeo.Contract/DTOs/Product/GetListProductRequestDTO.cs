using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs.Product
{
    public class GetListProductRequestDTO : BasePaging
    {
        public string? NameFilter { get; set; }
    }
}
