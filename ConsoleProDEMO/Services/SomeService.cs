using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ConsoleProDEMO.Services
{
    public class SomeService : ISomeService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SomeService> _logger;

        public SomeService(IConfiguration config, ILogger<SomeService> logger)
        {
            _config = config;
            _logger = logger ?? NullLogger<SomeService>.Instance;
        }

        public async Task Run()
        {
            _logger.LogInformation("Hello World!");
            _logger.LogInformation("The current environment is {DOTNET_ENVIRONMENT}", Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production");
        }
    }
}