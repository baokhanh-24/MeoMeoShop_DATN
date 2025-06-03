using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities
{
    public class Promotion : BaseEnitityAudited
    {
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public virtual ICollection<PromotionDetail> PromotionDetails { get; set; }
    }
}
