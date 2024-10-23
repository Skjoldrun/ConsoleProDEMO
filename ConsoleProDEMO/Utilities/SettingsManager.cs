using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConsoleProDEMO.Utilities
{
    public class SettingsManager : ISettingsManager
    {
        private readonly ILogger<SettingsManager> _logger;

        /// <summary>
        /// Switch between using enum string names or int keys
        /// </summary>
        public bool ConvertEnumNames { get; set; } = true;

        public SettingsManager(ILogger<SettingsManager> logger = null)
        {
            _logger = logger ?? NullLogger<SettingsManager>.Instance;
        }

        /// <summary>
        /// Reads from the given path.
        /// Default path is C:\ProgramData\<CompanyName>\EntryAssemblyName\Settings.json
        /// </summary>
        /// <typeparam name="T">Type of settings to be deserialized from json</typeparam>
        /// <param name="path">path with full name of the settings file to be read</param>
        /// <returns>Instance of T from config</returns>
        public T ReadFromFile<T>(string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = Path.Combine(
                    AppDataCommonPathProcessor.GetProgramDataCompanyFolderPath(),
                    AssemblyInfoProcessor.GetEntryAssemblyName(),
                    "Settings.json");
            }

            if (File.Exists(path) == false)
            {
                throw new Exception($"No settings file found at {path}!");
            }

            string json = File.ReadAllText(path);
            T settings;

            if (ConvertEnumNames)
            {
                // options for converting ENUM Names instead of keys
                var options = new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
                };

                settings = JsonSerializer.Deserialize<T>(json, options);
            }
            else
            {
                settings = JsonSerializer.Deserialize<T>(json);
            }

            _logger.LogDebug("Deserialized {typeName} from {configFileJson}", typeof(T).Name, json);
            return settings;
        }

        public void SaveToFile<T>(T settings, string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = Path.Combine(
                AppDataCommonPathProcessor.GetProgramDataCompanyFolderPath(),
                AssemblyInfoProcessor.GetEntryAssemblyName(),
                "Settings.json");
            }

            string json;
            if (ConvertEnumNames)
            {
                var options = new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                    WriteIndented = true
                };

                json = JsonSerializer.Serialize(settings, options);
            }
            else
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                json = JsonSerializer.Serialize(settings, options);
            }

            File.WriteAllText(path, json);
            _logger.LogDebug("Serialized content as json to file: {path}", path);
        }
    }
}