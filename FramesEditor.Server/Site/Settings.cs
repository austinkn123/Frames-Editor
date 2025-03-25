using AppLibrary.DiConfigs;

namespace FramesEditor.Server.Site
{
    /// <summary>
    /// inherits from AppLibrary's settings
    /// </summary>
    public class Settings : AppLibrary.Settings
    {
        //Takes in the builder configuration from Program.cs that we passed in
        /// <summary>
        /// Inherit from AppLibrary Settings to initalize settings. 
        /// </summary>
        /// <param name="configuration"></param>
        /// IConfiguration Parameter: Represents the configuration source (such as appsettings.json or environment variables).
        /// Base is from App Library's constructor
        public Settings(IConfiguration configuration) : base(configuration)
        {
            configuration.SetProps(this);
        }
    }
}
