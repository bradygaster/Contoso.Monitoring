namespace Contoso.Monitoring.Sensors.Temperature;

public interface ITemperatureSensorClient
{
    Task<TemperatureSensor> GetTemperatureReading();
}