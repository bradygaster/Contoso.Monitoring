# Microsoft Orleans 

Orleans builds on the developer productivity of .NET and brings it to the world of distributed applications, such as cloud services. Orleans scales from a single on-premises server to globally distributed, highly-available applications in the cloud.

Orleans takes familiar concepts like objects, interfaces, async/await, and try/catch and extends them to multi-server environments. As such, it helps developers experienced with single-server applications transition to building resilient, scalable cloud services and other distributed applications. For this reason, Orleans has often been referred to as "Distributed .NET".

## Orleans Grains

The fundamental building block in any Orleans application is a grain. Grains are entities comprising user-defined identity, behavior, and state. Grain identities are user-defined keys which make Grains always available for invocation. Grains can be invoked by other grains or by external clients such as Web frontends, via strongly-typed communication interfaces (contracts). Each grain is an instance of a class which implements one or more of these interfaces.

One example is the `ITemperatureSensorGrain` interface, which is in the `Contoso.Monitoring.Grains.Interfaces` project.

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

---

## Next Steps

Ready to get the app up and running? In the next step you'll run the Orleans Silo, host the temperature sensor and monitoring grains in it, and host the Orleans Dashboard.

[Go to Phase 3](03-clone-repo-and-run-silo.md)
