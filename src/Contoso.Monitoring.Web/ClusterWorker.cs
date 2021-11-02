using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Contoso.Monitoring.Web
{
    public class ClusterWorker : IHostedService
    {
        private readonly ContosoMonitoringClientService _clusterService;

        public ClusterWorker(ContosoMonitoringClientService clusterService)
        {
            _clusterService = clusterService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _clusterService.Connect(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _clusterService.Stop(cancellationToken);
        }
    }
}
