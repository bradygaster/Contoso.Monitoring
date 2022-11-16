using Contoso.Monitoring.Grains.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Contoso.Monitoring.Web
{
    public class BuildingDashboardGrain : Grain, IBuildingDashboardGrain
    {
        ILogger<BuildingDashboardGrain> _logger;
        IHubContext<BuildingMonitorHub, IBuildingMonitorClient> _hub;

        public BuildingDashboardGrain(ILogger<BuildingDashboardGrain> logger,
            IHubContext<BuildingMonitorHub, IBuildingMonitorClient> hub)
        {
            _logger = logger;
            _hub = hub;
        }

        public async Task UpdateBuildingDashboard(TemperatureReading reading)
        {
            _logger.LogInformation($"{reading.SensorName} reports {reading.Celsius}C/{reading.Fahrenheit}F");
            await _hub.Clients.All.ReceiveUpdate(reading);
        }
    }
}