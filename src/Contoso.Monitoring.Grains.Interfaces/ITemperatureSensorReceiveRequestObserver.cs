using Contoso.Monitoring.Grains;

namespace Contoso.Monitoring.Grains
{
    public interface ITemperatureSensorReceiveRequestObserver : IGrainObserver
    {
        Task<TemperatureSensor> GetTemperatureReading();
    }
}