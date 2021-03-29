using System;
using System.Threading.Tasks;
using Contoso.Monitoring.Grains.Interfaces;
using Microsoft.Extensions.Logging;

namespace Contoso.Monitoring.Grains
{
    public class TemperatureSensorGrain : Orleans.Grain, ITemperatureSensorGrain
    {
        public TemperatureSensorGrain(ILogger<TemperatureSensorGrain> logger)
        {
            this.Logger = logger;
        }

        public ILogger<TemperatureSensorGrain> Logger { get; }

        public Task ReceiveTemperatureReading(TemperatureReading temperatureReading)
        {
            Logger.LogInformation($"Received {temperatureReading.Fahrenheit} from client {temperatureReading.SensorName} at {DateTime.UtcNow}.");
            return Task.FromResult(true);
        }
    }
}