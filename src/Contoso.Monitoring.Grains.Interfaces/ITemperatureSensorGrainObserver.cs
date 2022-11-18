namespace Contoso.Monitoring.Grains
{
    public interface ITemperatureSensorGrainObserver : IGrainObserver
    {
        Task OnTemperatureReadingReceived(TemperatureSensor reading);
    }
}
