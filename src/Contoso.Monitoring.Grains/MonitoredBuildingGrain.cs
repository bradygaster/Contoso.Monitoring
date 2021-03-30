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
    public class MonitoredBuildingGrain : Grain, IMonitoredBuildingGrain
    {
        private readonly ILogger<MonitoredBuildingGrain> _logger;
        private readonly IPersistentState<MonitoredBuildingGrainState> _monitoredBuildingGrainState;
        private readonly IGrainFactory _grainFactory;
        private ITemperatureSensorGrain _temperatureSensorGrain;

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
            _temperatureSensorGrain ??= _grainFactory.GetGrain<ITemperatureSensorGrain>(areaName);

            return new MonitoredArea
            {
                Temperature = await _temperatureSensorGrain.GetTemperature()
            };
        }

        public Task<List<string>> GetMonitoredAreaNames() => 
            Task.FromResult(_monitoredBuildingGrainState.State.MonitoredAreaNames);
    }

    [Serializable]
    public class MonitoredBuildingGrainState
    {
        public List<string> MonitoredAreaNames { get; set; } = new List<string>();
    }
}