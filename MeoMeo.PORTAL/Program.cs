using System.Globalization;
using MeoMeo.PORTAL.Components;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Middlewares;
using MeoMeo.Shared.Services;
using MeoMeo.Shared.Utilities;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddAntDesign();
builder.Services.AddScoped<MessageModalService>();
builder.Services.AddSingleton<LoadingService>();
builder.Services.AddRadzenComponents();
builder.Services.AddSignalR(e => {
    e.MaximumReceiveMessageSize = 102400000;
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue;
});
// Authentication Services
builder.Services.AddScoped<IAuthClientService, AuthClientService>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddHttpClient<IApiCaller, ApiCaller>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl!);
});
builder.Services.AddAuthorizationCore();
CultureInfo culture = new CultureInfo("vi-VN");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;
builder.Services.AddInteractiveStringLocalizer();

// đăng kí 
builder.Services.AddScoped<ICustomerClientService, CustomerClientService>();
builder.Services.AddScoped<IProductClientService, ProductClientService>();
builder.Services.AddScoped<ICartClientService, CartClientService>();
builder.Services.AddScoped<ISizeClientService, SizeClientService>();
builder.Services.AddScoped<IColourClientService, ColourClientService>();
builder.Services.AddScoped<IVoucherClientService, VoucherClientService>();
builder.Services.AddScoped<ICommuneClientService, CommuneClientService>();
builder.Services.AddScoped<IDistrictClientService, DistrictClientService>();
builder.Services.AddScoped<IProvinceClientService, ProvinceClientService>();
builder.Services.AddScoped<IDeliveryAddressClientService, DeliveryAddressClientService>();
builder.Services.AddScoped<IOrderClientService, OrderClientService>();
builder.Services.AddScoped<IProductReviewClientService, ProductReviewClientService>();
builder.Services.AddScoped<IGhnService, GhnService>();

//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.ListenAnyIP(80); 
//});
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