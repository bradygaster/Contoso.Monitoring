namespace Contoso.Monitoring.Grains;

public class MonitoredBuildingGrain : Grain, IMonitoredBuildingGrain
{
    private readonly ILogger<MonitoredBuildingGrain> _logger;
    private readonly IPersistentState<MonitoredBuildingGrainState> _monitoredBuildingGrainState;
    private readonly IGrainFactory _grainFactory;

    public MonitoredBuildingGrain(ILogger<MonitoredBuildingGrain> logger,
        [PersistentState("monitoredBuildingGrainState", "contosoMonitoringStore")] 
        IPersistentState<MonitoredBuildingGrainState> monitoredBuildingGrainState,
        IGrainFactory grainFactory)
    {
        _logger = logger;
        _monitoredBuildingGrainState = monitoredBuildingGrainState;
        _grainFactory = grainFactory;
    }

    public Task MonitorArea(string areaName)
    {
        _logger.LogInformation($"Adding '{areaName}' to the list of monitored areas.");
        _monitoredBuildingGrainState.State.MonitoredAreaNames.Remove(areaName);
        _monitoredBuildingGrainState.State.MonitoredAreaNames.Add(areaName);
        _logger.LogInformation($"Added '{areaName}' to the list of monitored areas.");
        _logger.LogInformation("The list of area names now includes:");
        _monitoredBuildingGrainState.State.MonitoredAreaNames.ForEach(_ => _logger.LogInformation(_));
        return Task.CompletedTask;
    }
    public async Task<MonitoredArea> GetMonitoredArea(string areaName)
    {
        return new MonitoredArea
        {
            Name = areaName,
            Temperature = await _grainFactory.GetGrain<ITemperatureSensorGrain>(areaName).GetTemperature()
        };
    }

    public async Task<List<MonitoredArea>> GetMonitoredAreas()
    {
        var result = new List<MonitoredArea>();

        foreach (var area in _monitoredBuildingGrainState.State.MonitoredAreaNames)
        {
            result.Add(await GetMonitoredArea(area));
        }

        return result;
    }
}

[Serializable]
public class MonitoredBuildingGrainState
{
    public List<string> MonitoredAreaNames { get; set; } = new List<string>();
}