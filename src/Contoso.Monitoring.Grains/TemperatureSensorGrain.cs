using Contoso.Monitoring.Grains.Interfaces;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;

namespace Contoso.Monitoring.Grains
{
    public class TemperatureSensorGrain : Orleans.Grain, ITemperatureSensorGrain
    {
        private ILogger<TemperatureSensorGrain> _logger;
        private IPersistentState<TemperatureSensorGrainState> _temperatureSensorGrainState;
        private IBuildingDashboardGrain _buildingDashboardGrain;

        public TemperatureSensorGrain(ILogger<TemperatureSensorGrain> logger,
            [PersistentState(nameof(TemperatureSensorGrain))] IPersistentState<TemperatureSensorGrainState> temperatureSensorGrainState)
        {
            _logger = logger;
            _temperatureSensorGrainState = temperatureSensorGrainState;
            _buildingDashboardGrain = GrainFactory.GetGrain<IBuildingDashboardGrain>(Guid.Empty);
        }

        public Task<TemperatureReading> GetTemperature() =>
            _temperatureSensorGrainState.State.Readings.Any()
                ? Task.FromResult(_temperatureSensorGrainState.State.Readings.Last())
                : null;

        public async Task ReceiveTemperatureReading(TemperatureReading temperatureReading)
        {
            await _buildingDashboardGrain.UpdateBuildingDashboard(temperatureReading);
            _logger.LogInformation($"Received {temperatureReading.Fahrenheit} from client {temperatureReading.SensorName} at {temperatureReading.Timestamp}.");
            _temperatureSensorGrainState.State.Readings.Add(temperatureReading);
            _logger.LogInformation($"Temperature sensor {temperatureReading.SensorName} currently has {_temperatureSensorGrainState.State.Readings.Count} records.");
        }
    }

    [GenerateSerializer]
    public class TemperatureSensorGrainState
    {
        [Id(0)]
        public List<TemperatureReading> Readings { get; set; } = new List<TemperatureReading>();
    }
}