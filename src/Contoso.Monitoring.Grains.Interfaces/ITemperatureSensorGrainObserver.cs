namespace Contoso.Monitoring.Grains.Interfaces
{
    public interface ITemperatureSensorGrainObserver : IGrainObserver
    {
        Task OnTemperatureReadingReceived(TemperatureReading reading);
    }
}
