using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs.Product
{
    public class GetListProductResponseDTO
    {
        public int TotalAll { get; set; }
        public int Selling { get; set; }
        public int StopSelling { get; set; }
        public int Pending { get; set; }
        public int Rejected { get; set; }
    }
} 