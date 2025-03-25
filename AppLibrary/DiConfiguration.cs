using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AppLibrary
{
    public class DiConfiguration
    {
        public static void ConfigureServices(IServiceCollection services, Settings settings)
        {
            // Check if Settings is already registered
            // Prevents the Settings object from being registered more than once to avoid dupliate registrations
            if (services.Any(sd => sd.ServiceType == typeof(Settings))) return;

            // Add HttpClient services
            // Adds support for HttpClient which can be used for making HTTP requests throughout the application
            services.AddHttpClient();

            // Register Settings as a singleton
            // Singleton. This ensures that the same instance of Settings is used throughout the application
            services.AddSingleton(settings);

            // Get the assembly containing the DiConfiguration class
            var assembly = typeof(DiConfiguration).GetTypeInfo().Assembly;

            // Register FluentValidation validators from the assembly
            // FluentValidation looks for classes in the specified assembly that implement IValidator<T>, where T is the type being validated.
            // These validators are then registered with the DI container
            services.AddValidatorsFromAssembly(assembly);

            // Register services from the assembly
            services.RegisterServices(assembly, settings);
        }
    }
}
