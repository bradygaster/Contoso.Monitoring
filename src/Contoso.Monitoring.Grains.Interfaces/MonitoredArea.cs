namespace Contoso.Monitoring.Grains.Interfaces
{
    [GenerateSerializer]
    public class MonitoredArea
    {
        [Id(0)]
        public string Name { get; set; }
        [Id(1)]
        public TemperatureReading Temperature { get; set; }
    }
}
