using MeoMeo.CMS.Components;
using MeoMeo.Shared.IServices;
using System.Globalization;
using MeoMeo.Shared.Services;
using MeoMeo.Shared.Utilities;

var builder = WebApplication.CreateBuilder(args);

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddAntDesign();
builder.Services.AddScoped<MessageModalService>();
builder.Services.AddSingleton<LoadingService>();
builder.Services.AddHttpClient<IApiCaller, ApiCaller>();
CultureInfo culture = new CultureInfo("vi-VN");
CultureInfo.DefaultThreadCurrentUICulture = culture;
// đăng kí 
builder.Services.AddScoped<ICustomerClientService, CustomerClientService>();
builder.Services.AddScoped<IProductDetailClientService, ProductDetailClientService>();
builder.Services.AddScoped<IProductClientService, ProductClientService>();
builder.Services.AddScoped<IBankClientService, BankClientService>();
builder.Services.AddScoped<IInventoryBatchClientService, InventoryBatchClientService>();
builder.Services.AddScoped<ISizeClientService, SizeClientService>();
builder.Services.AddScoped<IMaterialClientService, MaterialClientService>();
builder.Services.AddScoped<IVoucherClientService, VoucherClientService>();
builder.Services.AddScoped<IBrandClientService, BrandClientService>();
builder.Services.AddScoped<ISeasonClientService, SeasonClientService>();
builder.Services.AddScoped<IColourClientService, ColourClientService>();
builder.Services.AddScoped<ICategoryClientService, CategoryClientService>();
builder.Services.AddScoped<IEmployeesClientService, EmployeesClientService>();
builder.Services.AddScoped<IOrderClientService,OrderClientService>();
builder.Services.AddScoped<IPromotionClientService, PromotionClientService>();
builder.Services.AddScoped<IPromotionDetailClientService, PromotionDetailClientService>();
builder.Services.AddScoped<ISystemConfigClientService, SystenConfigClientService>();
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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();