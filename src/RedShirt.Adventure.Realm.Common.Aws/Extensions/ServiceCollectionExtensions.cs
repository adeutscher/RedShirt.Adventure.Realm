using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Microsoft.Extensions.DependencyInjection;

namespace RedShirt.Adventure.Realm.Common.Aws.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAwsServiceWithLocalSupport<TService>(this IServiceCollection services)
        where TService : IAmazonService
    {
        var url = Environment.GetEnvironmentVariable("AWS_SERVICE_URL");
        if (string.IsNullOrWhiteSpace(url))
        {
            return services
                .AddAWSService<TService>();
        }

        // Note: S3 needs a special carve-out for AmazonS3Config.ForcePathStyle that is not needed here.

        Console.WriteLine($"Using AWS service URL for {typeof(TService)}: {url}");

        return services.AddAWSService<TService>(new AWSOptions
        {
            DefaultClientConfig =
            {
                ServiceURL = url,
                AuthenticationRegion = Environment.GetEnvironmentVariable("AWS_REGION")
            }
        });
    }
}