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

    public class MonitoredArea
    {
        public string Name { get; set; }
        public TemperatureReading Temperature { get; set; }
    }
}