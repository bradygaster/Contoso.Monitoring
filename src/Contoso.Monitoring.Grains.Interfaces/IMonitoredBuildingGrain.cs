namespace Contoso.Monitoring.Grains.Interfaces
{
    public interface IMonitoredBuildingGrain : Orleans.IGrainWithGuidKey
    {
        Task<MonitoredArea> GetMonitoredArea(string areaName);
        Task MonitorArea(string areaName);
        Task<List<MonitoredArea>> GetMonitoredAreas();
    }
}