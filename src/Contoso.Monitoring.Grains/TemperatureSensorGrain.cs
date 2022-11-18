using Microsoft.Extensions.Logging;
using Orleans.Runtime;

namespace Contoso.Monitoring.Grains
{ 
    public class TemperatureSensorGrain : Grain, ITemperatureSensorGrain
    {
        private ILogger<TemperatureSensorGrain> _logger;
        private IPersistentState<TemperatureSensorGrainState> _temperatureSensorGrainState;
        private HashSet<ITemperatureSensorReceivedReadingObserver> _observers = new();
        private IDisposable _timer;
        private ITemperatureSensorReceiveRequestObserver _temperatureSensorReceiveRequestObserver;

        public TemperatureSensorGrain(ILogger<TemperatureSensorGrain> logger,
            [PersistentState(nameof(TemperatureSensorGrain))] IPersistentState<TemperatureSensorGrainState> temperatureSensorGrainState)
        {
            _logger = logger;
            _temperatureSensorGrainState = temperatureSensorGrainState;
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            var grainId = this.GetGrainId();
            var sensorName = grainId.Key.ToString();
            var lastKnown = await GrainFactory.GetGrain<ISensorRegistryGrain>(Guid.Empty).GetSensorReading(sensorName);

            _timer?.Dispose();
            _timer = RegisterTimer(async _ => await RequestReadingFromClient(), null, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100));

            await base.OnActivateAsync(cancellationToken);
        }

        public Task<TemperatureSensor> GetTemperature() =>
            _temperatureSensorGrainState.State.Readings.Any()
                ? Task.FromResult(_temperatureSensorGrainState.State.Readings.Last())
                : null;

        public async Task RequestReadingFromClient()
        {
            if (_temperatureSensorReceiveRequestObserver != null)
            {
                _logger.LogInformation($"Requesting temperature from sensor {this.GetGrainId().Key} at {DateTime.UtcNow}.");
                var reading = await _temperatureSensorReceiveRequestObserver.GetTemperatureReading();
                await ReceiveTemperatureReading(reading);
            }
        }

        public async Task ReceiveTemperatureReading(TemperatureSensor temperatureReading)
        {
            _logger.LogInformation($"Received {temperatureReading.Fahrenheit} from client {temperatureReading.SensorName} at {temperatureReading.Timestamp}.");
            _temperatureSensorGrainState.State.Readings.Add(temperatureReading);
            _logger.LogInformation($"Temperature sensor {temperatureReading.SensorName} currently has {_temperatureSensorGrainState.State.Readings.Count} records.");

            foreach (var observer in _observers)
            {
                try
                { 
                    await observer.OnTemperatureReadingReceived(temperatureReading);
                }
                catch (Exception ex)
                {
                    _observers.Remove(observer);
                }
            }
        }

        public Task Subscribe(ITemperatureSensorReceivedReadingObserver observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
            return Task.CompletedTask;
        }

        public Task Unsubscribe(ITemperatureSensorReceivedReadingObserver observer)
        {
            if (_observers.Contains(observer))
                _observers.Remove(observer);
            return Task.CompletedTask;
        }

        public Task ListenForRequests(ITemperatureSensorReceiveRequestObserver observer)
        {
            _temperatureSensorReceiveRequestObserver = observer;
            return Task.CompletedTask;
        }

        public Task StopListeningForRequests()
        {
            _temperatureSensorReceiveRequestObserver = null;
            return Task.CompletedTask;
        }
    }

    [GenerateSerializer]
    public class TemperatureSensorGrainState
    {
        [Id(0)]
        public List<TemperatureSensor> Readings { get; set; } = new List<TemperatureSensor>();
    }
}