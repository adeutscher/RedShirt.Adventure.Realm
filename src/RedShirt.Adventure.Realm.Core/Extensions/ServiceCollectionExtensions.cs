using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedShirt.Adventure.Realm.Core.Services;

namespace RedShirt.Adventure.Realm.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureApiCore(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddSingleton<IExampleItemService, ExampleItemService>();
    }
}