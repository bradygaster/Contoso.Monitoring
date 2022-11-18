using Contoso.Monitoring.Grains;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics.CodeAnalysis;

namespace Contoso.Monitoring.Web
{
    public class TemperatureSensorReceivedReadingObserver : ITemperatureSensorReceivedReadingObserver
    {
        protected string MachineName = Environment.MachineName;
        private IHubContext<BuildingMonitorHub, ITemperatureSensorReceivedReadingObserver> _hub;
        private ILogger<TemperatureSensorReceivedReadingObserver> _logger;

        public TemperatureSensorReceivedReadingObserver(IHubContext<BuildingMonitorHub, ITemperatureSensorReceivedReadingObserver> hub, 
            ILogger<TemperatureSensorReceivedReadingObserver> logger)
        {
            _hub = hub;
            _logger = logger;
        }

        public bool Equals(ITemperatureSensorReceivedReadingObserver x, ITemperatureSensorReceivedReadingObserver y)
            => (x as TemperatureSensorReceivedReadingObserver).MachineName.ToLower()
                .Equals((y as TemperatureSensorReceivedReadingObserver).MachineName.ToLower());

        public int GetHashCode([DisallowNull] ITemperatureSensorReceivedReadingObserver obj)
            => (obj as TemperatureSensorReceivedReadingObserver).MachineName.ToLower().GetHashCode();

        public Task OnTemperatureReadingReceived(TemperatureSensor reading)
        {
            _hub.Clients.All.OnTemperatureReadingReceived(reading);
            return Task.CompletedTask;
        }
    }
}
