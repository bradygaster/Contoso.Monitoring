using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contoso.Monitoring.Grains.Interfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;

namespace Contoso.Monitoring.Grains
{
    public class TemperatureSensorGrain : Orleans.Grain, ITemperatureSensorGrain
    {
        private ILogger<TemperatureSensorGrain> _logger;
        private IPersistentState<TemperatureSensorGrainState> _temperatureSensorGrainState;
        private IBuildingDashboardGrain _buildingDashboardGrain;
        private IGrainFactory _grainFactory;

        public TemperatureSensorGrain(ILogger<TemperatureSensorGrain> logger,
            [PersistentState("temperatureSensorGrainState", "contosoMonitoringStore")]
            IPersistentState<TemperatureSensorGrainState> temperatureSensorGrainState,
            IGrainFactory grainFactory)
        {
            _logger = logger;
            _temperatureSensorGrainState = temperatureSensorGrainState;
            _grainFactory = grainFactory;
            _buildingDashboardGrain = _grainFactory.GetGrain<IBuildingDashboardGrain>(Guid.Empty);
        }

        public Task<TemperatureReading> GetTemperature()
        {
            if (_temperatureSensorGrainState.State.Readings.Any())
            {
                return Task.FromResult(_temperatureSensorGrainState.State.Readings.Last());
            }

            return null;
        }

        public async Task ReceiveTemperatureReading(TemperatureReading temperatureReading)
        {
            await _buildingDashboardGrain.UpdateBuildingDashboard(temperatureReading);
            _logger.LogInformation($"Received {temperatureReading.Fahrenheit} from client {temperatureReading.SensorName} at {temperatureReading.Timestamp}.");
            _temperatureSensorGrainState.State.Readings.Add(temperatureReading);
            _logger.LogInformation($"Temperature sensor {temperatureReading.SensorName} currently has {_temperatureSensorGrainState.State.Readings.Count} records.");
        }
    }

    [Serializable]
    public class TemperatureSensorGrainState
    {
        public List<TemperatureReading> Readings { get; set; } = new List<TemperatureReading>();
    }
}