using MasterApi.Application.Abstractions;
using MasterApi.Application.Abstractions.Authentication;
using MasterApi.Application.Abstractions.Persistence;
using MasterApi.Infrastructure.Authentication;
using MasterApi.Infrastructure.Persistence;
using MasterApi.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MasterApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLocalization();
        services.AddSingleton<ILocalizationService, LocalizationService>();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddSingleton<IJwtProvider, JwtProvider>();
        services.AddSingleton<IPasswordHasherService, PasswordHasherService>();

        services.AddSingleton<IUserRepository, UserRepository>();

        return services;
    }
}

