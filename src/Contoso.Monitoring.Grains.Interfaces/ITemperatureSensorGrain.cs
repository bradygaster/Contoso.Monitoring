namespace Contoso.Monitoring.Grains.Interfaces
{
    public interface ITemperatureSensorGrain : IGrainWithStringKey
    {
        Task ReceiveTemperatureReading(TemperatureReading temperatureReading);
        Task<TemperatureReading> GetTemperature();
        Task Subscribe(ITemperatureSensorGrainObserver observer);
        Task Unsubscribe(ITemperatureSensorGrainObserver observer);
    }

    public static class TemperatureReadingConverter
    {
        public static double ToCelsius(double fahrenheit) => (fahrenheit - 32) * 5 / 9;

        public static double ToFahrenheit(double celsius) => (celsius * 9) / 5 + 32;
    }
}