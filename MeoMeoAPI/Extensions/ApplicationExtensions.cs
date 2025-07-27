using MeoMeo.Shared.Middlewares;

namespace MeoMeo.API.Extensions;

public static class ApplicationExtensions
{
    public static void UseInfrastructure(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            context.Response.Headers["Cache-Control"] = "no-store";
            context.Response.Headers["Pragma"] = "no-cache";
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            await next();
        });

        app.UseMiddleware<BasicAuthMiddleware>();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MEOMEO API v1"));
        app.UseStaticFiles();
        app.UseCors("corsapp");
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        // app.UseMiddleware<CustomAuthorizeMiddleware>(); // Temporarily disabled for testing
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
        });
    }
}
