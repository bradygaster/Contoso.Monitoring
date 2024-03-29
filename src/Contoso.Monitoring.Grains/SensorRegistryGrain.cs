namespace Contoso.Monitoring.Grains;

public class SensorRegistryGrain : Grain, ISensorRegistryGrain
{
    private readonly ILogger<SensorRegistryGrain> _logger;
    private readonly IPersistentState<MonitoredBuildingGrainState> _monitoredBuildingGrainState;

    public SensorRegistryGrain(ILogger<SensorRegistryGrain> logger,
        [PersistentState(nameof(SensorRegistryGrain))] IPersistentState<MonitoredBuildingGrainState> monitoredBuildingGrainState)
    {
        _logger = logger;
        _monitoredBuildingGrainState = monitoredBuildingGrainState;
    }

    public async Task<TemperatureSensor> GetSensorReading(string areaName)
    {
        if (!_monitoredBuildingGrainState.State.MonitoredAreaNames.Contains(areaName))
        {
            _logger.LogInformation($"Adding '{areaName}' to the list of monitored areas.");
            _monitoredBuildingGrainState.State.MonitoredAreaNames.Add(areaName);
            _logger.LogInformation($"Added '{areaName}' to the list of monitored areas.");
            return new TemperatureSensor { SensorName = areaName, Timestamp = DateTime.UtcNow };
        }
        else
        {
            return await GrainFactory.GetGrain<ITemperatureSensorGrain>(areaName).GetTemperature();
        }
    }

    public async Task<List<TemperatureSensor>> GetSensors()
    {
        var tasks = _monitoredBuildingGrainState.State.MonitoredAreaNames.Select(async _ => await GetSensorReading(_));
        var sensors = await Task.WhenAll(tasks);
        return sensors.ToList();
    }

    public async Task Subscribe(ITemperatureSensorGrainObserver observer)
    {
        var sensors = await GetSensors();
        var tasks = sensors.Select(async x => await GrainFactory.GetGrain<ITemperatureSensorGrain>(x.SensorName).Subscribe(observer));
        await Task.WhenAll(tasks);
    }

    public async Task Unsubscribe(ITemperatureSensorGrainObserver observer)
    {
        var sensors = await GetSensors();
        var tasks = sensors.Select(async x => await GrainFactory.GetGrain<ITemperatureSensorGrain>(x.SensorName).Unsubscribe(observer));
        await Task.WhenAll(tasks);
    }

    [GenerateSerializer]
    public class MonitoredBuildingGrainState
    {
        [Id(0)]
        public List<string> MonitoredAreaNames { get; set; } = new List<string>();
    }
}