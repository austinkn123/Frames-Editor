using FluentValidation;
using System.Reflection;

namespace FramesEditor.Server.Site
{
    public class DiConfiguration
    {
        /// <summary>
        /// Set up services throughout the application
        /// </summary>
        /// <param name="services"></param>
        /// <param name="settings"></param>
        public static void ConfigureServices(IServiceCollection services, Settings settings)
        {
            //This allows for a consistent DI setup across different projects or layers of your application
            AppLibrary.DiConfiguration.ConfigureServices(services, settings);

            //Making Settings available as a singleton ensures that the same configuration settings are used wherever they are needed within the application
            services.AddSingleton(settings);

            //An assembly is a compiled code library in .NET that can be used for deployment, versioning, and security.
            //It contains code in the form of Intermediate Language (IL) and metadata about the code, such as types and their relationships.
            //Assemblies are fundamental units of deployment and versioning in .NET
            //An assembly is typically a .dll (Dynamic Link Library) or .exe (Executable) file.
            //It can contain one or more namespaces, classes, interfaces, and other types.


            //Retrieves the assembly (a compiled code library) where the DiConfiguration class is defined
            //This is useful for dynamically discovering and registering services based on the assembly where DiConfiguration resides
            var assembly = typeof(DiConfiguration).GetTypeInfo().Assembly;

            // Register services from the assembly
            services.RegisterServices(assembly, settings);
        }
    }
}
