using Contoso.Monitoring.Grains;

namespace Contoso.Monitoring.Sensors.Temperature
{
    public interface ITemperatureSensorClient
    {
        Task<TemperatureSensor> GetTemperatureReading();
    }
}