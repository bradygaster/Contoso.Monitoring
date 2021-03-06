using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contoso.Monitoring.Grains.Interfaces;
using Microsoft.Extensions.Logging;
using Orleans;

namespace Contoso.Monitoring
{
    public class ContosoMonitoringClientService
    {
        public IClusterClient Client { get; private set; }
        private ILogger<ContosoMonitoringClientService> _logger;

        public ContosoMonitoringClientService(ILogger<ContosoMonitoringClientService> logger)
        {
            _logger = logger;

            try
            {
                Client = new ClientBuilder().UseLocalhostClustering().Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to cluster");
                throw;
            }
        }

        public Task Stop(CancellationToken cancellationToken)
        {
            var cancellation = new TaskCompletionSource<bool>();
            cancellationToken.Register(() => cancellation.TrySetCanceled(cancellationToken));
            return Task.WhenAny(Client.Close(), cancellation.Task);
        }

        public async Task Connect(CancellationToken cancellationToken)
        {
            var retries = 100;

            await Client.Connect(async error =>
            {
                if (--retries < 0)
                {
                    _logger.LogError("Could not connect to the cluster: {@Message}", error.Message);
                    return false;
                }
                else
                {
                    _logger.LogWarning(error, "Error Connecting: {@Message}", error.Message);
                }

                try
                {
                    await Task.Delay(1000, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return false;
                }

                return true;
            });
        }

        public async Task<List<MonitoredArea>> GetMonitoredAreas()
        {
            return await Client.GetGrain<IMonitoredBuildingGrain>(Guid.Empty).GetMonitoredAreas();
        }
    }
}