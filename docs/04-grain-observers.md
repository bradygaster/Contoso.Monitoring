## Phase 4 - Grain Observers

Like Grains, Grain Observers can live on one or a variety of Silos in an Orleans Cluster. Grain Observers are useful in scenarios where you want to react to a change in a Grain's state, or react to behaviors as the Grains execute. 

In the `Contoso.Monitoring.Grains.Interfaces` project you'll find the `ITemperatureSensorGrainObserver` interface. This interface provides an abstraction for use when reacting to a `TemperatureSensorGrain` receiving a temperature reading. 

```csharp
public interface ITemperatureSensorGrainObserver : IGrainObserver
{
    Task OnTemperatureReadingReceived(TemperatureSensor reading);
}
```

The `ITemperatureSensorGrain` has two methods - `Subscribe` and `Unsubcribe`, that take an instance of an `ITemperatureSensorGrainObserver` interface. The implementation of these methods, in `TemperatureSensorGrain`, is used to Subscribe and Unsubscribe each Observer. 

```csharp
private ObserverManager<ITemperatureSensorGrainObserver> _observerManager;

public TemperatureSensorGrain(ILogger<TemperatureSensorGrain> logger,
    [PersistentState(nameof(TemperatureSensorGrain))] IPersistentState<TemperatureSensorGrainState> temperatureSensorGrainState)
{
    _logger = logger;
    _temperatureSensorGrainState = temperatureSensorGrainState;
    _observerManager = new ObserverManager<ITemperatureSensorGrainObserver>(TimeSpan.FromMinutes(5), _logger);
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
```

As any `TemperatureSensorGrain` in the cluster receives readings, it can turn around and pump those into all of the `ITemperatureSensorGrainObserver` instances that have been subscribed thus far. 

```csharp
public async Task ReceiveTemperatureReading(TemperatureSensor temperatureReading)
{
    /// other code omitted for now

    await _observerManager.Notify(o => o.OnTemperatureReadingReceived(temperatureReading));
}
```

So, when any temperature sensor grain receives a reading, it can then turn around and pump all of those readings into the observers.

---

## Next Steps

Now that you understand how Grain Observers work to provide reactionary logic for Grain behaviors or changes in state, let's see how Grain Observers can be used together with SignalR Hubs, so you can echo changes in the back end all the way to your HTML client.

[Go to Phase 5](05-run-blazor-web-app.md)