namespace Contoso.Monitoring.Grains.Interfaces
{
    public interface IMonitoredBuildingGrain : IGrainWithGuidKey
    {
        Task<MonitoredArea> GetMonitoredArea(string areaName);
        Task MonitorArea(string areaName);
        Task<List<MonitoredArea>> GetMonitoredAreas();
        Task Subscribe(ITemperatureSensorGrainObserver observer);
        Task Unsubscribe(ITemperatureSensorGrainObserver observer);
    }
}