using App.Utils;
using AppLibrary;
using ConnectionStrings;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DiExtensions
    {
        /// <summary>
        /// Custom extension method for the IServiceCollection interface in dotnet. 
        /// It dynamically registers services with various lifetimes based on attributes applied to types
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <param name="settings"></param>
        /// 
        public static void RegisterServices(this IServiceCollection services, Assembly assembly, Settings settings)
        {
            var PostgreSQLServerFactory = typeof(DbAdapterFactory).GetMethod(nameof(DbAdapterFactory.GetConnectionAs));

            // Loops through all types in the provided assembly to determine how each type should be registered with the DI container
            // Uses reflection to get all types exported by the assembly
            foreach (var type in assembly.GetExportedTypes())
            {
                // Checks if the type has the TransientServiceAttribute attribute and registers it with AddTransient
                // Like in the usecase attributes
                // Dp the same for singleton and scoped
                if (type.GetCustomAttribute<TransientServiceAttribute>() != null)
                {
                    services.AddTransient(type);
                }
                else if (type.GetCustomAttribute<SingletonServiceAttribute>() != null)
                {
                    services.AddSingleton(type);
                }
                else if (type.GetCustomAttribute<ScopedServiceAttribute>() != null)
                {
                    services.AddScoped(type);
                }
                // Registers transient services to types that are related to database connections
                // Like for the adapter attributes
                else if (type.GetCustomAttribute<DatabaseServiceAttribute>(true) is DatabaseServiceAttribute databaseServiceAttribute)
                {
                    services.AddTransient(type, (serviceProvider) =>
                    {
                        // Checks for the DatabaseServiceAttribute attribute and uses reflection to invoke the
                        // GetConnectionAs method from DbAdapterFactory to create instances of the service, passing in the appropriate connection string
                        var connectionString = settings.DatabaseConnectionStrings[databaseServiceAttribute.ConnectionStringName];

                        var genericMethod = PostgreSQLServerFactory.MakeGenericMethod(type);
                        return genericMethod.Invoke(null, new object[] { connectionString });
                    });
                }
            }
        }
    }
}