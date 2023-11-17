using Discount.Api.Services;
using Discount.Infrastructure.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using WebFramework.Configuration.Swagger;

namespace WebFramework.Configuration
{
    public static class WebApplicationExtensions
    {
        public static WebApplication Configure(this WebApplication app)
        {
            var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();

            try
            {
                // If you're in a development environment, you might want to see detailed exception information.
                if (app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                    app.UseSwaggerAndUI();
                }
                 
                app.UseHttpsRedirection();
                app.UseRouting();
                app.UseCors();
                //app.UseAuthentication();
                app.UseStaticFiles();
                //app.UseAuthorization();

                app.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStarted.Register(() =>
                {
                    app.MigrateDatabase<DiscountService>(); // Use a service related to your database context.
                });
                 
                // Endpoints mapping comes after Authentication and Authorization.
                app.MapControllers();

                app.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGrpcService<DiscountService>();
                    endpoints.MapGet("/", async context =>
                    {
                        await context.Response.WriteAsync(
                            "Communication with gRPC endpoints must be made through a gRPC client.");
                    });
                });
                return app;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }
    }
}
