namespace ConnectionStrings
{

    //an attribute applied to the DatabaseServiceAttribute class. It specifies that DatabaseServiceAttribute can only be applied to classes.
    //AttributeUsage is a predefined attribute that controls how the custom attribute can be used.
    //now an adapter class can be decorated with a database connection attribute
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DatabaseServiceAttribute : Attribute
    {
        /// <summary>
        /// Gets the connection string key name
        /// </summary>
        /// <param name="connectionStringName"></param>
        public DatabaseServiceAttribute(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }

        public string ConnectionStringName { get; }

    }
    public sealed class ConnectionString
    {
        public const string local = "localConnection";
    }

}
