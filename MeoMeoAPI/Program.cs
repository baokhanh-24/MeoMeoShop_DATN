using System.Globalization;
using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using MeoMeo.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using MeoMeo.API.Extensions;
using MeoMeo.Domain.Commons;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.Shared.Middlewares;
using MeoMeo.Shared.VnPay;
using MeoMeo.Shared.VnPay.Models;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var culture = new CultureInfo("vi-VN");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;
builder.Services.AddDbContext<MeoMeoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("MeoMeo.API")));
builder.Services.AddConfigurationSettings(builder.Configuration);
builder.Services.AddInfrastructure();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductServices, ProductService>();
builder.Services.AddScoped<IProductsDetailRepository, ProductsDetailRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICartDetaillRepository, CartDetaillRepository>();
builder.Services.AddScoped<ICartDetaillService, CartDetaillService>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IColourRepository, ColourRepository>();
builder.Services.AddScoped<IColourService, ColourService>();
builder.Services.AddScoped<ISizeRepository, SizeRepository>();
builder.Services.AddScoped<ISizeService, SizeService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();
builder.Services.AddScoped<IDeliveryAddressRepository, DeliveryAddressRepository>();
builder.Services.AddScoped<IDeliveryAddressService, DeliveryAddressService>();
builder.Services.AddScoped<IIventoryBatchReposiory, InventoryBatchRepository>();
builder.Services.AddScoped<IInventoryBatchServices, InventoryBatchService>();
builder.Services.AddScoped<IImportBatchRepository, ImportBatchRepository>();
builder.Services.AddScoped<IImportBatchServices, ImportBatchService>();
builder.Services.AddScoped<IInventoryTranSactionRepository, InventoryTranSactionRepository>();
builder.Services.AddScoped<IIventoryTranSactionServices, InventoryTranSactionService>();
builder.Services.AddScoped<IProductSeasonRepository, ProductSeasonRepository>();
builder.Services.AddScoped<IProductSeasonServices, ProductSeasonService>();
builder.Services.AddScoped<ISeasonRepository, SeasonRepository>();
builder.Services.AddScoped<ISeasonService, SeasonService>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IBrandServices, BrandService>();
builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();
builder.Services.AddScoped<IPromotionServices, PromotionServices>();
builder.Services.AddScoped<IPromotionDetailRepository, PromotionDetailRepository>();
builder.Services.AddScoped<IPromotionDetailServices, PromotionDetailServices>();
builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();
builder.Services.AddScoped<IVoucherService, VoucherServices>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductReviewService, ProductReviewService>();
builder.Services.AddScoped<IOrderDetailInventoryBatchRepository, OrderDetailInventoryBatchRepository>();
builder.Services.AddScoped<IOrderHistoryRepository, OrderHistoryRepository>();
builder.Services.AddScoped<IProductMaterialRepository, ProductMaterialRepository>();
// builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeServices, EmployeeServices>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerServices, CustomerServices>();
builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
builder.Services.AddScoped<IMaterialServices, MaterialServices>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IInventoryStatisticsService, InventoryStatisticsService>();
builder.Services.AddScoped<IOrderReturnService, OrderReturnService>();
builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
builder.Services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
builder.Services.AddScoped<IOrderReturnRepository, OrderReturnRepository>();
builder.Services.AddScoped<IOrderReturnItemRepository, OrderReturnItemRepository>();
builder.Services.AddScoped<IOrderReturnFileRepository, OrderReturnFileRepository>();
builder.Services.AddScoped<IProductReviewFileRepository, ProductReviewFileRepository>();
builder.Services.AddScoped<IOrderReturnService, OrderReturnService>();
// Role services
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<IWishlistService, WishlistService>();

// Email Services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IExcelReportService, ExcelReportService>();


// Auth Service Dependencies
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IUserTokenRepository, UserTokenRepository>();
builder.Services.Configure<PaymentOptions>(builder.Configuration.GetSection(PaymentOptions.SectionName));
builder.Services.AddSingleton<IVnpay, Vnpay>();
// Auth Service
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(MeoMeoAutoMapperProfile));
builder.Services.AddControllers(options =>
{
    options.Filters.Add<TrimStringsFilter>();
});
var app = builder.Build();
app.UseInfrastructure();
app.Run();
