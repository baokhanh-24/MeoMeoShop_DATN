using System.Globalization;
using MeoMeo.PORTAL.Components;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Services;
using MeoMeo.Shared.Utilities;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
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
builder.Services.AddInteractiveStringLocalizer();
builder.Services.AddHttpClient<IApiCaller, ApiCaller>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl!);
});
// đăng kí 
builder.Services.AddScoped<ICustomerClientService, CustomerClientService>();
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