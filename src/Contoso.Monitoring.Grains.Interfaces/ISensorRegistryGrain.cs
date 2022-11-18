namespace Contoso.Monitoring.Grains
{
    public interface ISensorRegistryGrain : IGrainWithGuidKey
    {
        Task<TemperatureSensor> GetSensorReading(string areaName);
        Task<List<TemperatureSensor>> GetSensors();
        Task Subscribe(ITemperatureSensorReceivedReadingObserver observer);
        Task Unsubscribe(ITemperatureSensorReceivedReadingObserver observer);
    }
}