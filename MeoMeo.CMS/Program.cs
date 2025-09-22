using MeoMeo.CMS.Components;
using MeoMeo.CMS.IServices;
using MeoMeo.CMS.Services;
using MeoMeo.CMS.Utilities;
using MeoMeo.Shared.IServices;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MeoMeo.Shared.Constants;
using MeoMeo.Shared.Services;
using MeoMeo.Shared.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];


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

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.BackchannelHttpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt:key").Value)),
            ValidIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value,
            ValidAudience = builder.Configuration.GetSection("Jwt:Audience").Value,
            RoleClaimType = "roles",
            NameClaimType = "userName"
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("Customer", policy =>
        policy.RequireRole("Customer"));

    options.AddPolicy("AdminOrCustomer", policy =>
        policy.RequireRole("Admin", "Customer"));
});
builder.Services.AddCascadingAuthenticationState();
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
CultureInfo culture = new CultureInfo("vi-VN");
CultureInfo.DefaultThreadCurrentUICulture = culture;
// đăng kí 
builder.Services.AddScoped<ICustomerClientService, CustomerClientService>();
builder.Services.AddScoped<IProductClientService, ProductClientService>();
builder.Services.AddScoped<IBankClientService, BankClientService>();
builder.Services.AddScoped<IStatisticsClientService, StatisticsClientService>();
builder.Services.AddScoped<IInventoryBatchClientService, InventoryBatchClientService>();
builder.Services.AddScoped<IInventoryStatisticsClientService, InventoryStatisticsClientService>();
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

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery(); 

app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();