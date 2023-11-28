using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Order.Application
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        }
    }
}
