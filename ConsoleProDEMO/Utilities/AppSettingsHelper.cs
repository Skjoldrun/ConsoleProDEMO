using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace ConsoleProDEMO.Utilities
{
    public static class AppSettingsHelper
    {
        private const string AspNetVarVarName = "ASPNETCORE_ENVIRONMENT";
        private const string DotNetEnvVarName = "DOTNET_ENVIRONMENT";
        private const string AppSettingsSectionName = "AppSettings";

        private static IConfiguration? _configuration;

        /// <summary>
        /// Gets the active environment variable name used for determining the environment.
        /// </summary>
        private static string GetEnvVarName()
        {
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(AspNetVarVarName)))
                return AspNetVarVarName;

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(DotNetEnvVarName)))
                return DotNetEnvVarName;

            return string.Empty;
        }

        /// <summary>
        /// Builds and caches the configuration. Reloads only if explicitly reset.
        /// </summary>
        public static IConfiguration GetConfiguration()
        {
            if (_configuration != null)
                return _configuration;

            var envVarName = GetEnvVarName();
            var environment = string.IsNullOrWhiteSpace(envVarName)
                ? null
                : Environment.GetEnvironmentVariable(envVarName);

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            if (!string.IsNullOrEmpty(environment))
                builder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

            builder.AddUserSecrets<Program>();
            builder.AddEnvironmentVariables();
            _configuration = builder.Build();

            return _configuration;
        }

        /// <summary>
        /// Retrieves a value of type T from the "AppSettings" configuration section by key.
        /// Returns the specified default value if the key does not exist or the section is missing.
        /// </summary>
        /// <typeparam name="T">The expected type of the configuration value.</typeparam>
        /// <param name="key">The key name within the AppSettings section.</param>
        /// <param name="defaultValue">The default value to return if the key is not found.</param>
        /// <returns>The configuration value associated with the key, or the default value.</returns>
        public static T? GetValue<T>(string key, T? defaultValue = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                return defaultValue;

            var section = GetConfiguration().GetSection(AppSettingsSectionName);

            if (!section.Exists())
                return defaultValue;

            var value = section.GetValue<T>(key);

            return value != null ? value : defaultValue;
        }

        /// <summary>
        /// Gets a connection string by key.
        /// </summary>
        public static string GetConnectionString(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var connStr = GetConfiguration()
                .GetConnectionString(key);

            return connStr ?? string.Empty;
        }

        /// <summary>
        /// Manually resets the cached configuration (e.g., for unit testing or reload).
        /// </summary>
        public static void ResetConfiguration()
        {
            _configuration = null;
        }
    }
}