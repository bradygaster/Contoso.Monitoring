using System;

namespace Contoso.Monitoring.Grains.Interfaces
{
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
        public static double ToCelsius(double fahrenheit)
        {
            var celsius = (fahrenheit - 32) * 5 / 9;
            return celsius;
        }

        public static double ToFahrenheit(double celsius)
        {
            var fahrenheit = (celsius * 9) / 5 + 32;
            return fahrenheit;
        }
    }
}