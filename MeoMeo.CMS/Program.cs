using MeoMeo.CMS.Components;
using MeoMeo.CMS.IServices;
using MeoMeo.CMS.Services;
using MeoMeo.Utilities;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddAntDesign();
builder.Services.AddScoped<MessageModalService>();
builder.Services.AddSingleton<LoadingService>();
builder.Services.AddHttpClient<IApiCaller, ApiCaller>();
CultureInfo culture = new CultureInfo("vi-VN");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;
// đăng kí 
builder.Services.AddScoped<ICustomerClientService, CustomerClientService>();
builder.Services.AddScoped<IProductDetailClientService, ProductDetailClientService>();
builder.Services.AddScoped<IBankClientService, BankClientService>();
builder.Services.AddScoped<IInventoryBatchClientService, InventoryBatchClientService>();

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