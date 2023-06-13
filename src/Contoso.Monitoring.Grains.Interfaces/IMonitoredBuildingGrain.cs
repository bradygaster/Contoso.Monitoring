using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contoso.Monitoring.Grains.Interfaces
{
    public interface IMonitoredBuildingGrain : Orleans.IGrainWithGuidKey
    {
        Task<MonitoredArea> GetMonitoredArea(string areaName);
        Task MonitorArea(string areaName);
        Task<List<MonitoredArea>> GetMonitoredAreas();
    }

    [GenerateSerializer]
    public class MonitoredArea
    {
        [Id(0)]
        public string Name { get; set; }
        [Id(1)]
        public TemperatureSensor Temperature { get; set; }
    }
}