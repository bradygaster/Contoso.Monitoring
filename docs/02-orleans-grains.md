# Microsoft Orleans Grains

The main distributed services used in Contoso Monitoring to transmit data are Microsoft Orleans Grains. You can think of a Grain as a "cloud native object," that has a variety of capabilities. One example is the `ITemperatureSensorGrain` interface, which is in the `Contoso.Monitoring.Grains.Interfaces` project.

```csharp
namespace Contoso.Monitoring.Grains.Interfaces
{
    public interface ITemperatureSensorGrain : Orleans.IGrainWithStringKey
    {
        Task ReceiveTemperatureReading(TemperatureReading temperatureReading);
        Task<TemperatureReading> GetTemperature();
    }
}
```

The interface is implemented via the `TemperatureSensorGrain` class, from the `Contoso.Monitoring.Grains` project. Each `TemperatureSensorGrain` instance can be thought of as a digital twin of a physical temperature sensor. Each instance can save its own state. 

```csharp
namespace Contoso.Monitoring.Grains
{
    public class TemperatureSensorGrain : Orleans.Grain, ITemperatureSensorGrain
    {
        private IPersistentState<TemperatureSensorGrainState> _temperatureSensorGrainState;

        public Task<TemperatureReading> GetTemperature()
        {
            if(_temperatureSensorGrainState.State.Readings.Any())
            {
                return Task.FromResult(_temperatureSensorGrainState.State.Readings.Last());
            }

            return null;
        }

        public Task ReceiveTemperatureReading(TemperatureReading temperatureReading)
        {
            _temperatureSensorGrainState.State.Readings.Add(temperatureReading);
            
            return Task.FromResult(true);
        }
    }
}
```

## Persisting state and data

To store data and state specific to the temperature sensor, each `TemperatureSensorGrain` uses an `TemperatureSensorGrainState` instance stored in Orleans persistent state store.

```csharp
[Serializable]
public class TemperatureSensorGrainState
{
    public List<TemperatureReading> Readings { get; set; } = new List<TemperatureReading>();
}
```

The `TemperatureSensorGrainState` model is created using the Orleans `IPersistentState<T>` object; in our case, an instance of `IPersistentState<TemperatureSensorGrainState>` is passed to the `TemperatureSensorGrain` constructor. 

```csharp
public TemperatureSensorGrain(ILogger<TemperatureSensorGrain> logger,
    [PersistentState("temperatureSensorGrainState", "contosoMonitoringStore")] 
    IPersistentState<TemperatureSensorGrainState> temperatureSensorGrainState)
{
    _logger = logger;
    _temperatureSensorGrainState = temperatureSensorGrainState;
}
```

## Grains calling other Grains

Grain classes can make use of one another. The `MonitoredBuildingGrain` class, which implements the `IMonitoredBuildingGrain` grain interface, represent a digital twin of an area in a building that's being monitored. Like the `TemperatureSensorGrain` class, `MonitoredBuildingGrain` persists its own state. But it also uses the `TemperatureSensorGrain` object to get a monitored area's last-recorded temperature.

```csharp
public MonitoredBuildingGrain(ILogger<MonitoredBuildingGrain> logger,
    [PersistentState("monitoredBuildingGrainState", "contosoMonitoringStore")] 
    IPersistentState<MonitoredBuildingGrainState> monitoredBuildingGrainState,
    IGrainFactory grainFactory)
{
    _logger = logger;
    _monitoredBuildingGrainState = monitoredBuildingGrainState;
    _grainFactory = grainFactory;
}

// other code

public async Task<MonitoredArea> GetMonitoredArea(string areaName)
{
    return new MonitoredArea
    {
        Name = areaName,
        Temperature = await _grainFactory.GetGrain<ITemperatureSensorGrain>(areaName).GetTemperature()
    };
}
``` 

## Hosting Grains

In the `Contoso.Monitoring.Sensors.Silo` project's `Program.cs`, you'll see how a Microsoft Orleans cluster silo - essentially a multi-server application server that hosts you'r grain class instances - is hosted using the .NET Core Generic Host.

```csharp
await Host
        .CreateDefaultBuilder(args)
        .UseOrleans(siloBuilder =>
        {
            siloBuilder
                .AddMemoryGrainStorage(name: "contosoMonitoringStore")
                .Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "dev";
                        options.ServiceId = "ContosoMonitoring";
                    })
                .UseLocalhostClustering();
        })
        .RunConsoleAsync();
```

The `UseOrleans` extension method is essentially all that a .NET Core Generic Host needs to host all the grains in a distributed system. 

---

## Next Steps

Ready to get the app up and running? In the next step you'll clone the project and get it running locally.

[Go to Phase 3](03-clone-repo-and-run-silo.md)
