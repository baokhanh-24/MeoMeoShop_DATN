using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeoMeo.EntityFrameworkCore.Configurations.Contexts
{
    public class MeoMeoDbContext : DbContext
    {
        public MeoMeoDbContext(DbContextOptions options) : base(options)
        {
        }
        
        public DbSet<Brand> brands { get; set; }
        public DbSet<Cart> carts { get; set; }
        public DbSet<CartDetail> cartDetails { get; set; }
        public DbSet<Colour> colours { get; set; }
        public DbSet<Customers> customers { get; set; }
        public DbSet<DeliveryAddress> deliveryAddresses { get; set; }
        public DbSet<Employee> employees { get; set; }
        public DbSet<Image> images { get; set; }
        public DbSet<ImportBatch> importBatches { get; set; }
        public DbSet<InventoryBatch> inventoryBatches { get; set; }
        public DbSet<InventoryTransaction> inventoryTransactions { get; set; }
        public DbSet<Material> materials { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<OrderDetail> orderDetails { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<ProductDetail> productDetails { get; set; }
        public DbSet<ProductMaterial> productMaterials { get; set; }
        public DbSet<ProductSeason> productSeasons { get; set; }
        public DbSet<Promotion> promotions { get; set; }
        public DbSet<PromotionDetail> promotionDetails { get; set; }
        public DbSet<Season> seasons { get; set; }
        public DbSet<Size> sizes { get; set; }
        public DbSet<SystemConfig> systemConfigs { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<Voucher> vouchers { get; set; }
        public DbSet<OrderDetailInventoryBatch> orderDetailInventoryBatches { get; set; }
        public DbSet<OrderHistory> orderHistories { get; set; }
        public DbSet<OrderReturn> orderReturns { get; set; }
        public DbSet<OrderReturnItem> orderReturnItems { get; set; }
        public DbSet<OrderReturnFile> orderReturnFiles { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<ProductCategory> productCategories { get; set; }
        public DbSet<ProductReview> productReviews { get; set; }
        public DbSet<Role> roles { get; set; }
        public DbSet<UserRole> userRoles { get; set; }
        public DbSet<UserToken> userTokens { get; set; }
        public DbSet<Wishlist> wishlists { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new BrandConfiguration());
            modelBuilder.ApplyConfiguration(new CartConfiguration());
            modelBuilder.ApplyConfiguration(new CartDetailConfiguration());
            modelBuilder.ApplyConfiguration(new ColourConfiguration());
            modelBuilder.ApplyConfiguration(new CustomersConfiguration());
            modelBuilder.ApplyConfiguration(new DeliveryAddressConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new ImageConfiguration());
            modelBuilder.ApplyConfiguration(new ImportBatchConfiguration());
            modelBuilder.ApplyConfiguration(new InventoryBatchConfiguration());
            modelBuilder.ApplyConfiguration(new InventoryTransactionConfiguration());
            modelBuilder.ApplyConfiguration(new MaterialConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderDetailConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new ProductDetailConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new ProductSeasonConfiguration());
            modelBuilder.ApplyConfiguration(new PromotionConfiguration());
            modelBuilder.ApplyConfiguration(new PromotionDetailConfiguration());
            modelBuilder.ApplyConfiguration(new SeasonConfiguration());
            modelBuilder.ApplyConfiguration(new SizeConfiguration());
            modelBuilder.ApplyConfiguration(new SystemConfigConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new VoucherConfiguration());
            modelBuilder.ApplyConfiguration(new OrderDetailInventoryBatchConfiguration());
            modelBuilder.ApplyConfiguration(new OrderHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new OrderReturnConfiguration());
            modelBuilder.ApplyConfiguration(new OrderReturnItemConfiguration());
            modelBuilder.ApplyConfiguration(new OrderReturnFileConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ProductCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserTokenConfiguration());
            modelBuilder.ApplyConfiguration(new ProductMaterialConfiguration());
            modelBuilder.ApplyConfiguration(new ProductReviewConfiguration());
            modelBuilder.ApplyConfiguration(new WishlistConfiguration());

        }
    }
}
