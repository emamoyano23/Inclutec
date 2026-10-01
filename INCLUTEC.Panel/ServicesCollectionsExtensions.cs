using INCLUTEC.Panel.Infrastructure;
using INCLUTEC.Services.Services;

namespace INCLUTEC.Panel
{
    public static class ServicesCollectionsExtensions
    {
        public static IServiceCollection AddServicesCollections(this IServiceCollection services, IConfiguration configuration)
        {
            var baseUrl = configuration["ApiSettings:baseUrl"];

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException("La clave 'ApiSettings:baseUrl' no está configurada en appsettings.json.");
            }

            services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            });
            services.AddScoped<IRegistroAsistenciaService, RegistroAsistenciaService>();
            services.AddScoped<IAulaService, AulaService>();
            return services;
        }
    }
}