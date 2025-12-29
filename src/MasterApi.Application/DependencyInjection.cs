using Microsoft.Extensions.DependencyInjection;

namespace MasterApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Aqui se registrarian los servicios de la capa de aplicacion, como MediatR, AutoMapper, etc.
        return services;
    }
}
