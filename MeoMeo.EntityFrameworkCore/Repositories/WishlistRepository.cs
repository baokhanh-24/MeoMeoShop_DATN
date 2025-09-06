using MeoMeo.Domain.Entities;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class WishlistRepository : BaseRepository<Wishlist>
    {
        public WishlistRepository(MeoMeoDbContext dbContext) : base(dbContext)
        {
        }
    }
}


