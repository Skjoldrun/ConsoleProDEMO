using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Reflection;

namespace ConsoleProDEMO.Utilities
{
    public class AppSettingsAppConfigConverter : IAppSettingsAppConfigConverter
    {
        private readonly IConfiguration _config;
        private readonly ILogger<AppSettingsAppConfigConverter> _logger;

        public AppSettingsAppConfigConverter(IConfiguration config, ILogger<AppSettingsAppConfigConverter> logger = null)
        {
            _config = config;
            _logger = logger ?? NullLogger<AppSettingsAppConfigConverter>.Instance;
        }

        public void OverwriteConfigFromAppSettings()
        {
            // Use this for projects with app.config settings still available
            //try
            //{
            //    foreach (PropertyInfo prop in typeof(Properties.Settings).GetProperties())
            //    {
            //        if (prop.Name == "Default")
            //        {
            //            continue;
            //        }
            //        string appSettingsConnString = _config.GetConnectionString(prop.Name);
            //        if (string.IsNullOrWhiteSpace(appSettingsConnString) == false)
            //        {
            //            Properties.Settings.Default[prop.Name] = appSettingsConnString;
            //            _logger.LogDebug("Change connString {Name} to {New}", prop.Name, appSettingsConnString);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Error while overwriting settings connStrings from AppSettings: {Message}", ex.Message);
            //    throw;
            //}
        }
    }
}