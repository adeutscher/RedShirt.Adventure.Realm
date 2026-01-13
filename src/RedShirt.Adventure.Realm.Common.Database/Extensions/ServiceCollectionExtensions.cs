using Amazon.SimpleSystemsManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedShirt.Adventure.Realm.Common.Aws.Extensions;
using RedShirt.Adventure.Realm.Common.Database.Services;

namespace RedShirt.Adventure.Realm.Common.Database.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfigurationRoot configuration)
    {
        var connectionStringPath = configuration["MYSQL_CONNECTION_STRING_PATH"];

        return services
            .AddAwsServiceWithLocalSupport<IAmazonSimpleSystemsManagement>()
            .AddSingleton<ISqlConnectionFactory>(provider =>
                new SqlConnectionFactory(
                    provider.GetRequiredService<IAmazonSimpleSystemsManagement>(),
                    connectionStringPath
                ));
    }

    public static IServiceCollection AddGenericDtoHandler<TDto, TKey>(this IServiceCollection services)
        where TDto : class
    {
        return services
            .AddSingleton<IGenericDtoStorage<TDto, TKey>, GenericDtoStorage<TDto, TKey>>();
    }
}