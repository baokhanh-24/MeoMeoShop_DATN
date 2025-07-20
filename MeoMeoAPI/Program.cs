using System.Globalization;
using MeoMeo.Application.IServices;
using MeoMeo.Application.Services;
using MeoMeo.Contract;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using MeoMeo.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using MeoMeo.Domain.Commons;
using MeoMeo.EntityFrameworkCore.Commons;

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

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductServices, ProductService>();
builder.Services.AddScoped<IProductsDetailRepository, ProductsDetailRepository>();
builder.Services.AddScoped<IProductDetailServices, ProductDetailService>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICartDetaillRepository, CartDetaillRepository>();
builder.Services.AddScoped<ICartDetaillService, CartDetaillService>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IColourRepository, ColourRepository>();
builder.Services.AddScoped<IColourService, ColourService>();
builder.Services.AddScoped<IProductDetaillColourRepository, ProductDetaillColourRepository>();
builder.Services.AddScoped<IProductDetaillColourService, ProductDetaillColourService>();
builder.Services.AddScoped<IProductDetaillSizeRepository, ProductDetaillSizeRepository>();
builder.Services.AddScoped<IProductDetaillSizeService, ProductDetaillSizeService>();
builder.Services.AddScoped<ISizeRepository, SizeRepository>();
builder.Services.AddScoped<ISizeService, SizeService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();
builder.Services.AddScoped<IDeliveryAddressRepository, DeliveryAddressRepository>();
builder.Services.AddScoped<IDeliveryAddressService, DeliveryAddressService>();
builder.Services.AddScoped<IProvinceRepository, ProvinceRepository>();
builder.Services.AddScoped<IProvinceService, ProvinceService>();
builder.Services.AddScoped<IIventoryBatchReposiory, InventoryBatchRepository>();
builder.Services.AddScoped<IInventoryBatchServices, InventoryBatchService>();
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
builder.Services.AddScoped<IOrderDetailInventoryBatchRepository, OrderDetailInventoryBatchRepository>();
builder.Services.AddScoped<IOrderHistoryRepository, OrderHistoryRepository>();
// builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeServices, EmployeeServices>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerServices, CustomerServices>();
builder.Services.AddScoped<IResetPasswordHistoryRepository, ResetPasswordHistoryRepository>();
builder.Services.AddScoped<IResetPasswordHistoryServices, ResetPasswordHistoryServices>();
builder.Services.AddScoped<IBankRepository, BankRepository>();
builder.Services.AddScoped<IBankService, BankServices>();
builder.Services.AddScoped<ICustomersBankRepository, CustomersBankRepository>();
builder.Services.AddScoped<ICustomersBankServices, CustomersBankServices>();
builder.Services.AddScoped<IDistrictRepository, DistrictRepository>();
builder.Services.AddScoped<IDistrictService, DistrictService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
builder.Services.AddScoped<IMaterialServices, MaterialServices>();
builder.Services.AddScoped<ISystemConfigRepository, SystemConfigRepository>();
builder.Services.AddScoped<ISystemConfigService, SystemConfigService>();


builder.Services.AddAutoMapper(typeof(MeoMeoAutoMapperProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
