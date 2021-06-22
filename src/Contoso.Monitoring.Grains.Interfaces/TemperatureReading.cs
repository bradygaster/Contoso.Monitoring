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
}