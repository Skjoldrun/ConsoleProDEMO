using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConsoleProDEMO.Utilities
{
    public class ConfigManager : IConfigManager
    {
        private readonly ILogger<ConfigManager> _logger;

        /// <summary>
        /// Switch between using enum string names or int keys
        /// </summary>
        public bool ConvertEnumNames { get; set; } = true;

        /// <summary>
        /// Reads the json text from the given file , stores it and can deserialze with generic type.
        /// </summary>
        /// <param name="logger">logger instance, will be set as NullLogger if null</param>
        public ConfigManager(ILogger<ConfigManager> logger = null)
        {
            _logger = logger ?? NullLogger<ConfigManager>.Instance;
        }

        /// <summary>
        /// Reads and deserializes a given json config file to an instnace of T.
        /// </summary>
        /// <typeparam name="T">Type of config to be deserialized from json</typeparam>
        /// <param name="fileName">filename of the config file to be read</param>
        /// <returns>Instance of T from config</returns>
        public T DeserializeConfig<T>(string fileName)
        {
            T config = DeserializeConfig<T>(fileName, ConvertEnumNames);
            _logger.LogDebug("Deserialized {typeName} from {configFileJson}", typeof(T).Name, fileName);

            return config;
        }

        /// <summary>
        /// Old method for non DI calls.
        /// Reads and deserializes a given json config file to an instnace of T.
        /// </summary>
        /// <typeparam name="T">Type of config to be deserialized from json</typeparam>
        /// <param name="fileName">filename of the config file to be read</param>
        /// <param name="convertEnumNames">convert ENUM Values to string names instead of int values</param>
        /// <returns>Instance of T from config</returns>
        public static T DeserializeConfig<T>(string fileName, bool convertEnumNames = true)
        {
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", fileName);
            var configFileJson = File.ReadAllText(configPath);

            T config;
            if (convertEnumNames)
            {
                var options = new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
                };

                config = JsonSerializer.Deserialize<T>(configFileJson, options);
            }
            else
            {
                config = JsonSerializer.Deserialize<T>(configFileJson);
            }

            return config;
        }
    }
}