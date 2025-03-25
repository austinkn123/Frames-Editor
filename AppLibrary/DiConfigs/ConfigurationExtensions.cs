using Microsoft.Extensions.Configuration;
using System.Reflection;

//This file leverages reflection to populate properties of an object with values from configuration
//and includes additional utility methods for handling configurations.
//reflection is a feature in .NET that allows code to inspect and interact with object types and members (such as properties, methods, and fields) at runtime

//this is an extension of the Microsoft.Extensions.Configuration namespace, providing additional functionality.
namespace AppLibrary.DiConfigs
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Retrieves a configuration section as a strongly typed object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="config"></param>
        /// <param name="name"></param>
        /// Retrieves the section with the specified name.
        /// If the section exists, converts it to type T.
        /// Returns null if the section does not exist.
        public static T? GetSectionAs<T>(this IConfiguration config, string name) where T : class
        {
            var section = config.GetSection(name);
            if (section.Exists())
            {
                return section.Get<T>();
            }

            return null;
        }

        /// <summary>
        /// Purpose: Retrieves a string configuration value from two potential locations
        /// </summary>
        /// <param name="config"></param>
        /// <param name="name"></param>
        /// <param name="def"></param>
        /// First looks for Values:{name} (used in some local settings).
        /// If not found, looks for name directly(used in deployed settings).
        /// Returns the default value if neither is found.
        public static string FromValuesOrRoot(this IConfiguration config, string name, string def = null)
        {
            // look for values:name (local function app or settings), then name (deployed function app)
            return config[$"Values:{name}"] ?? config[name] ?? def;
        }

        /// <summary>
        /// Retrieves a configuration value of a specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="config"></param>
        /// <param name="name"></param>
        /// <param name="def"></param>
        /// Retrieves the value of type T from the configuration.
        /// Looks in Values:{name} and then name.
        /// Returns the default value if neither is found.
        public static T FromValuesOrRoot<T>(this IConfiguration config, string name, T def)
        {
            // look for values:name (local function app or settings), then name (deployed function app)
            return config.GetValue<T>($"Values:{name}") ?? config.GetValue<T>(name) ?? def;
        }

        /// <summary>
        /// Executes an action on each item in an enumerable collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="action"></param>
        /// Iterates through the collection, applying the given action to each item
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T item in enumerable)
            {
                action(item);
            }
            return enumerable;
        }

        /// <summary>
        /// Uses reflection to set properties of an object based on configuration values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="target"></param>
        /// <param name="propsToSkip"></param>
        /// Retrieves public, instance properties declared directly on the type T.
        /// Using IConfiguration to configure objects means that you can leverage configuration settings (stored in files, environment variables, etc.) 
        /// to automatically populate the properties of classes in your application. 
        /// This approach enhances modularity, maintainability, and flexibility by keeping 
        /// configuration logic separate from your application code and allowing for dynamic configuration at runtime.
        public static void SetProps<T>(this IConfiguration configuration, T target, params string[] propsToSkip)
        {
            // only grab properties declared on the type
            typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(prop => !propsToSkip.Contains(prop.Name))
                .ForEach(prop =>
                {
                    if (prop.SetMethod == null) return; // readonly property

                    var propType = prop.PropertyType;
                    if (propType == typeof(string))
                        prop.SetMethod.Invoke(target, new object[] { configuration.FromValuesOrRoot(prop.Name) });
                    else if (propType == typeof(bool))
                        prop.SetMethod.Invoke(target, new object[] { EmptyToFalse(configuration.FromValuesOrRoot(prop.Name)) });
                    else if (propType == typeof(string[]))
                        prop.SetMethod.Invoke(target, new object[] { Split(configuration.FromValuesOrRoot(prop.Name)) });
                    else if (propType == typeof(int))
                        prop.SetMethod.Invoke(target, new object[] { configuration.FromValuesOrRoot(prop.Name, 0) });
                    else if (propType == typeof(double))
                        prop.SetMethod.Invoke(target, new object[] { configuration.FromValuesOrRoot(prop.Name, 0.0) });
                    else if (propType == typeof(Dictionary<string, string>))
                        prop.SetMethod.Invoke(target, new object[] { configuration.GetSectionAs<Dictionary<string, string>>(prop.Name) ?? new() });
                    else if (propType == typeof(Dictionary<string, int>))
                        prop.SetMethod.Invoke(target, new object[] { configuration.GetSectionAs<Dictionary<string, int>>(prop.Name) ?? new() });
                });
        }

        private static string[] Split(string s) => string.IsNullOrEmpty(s) ? Array.Empty<string>() : s.Split(',', StringSplitOptions.RemoveEmptyEntries);

        // configuration.GetValue uses the default if the key is missing, not if it is empty
        private static bool EmptyToFalse(string s) => !string.IsNullOrEmpty(s) && Convert.ToBoolean(s);

    }
}