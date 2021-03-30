using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contoso.Monitoring.Grains.Interfaces;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;

namespace Contoso.Monitoring.Grains
{
    public class TemperatureSensorGrain : Orleans.Grain, ITemperatureSensorGrain
    {
        private ILogger<TemperatureSensorGrain> _logger;
        private IPersistentState<TemperatureSensorGrainState> _temperatureSensorGrainState;

        public TemperatureSensorGrain(ILogger<TemperatureSensorGrain> logger,
            [PersistentState("temperatureSensorGrainState", "contosoMonitoringStore")] 
                IPersistentState<TemperatureSensorGrainState> temperatureSensorGrainState)
        {
            _logger = logger;
            _temperatureSensorGrainState = temperatureSensorGrainState;
        }

        public Task ReceiveTemperatureReading(TemperatureReading temperatureReading)
        {
            _logger.LogInformation($"Received {temperatureReading.Fahrenheit} from client {temperatureReading.SensorName} at {temperatureReading.Timestamp}.");
            _temperatureSensorGrainState.State.Readings.Add(temperatureReading);
            _logger.LogInformation($"Temperature sensor {temperatureReading.SensorName} currently has {_temperatureSensorGrainState.State.Readings.Count} records.");
            return Task.FromResult(true);
        }
    }

    [Serializable]
    public class TemperatureSensorGrainState
    {
        public List<TemperatureReading> Readings { get; set; } = new List<TemperatureReading>();
    }
}