using MasterApi.Application.Abstractions;
using MasterApi.Application.Resources;
using Microsoft.Extensions.Localization;
using System.Reflection;

namespace MasterApi.Infrastructure.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IStringLocalizer _localizer;

        public LocalizationService(IStringLocalizerFactory factory)
        {
            var type = typeof(ErrorMessages);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName!).Name!;
            _localizer = factory.Create(nameof(ErrorMessages), assemblyName);
        }

        public string GetString(string key)
        {
            return _localizer[key];
        }
    }
}
