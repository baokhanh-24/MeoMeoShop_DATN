using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Domain.Entities;

public class OrderHistory:BaseEntityAudited
{
    public Guid OrderId { get; set; }
    public string Content { get; set; }
    public EHistoryType Type { get; set; }
    
    public virtual Order Order { get; set; }
}