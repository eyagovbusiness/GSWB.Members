using Members.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Members.Infrastructure.Services
{
    public class StartupHostedService : IHostedService
    {
        private readonly ILogger<StartupHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupHostedService"/> class.
        /// </summary>
        /// <param name="aLogger">The logger instance.</param>
        /// <param name="aServiceProvider">The service provider to retrieve services.</param>
        public StartupHostedService(ILogger<StartupHostedService> aLogger, IServiceProvider aServiceProvider)
        {
            _logger = aLogger;
            _serviceProvider = aServiceProvider;
        }

        public async Task StartAsync(CancellationToken aCancellationToken)
        {
            try
            {
                _logger.LogInformation("StartupHostedService started.Synchronizing roles with Discord...");
                using var lScope = _serviceProvider.CreateScope();
                var lRolesInfrastructureService = lScope.ServiceProvider.GetRequiredService<IRolesInfrastructureService>();
                //await lRolesInfrastructureService.SyncRolesWithDiscordAsync(aCancellationToken);
                _logger.LogInformation("StartupHostedService finished. Roles with Discord synchronized.");
            }
            catch (Exception lEx)
            {
                _logger.LogError(lEx, "An error occurred during the StartupHostedService. Roles with Discord were not synchronized.");
            }
        }

        public Task StopAsync(CancellationToken aCancellationToken)
            => Task.CompletedTask;
    }
}
