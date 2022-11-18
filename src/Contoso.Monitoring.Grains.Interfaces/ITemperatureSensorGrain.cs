namespace Contoso.Monitoring.Grains
{
    public interface ITemperatureSensorGrain : IGrainWithStringKey
    {
        Task ReceiveTemperatureReading(TemperatureSensor temperatureReading);
        Task<TemperatureSensor> GetTemperature();
        Task Subscribe(ITemperatureSensorReceivedReadingObserver observer);
        Task Unsubscribe(ITemperatureSensorReceivedReadingObserver observer);
        Task ListenForRequests(ITemperatureSensorReceiveRequestObserver observer);
        Task StopListeningForRequests();
    }

    public static class TemperatureReadingConverter
    {
        public static double ToCelsius(this double fahrenheit) => (fahrenheit - 32) * 5 / 9;

        public static double ToFahrenheit(this double celsius) => (celsius * 9) / 5 + 32;
    }
}