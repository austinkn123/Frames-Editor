//using Insight.Database;
using Dapper;
using Npgsql;
using System.Data.Common;

namespace App.Utils
{
    public static class DbAdapterFactory
    {
        /// <summary>
        /// Creates and returns an instance of a type T that is configured with the provided connection string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static T GetConnectionAs<T>(string connectionString) where T : DbConnection
        {
            // Use reflection to create an instance using the constructor that accepts a string.
            return (T)Activator.CreateInstance(typeof(T), connectionString);
        }

    }
}