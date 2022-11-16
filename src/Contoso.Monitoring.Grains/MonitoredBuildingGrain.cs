using Contoso.Monitoring.Grains.Interfaces;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;

namespace Contoso.Monitoring.Grains
{
    public class MonitoredBuildingGrain : Grain, IMonitoredBuildingGrain
    {
        private readonly ILogger<MonitoredBuildingGrain> _logger;
        private readonly IPersistentState<MonitoredBuildingGrainState> _monitoredBuildingGrainState;

        public MonitoredBuildingGrain(ILogger<MonitoredBuildingGrain> logger,
            [PersistentState(nameof(MonitoredBuildingGrain))] IPersistentState<MonitoredBuildingGrainState> monitoredBuildingGrainState)
        {
            _logger = logger;
            _monitoredBuildingGrainState = monitoredBuildingGrainState;
        }

        public Task MonitorArea(string areaName)
        {
            if (!_monitoredBuildingGrainState.State.MonitoredAreaNames.Contains(areaName))
            {
                _logger.LogInformation($"Adding '{areaName}' to the list of monitored areas.");
                _monitoredBuildingGrainState.State.MonitoredAreaNames.Add(areaName);
                _logger.LogInformation("The list of area names now includes:");
                _monitoredBuildingGrainState.State.MonitoredAreaNames.ForEach(_ => _logger.LogInformation(_));
                _logger.LogInformation($"Added '{areaName}' to the list of monitored areas.");
            }

            return Task.CompletedTask;
        }
        public async Task<MonitoredArea> GetMonitoredArea(string areaName) => new MonitoredArea
        {
            Name = areaName,
            Temperature = await GrainFactory.GetGrain<ITemperatureSensorGrain>(areaName).GetTemperature()
        };

        public async Task<List<MonitoredArea>> GetMonitoredAreas()
        {
            var result = new List<MonitoredArea>();

            foreach (var area in _monitoredBuildingGrainState.State.MonitoredAreaNames)
            {
                result.Add(await GetMonitoredArea(area));
            }

            return result;
        }

        public async Task Subscribe(ITemperatureSensorGrainObserver observer)
        {
            foreach (var area in _monitoredBuildingGrainState.State.MonitoredAreaNames)
            {
                await GrainFactory.GetGrain<ITemperatureSensorGrain>(area).Subscribe(observer);
            }
        }

        public async Task Unsubscribe(ITemperatureSensorGrainObserver observer)
        {
            foreach (var area in _monitoredBuildingGrainState.State.MonitoredAreaNames)
            {
                await GrainFactory.GetGrain<ITemperatureSensorGrain>(area).Unsubscribe(observer);
            }
        }
    }

    [GenerateSerializer]
    public class MonitoredBuildingGrainState
    {
        [Id(0)]
        public List<string> MonitoredAreaNames { get; set; } = new List<string>();
    }
}