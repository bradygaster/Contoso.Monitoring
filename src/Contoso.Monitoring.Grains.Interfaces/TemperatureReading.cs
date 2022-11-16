namespace Contoso.Monitoring.Grains.Interfaces
{
    [GenerateSerializer]
    public class TemperatureReading
    {
        [Id(0)]
        public string SensorName { get; set; }
        [Id(1)]
        public double Fahrenheit { get; set; }
        [Id(2)]
        public double Celsius { get; set; }
        [Id(3)]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}