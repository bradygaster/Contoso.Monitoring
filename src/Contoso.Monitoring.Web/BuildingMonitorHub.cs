using System.Threading.Tasks;
using Contoso.Monitoring.Grains.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Contoso.Monitoring.Web
{
    public class BuildingMonitorHub : Hub<IBuildingMonitorClient>
    {

    }

    public interface IBuildingMonitorClient
    {
        Task ReceiveUpdate(TemperatureReading reading);
    }
}