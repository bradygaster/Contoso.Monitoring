using Contoso.Monitoring.Grains.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Contoso.Monitoring.Web
{
    public class BuildingMonitorHub : Hub<ITemperatureSensorGrainObserver>
    {
    }
}