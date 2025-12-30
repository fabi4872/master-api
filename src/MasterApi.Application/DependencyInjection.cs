using MasterApi.Application.Abstractions.Services;
using MasterApi.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MasterApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        
        return services;
    }
}
