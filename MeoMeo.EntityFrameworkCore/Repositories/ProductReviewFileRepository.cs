using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;

namespace MeoMeo.EntityFrameworkCore.Repositories;

public class ProductReviewFileRepository : BaseRepository<ProductReviewFile>, IProductReviewFileRepository
{
    public ProductReviewFileRepository(MeoMeoDbContext context) : base(context)
    {
    }
}