using Contoso.Monitoring.Grains;
using Microsoft.AspNetCore.SignalR;

namespace Contoso.Monitoring.Web
{
    public class BuildingMonitorHub : Hub<ITemperatureSensorReceivedReadingObserver>
    {
    }
}