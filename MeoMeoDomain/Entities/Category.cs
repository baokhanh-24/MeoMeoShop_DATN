using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities;

public class Category:BaseEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool Status { get; set; }
    public virtual ICollection<ProductCategory> ProductCategories { get; set; }
}