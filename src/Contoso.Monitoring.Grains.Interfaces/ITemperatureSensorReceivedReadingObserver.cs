namespace Contoso.Monitoring.Grains
{
    public interface ITemperatureSensorReceivedReadingObserver : IGrainObserver
    {
        Task OnTemperatureReadingReceived(TemperatureSensor reading);
    }
}
