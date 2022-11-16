using Contoso.Monitoring.Grains.Interfaces;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;

namespace Contoso.Monitoring.Grains
{
    public class TemperatureSensorGrain : Grain, ITemperatureSensorGrain
    {
        private ILogger<TemperatureSensorGrain> _logger;
        private IPersistentState<TemperatureSensorGrainState> _temperatureSensorGrainState;
        private HashSet<ITemperatureSensorGrainObserver> _observers = new();

        public TemperatureSensorGrain(ILogger<TemperatureSensorGrain> logger,
            [PersistentState(nameof(TemperatureSensorGrain))] IPersistentState<TemperatureSensorGrainState> temperatureSensorGrainState)
        {
            _logger = logger;
            _temperatureSensorGrainState = temperatureSensorGrainState;
        }

        public Task<TemperatureReading> GetTemperature() =>
            _temperatureSensorGrainState.State.Readings.Any()
                ? Task.FromResult(_temperatureSensorGrainState.State.Readings.Last())
                : null;

        public async Task ReceiveTemperatureReading(TemperatureReading temperatureReading)
        {
            _logger.LogInformation($"Received {temperatureReading.Fahrenheit} from client {temperatureReading.SensorName} at {temperatureReading.Timestamp}.");
            _temperatureSensorGrainState.State.Readings.Add(temperatureReading);
            _logger.LogInformation($"Temperature sensor {temperatureReading.SensorName} currently has {_temperatureSensorGrainState.State.Readings.Count} records.");

            foreach (var observer in _observers)
            {
                await observer.OnTemperatureReadingReceived(temperatureReading);
            }
        }

        public Task Subscribe(ITemperatureSensorGrainObserver observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
            return Task.CompletedTask;
        }

        public Task Unsubscribe(ITemperatureSensorGrainObserver observer)
        {
            if (_observers.Contains(observer))
                _observers.Remove(observer);
            return Task.CompletedTask;
        }
    }

    [GenerateSerializer]
    public class TemperatureSensorGrainState
    {
        [Id(0)]
        public List<TemperatureReading> Readings { get; set; } = new List<TemperatureReading>();
    }
}