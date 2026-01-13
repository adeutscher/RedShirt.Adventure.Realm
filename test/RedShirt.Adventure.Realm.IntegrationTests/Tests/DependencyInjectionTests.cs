using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedShirt.Adventure.Realm.Attributes;
using RedShirt.Adventure.Realm.Extensions;
using System.Reflection;

namespace RedShirt.Adventure.Realm.IntegrationTests.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void Controller_DependencyInjection_Test()
    {
        /*
         * Note: Referencing ServiceCollectionExtensions because it is a decently static
         *      class that we're about to reference a method from.
         *
         * Run cold, the assembly we're after wouldn't show up in `AppDomain.CurrentDomain.GetAssemblies()`.
         */
        var controllerClasses = Assembly.GetAssembly(typeof(ProducesJsonAttribute))
            !.DefinedTypes
            .Where(t =>
                t != typeof(ControllerBase)
                && t.IsAssignableTo(typeof(ControllerBase)))
            .ToList();

        // Sanity-check our test's seeking
        Assert.NotEmpty(controllerClasses);

        var configuration = new ConfigurationBuilder().Build();

        var serviceCollection = new ServiceCollection()
            // Trust boilerplate ASPNETCore setup to add logging under normal circumstances
            .AddLogging();

        TestUtilities.WrapEnvironment(new Dictionary<string, string>
        {
            ["AWS_SERVICE_URL"] = "http://localhost:8000",
            ["AWS_ACCESS_KEY_ID"] = "foo",
            ["AWS_SECRET_ACCESS_KEY"] = "bar",
            ["AWS_SESSION_TOKEN"] = "foobar"
        }, () =>
        {
            serviceCollection.ConfigureApiServices(configuration);

            foreach (var controllerType in controllerClasses)
            {
                // Unclear on why, but need to declare our type as an implementation of itself
                serviceCollection.AddSingleton(controllerType, controllerType);
            }

            var provider = serviceCollection.BuildServiceProvider();

            foreach (var controllerType in controllerClasses)
            {
                // Confirm that we can build each controller
                var service = provider.GetService(controllerType);
                Assert.NotNull(service);

                foreach (var method in controllerType.GetMethods())
                {
                    foreach (var parameter in method.GetParameters()
                                 .Where(parameter => parameter.GetCustomAttributes<FromServicesAttribute>().Any()))
                    {
                        provider.GetRequiredService(parameter.ParameterType);
                    }
                }
            }
        });
    }
}