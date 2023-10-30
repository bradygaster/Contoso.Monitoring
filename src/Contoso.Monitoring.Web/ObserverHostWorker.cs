namespace Contoso.Monitoring.Web;

public class ObserverHostWorker : BackgroundService
{
    private IGrainFactory _grainFactory;
    private ITemperatureSensorGrainObserver _temperatureSensorObserver;
    private ITemperatureSensorGrainObserver _temperatureSensorObserverRef;

    public ObserverHostWorker(IGrainFactory grainFactory, ITemperatureSensorGrainObserver temperatureSensorObserver)
    {
        _grainFactory = grainFactory;
        _temperatureSensorObserver = temperatureSensorObserver;
    }

    public async Task<List<TemperatureSensor>> GetSensors()
        => await _grainFactory.GetGrain<ISensorRegistryGrain>(Guid.Empty).GetSensors();

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _temperatureSensorObserverRef = _grainFactory.CreateObjectReference<ITemperatureSensorGrainObserver>(_temperatureSensorObserver);
        await _grainFactory.GetGrain<ISensorRegistryGrain>(Guid.Empty).Subscribe(_temperatureSensorObserverRef);
        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _grainFactory.GetGrain<ISensorRegistryGrain>(Guid.Empty).Unsubscribe(_temperatureSensorObserverRef);
        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _grainFactory.GetGrain<ISensorRegistryGrain>(Guid.Empty).Subscribe(_temperatureSensorObserverRef);
            await Task.Delay(1000);
        }
    }
}