namespace Contoso.Monitoring.Grains;

public class TemperatureSensorGrain : Grain, ITemperatureSensorGrain
{
    private ILogger<TemperatureSensorGrain> _logger;
    private IPersistentState<TemperatureSensorGrainState> _temperatureSensorGrainState;
    private ObserverManager<ITemperatureSensorGrainObserver> _observerManager;

    public TemperatureSensorGrain(ILogger<TemperatureSensorGrain> logger,
        [PersistentState(nameof(TemperatureSensorGrain))] IPersistentState<TemperatureSensorGrainState> temperatureSensorGrainState)
    {
        _logger = logger;
        _temperatureSensorGrainState = temperatureSensorGrainState;
        _observerManager = new ObserverManager<ITemperatureSensorGrainObserver>(TimeSpan.FromMinutes(5), _logger);
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var grainId = this.GetGrainId();
        var sensorName = grainId.Key.ToString();
        var lastKnown = await GrainFactory.GetGrain<ISensorRegistryGrain>(Guid.Empty).GetSensorReading(sensorName);

        await base.OnActivateAsync(cancellationToken);
    }

    public Task<TemperatureSensor> GetTemperature() =>
        _temperatureSensorGrainState.State.Readings.Any()
            ? Task.FromResult(_temperatureSensorGrainState.State.Readings.Last())
            : null;

    public async Task ReceiveTemperatureReading(TemperatureSensor temperatureReading)
    {
        _logger.LogInformation($"Received {temperatureReading.Fahrenheit} from client {temperatureReading.SensorName} at {temperatureReading.Timestamp}.");
        _temperatureSensorGrainState.State.Readings.Add(temperatureReading);
        _logger.LogInformation($"Temperature sensor {temperatureReading.SensorName} currently has {_temperatureSensorGrainState.State.Readings.Count} records.");

        await _observerManager.Notify(o => o.OnTemperatureReadingReceived(temperatureReading));
    }

    public Task Subscribe(ITemperatureSensorGrainObserver observer)
    {
        if (!_observerManager.Contains(observer))
            _observerManager.Subscribe(observer, observer);
        return Task.CompletedTask;
    }

    public Task Unsubscribe(ITemperatureSensorGrainObserver observer)
    {
        _observerManager.Unsubscribe(observer);
        return Task.CompletedTask;
    }
}

[GenerateSerializer]
public class TemperatureSensorGrainState
{
    [Id(0)]
    public List<TemperatureSensor> Readings { get; set; } = new List<TemperatureSensor>();
}