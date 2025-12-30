using System.Reflection;
using Microsoft.Extensions.Localization;

namespace MasterApi.Api.Services;

public class LocalizationService
{
    private readonly IStringLocalizer _localizer;

    public LocalizationService(IStringLocalizerFactory factory)
    {
        var type = typeof(Resources.ErrorMessages);
        var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName).Name;
        _localizer = factory.Create("ErrorMessages", assemblyName);
    }

    public string GetString(string key)
    {
        return _localizer[key];
    }
}
