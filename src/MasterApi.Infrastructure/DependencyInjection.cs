using MasterApi.Application.Abstractions;
using MasterApi.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MasterApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Aqui se registrarian los servicios de la capa de infraestructura, como el DbContext, repositorios, etc.
        services.AddLocalization();
        services.AddSingleton<ILocalizationService, LocalizationService>();

        return services;
    }
}
