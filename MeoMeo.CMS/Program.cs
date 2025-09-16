using MeoMeo.CMS.Components;
using MeoMeo.CMS.IServices;
using MeoMeo.CMS.Services;
using MeoMeo.CMS.Utilities;
using MeoMeo.Shared.IServices;
using System.Globalization;
using MeoMeo.Shared.Services;
using MeoMeo.Shared.Utilities;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

// Configure data protection
if (builder.Environment.IsDevelopment())
{
    // For development: persist keys to file system
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "DataProtection-Keys")))
        .SetDefaultKeyLifetime(TimeSpan.FromDays(90));
}
else
{
    // For production: use in-memory keys with longer lifetime
    builder.Services.AddDataProtection()
        .SetDefaultKeyLifetime(TimeSpan.FromDays(30));
}

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddAntDesign();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddRadzenComponents();
builder.Services.AddSignalR(e =>
{
    e.MaximumReceiveMessageSize = 102400000;
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue;
});
builder.Services.AddScoped<MessageModalService>();
builder.Services.AddSingleton<LoadingService>();

// Authentication Services for CMS
builder.Services.AddScoped<ICMSAuthService, CMSAuthService>();
builder.Services.AddScoped<ICMSUserInfoService, CMSUserInfoService>();
builder.Services.AddScoped<CMSAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, CMSAuthStateProvider>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddHttpClient<IApiCaller, ApiCaller>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl!);
});
builder.Services.AddAuthorizationCore();
CultureInfo culture = new CultureInfo("vi-VN");
CultureInfo.DefaultThreadCurrentUICulture = culture;
// đăng kí 
builder.Services.AddScoped<ICustomerClientService, CustomerClientService>();
builder.Services.AddScoped<IProductClientService, ProductClientService>();
builder.Services.AddScoped<IBankClientService, BankClientService>();
builder.Services.AddScoped<IStatisticsClientService, StatisticsClientService>();
builder.Services.AddScoped<IInventoryBatchClientService, InventoryBatchClientService>();
builder.Services.AddScoped<IImportBatchClientService, ImportBatchClientService>();
builder.Services.AddScoped<ISizeClientService, SizeClientService>();
builder.Services.AddScoped<IMaterialClientService, MaterialClientService>();
builder.Services.AddScoped<IVoucherClientService, VoucherClientService>();
builder.Services.AddScoped<IBrandClientService, BrandClientService>();
builder.Services.AddScoped<ISeasonClientService, SeasonClientService>();
builder.Services.AddScoped<IColourClientService, ColourClientService>();
builder.Services.AddScoped<ICategoryClientService, CategoryClientService>();
builder.Services.AddScoped<IEmployeesClientService, EmployeesClientService>();
builder.Services.AddScoped<IOrderClientService, OrderClientService>();
builder.Services.AddScoped<IPromotionClientService, PromotionClientService>();
builder.Services.AddScoped<IPromotionDetailClientService, PromotionDetailClientService>();
builder.Services.AddScoped<ISystemConfigClientService, SystenConfigClientService>();
builder.Services.AddScoped<IGhnClientService, GhnClientService>();
builder.Services.AddScoped<IDeliveryAddressClientService, DeliveryAddressClientService>();
// Permission services
builder.Services.AddScoped<IPermissionClientService, PermissionClientService>();
builder.Services.AddScoped<IPermissionGroupClientService, PermissionGroupClientService>();
builder.Services.AddScoped<IRoleClientService, RoleClientService>();
builder.Services.AddScoped<IUserRoleClientService, UserRoleClientService>();
builder.Services.AddScoped<IUserProfileClientService, UserProfileClientService>();
builder.Services.AddScoped<IProductReviewClientService, ProductReviewClientService>();
builder.Services.AddHttpClient<IApiCaller, ApiCaller>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl!);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();