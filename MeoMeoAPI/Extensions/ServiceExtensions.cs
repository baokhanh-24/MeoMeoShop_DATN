using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using MeoMeo.Shared.Utilities;
using MeoMeo.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;


namespace MeoMeo.API.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddPolicyMiddleWare(configuration);
        return services;
    }
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.ConfigureSwagger();
        services.AddCors(p => p.AddPolicy("corsapp", builder =>
        {
            builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
        }));
        services.AddLocalization();
        services.AddDistributedMemoryCache();
        services.AddMemoryCache();
        return services;
    }

    public static IServiceCollection AddPolicyMiddleWare(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
        services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.BackchannelHttpHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("Jwt:key").Value)),
                    ValidIssuer = configuration.GetSection("Jwt:Issuer").Value,
                    ValidAudience = configuration.GetSection("Jwt:Audience").Value
                };
                o.UseSecurityTokenValidators = true;
                o.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var dbContext = context.HttpContext.RequestServices.GetRequiredService<MeoMeoDbContext>();
                        var userId = context.Principal?.FindFirst(ClaimTypeConst.UserId)
                                     ?? context.Principal?.FindFirst(ClaimTypes.NameIdentifier)
                                     ?? context.Principal?.FindFirst("id");
                        var token = context.SecurityToken as JwtSecurityToken;
                        if (userId == null || token == null)
                        {
                            context.Fail("Unauthorized");
                            return;
                        }
                        var userToken = await dbContext.userTokens.FirstOrDefaultAsync(ut =>
                            ut.AccessToken == token.RawData && ut.ExpiryDate > DateTime.Now);
                        if (userToken == null || userToken.IsRevoked)
                        {
                            context.Fail("Unauthorized");
                        }   
                       
                    }
                 };
              


            });
       services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy =>
                policy.Requirements.Add(new PermissionRequirement(new[] { "ADMIN.ADMIN" }, requiresAll: false)));
            options.AddPolicy("Customer", policy =>
                policy.Requirements.Add(new PermissionRequirement(new[] { "CUSTOMER.CUSTOMER" }, requiresAll: false))); 
           
            options.AddPolicy("AdminOrCustomer", policy =>
            {
                policy.Requirements.Add(new PermissionRequirement(new[] { "ADMIN.ADMIN", "CUSTOMER.CUSTOMER" }, requiresAll: false));
                
            });
            
        });
        return services;
    }
    private static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "MEOMEO API", Version = "v1" });
            options.CustomSchemaIds(type => type.FullName);
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = HeaderNames.Authorization,
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });
            options.AddSecurityDefinition("basic", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "basic",
                Description = "Input your username and password to access this API"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "basic"
                        }
                    },
                    new string[] {}
                }
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new List<string>()
                    }
                });
        });
    }
}
