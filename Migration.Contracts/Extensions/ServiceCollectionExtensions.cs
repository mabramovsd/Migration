using Microsoft.Extensions.DependencyInjection;
using Migration.Contracts.Http;

namespace Migration.Contracts.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCorrelationIdSupport(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<CorrelationIdHandler>();
            return services;
        }
    }
}
