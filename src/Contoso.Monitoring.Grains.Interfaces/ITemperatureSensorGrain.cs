using System;
using System.Threading.Tasks;

namespace Contoso.Monitoring.Grains.Interfaces
{
    public interface ITemperatureSensorGrain : Orleans.IGrainWithStringKey
    {
        Task ReceiveTemperatureReading(TemperatureReading temperatureReading);
        Task<TemperatureReading> GetTemperature();
    }

    [Serializable]
    public class TemperatureReading
    {
        public string SensorName { get; set; }
        public double Fahrenheit { get; set; }
        public double Celsius { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public static class TemperatureReadingConverter
    {
        public static double ToCelsius(double fahrenheit) => (fahrenheit - 32) * 5 / 9;

        public static double ToFahrenheit(double celsius) => (celsius * 9) / 5 + 32;
    }
}