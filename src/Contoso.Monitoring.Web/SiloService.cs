using Contoso.Monitoring.Grains.Interfaces;

namespace Contoso.Monitoring.Web
{
    public class SiloService : BackgroundService
    {
        private IGrainFactory _grainFactory;
        private ITemperatureSensorGrainObserver _temperatureSensorObserver;

        public SiloService(IGrainFactory grainFactory, ITemperatureSensorGrainObserver temperatureSensorObserver)
        {
            _grainFactory = grainFactory;
            _temperatureSensorObserver = temperatureSensorObserver;
        }

        public async Task<List<MonitoredArea>> GetMonitoredAreas()
            => await _grainFactory.GetGrain<IMonitoredBuildingGrain>(Guid.Empty).GetMonitoredAreas();

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var reference = _grainFactory.CreateObjectReference<ITemperatureSensorGrainObserver>(_temperatureSensorObserver);
            await _grainFactory.GetGrain<IMonitoredBuildingGrain>(Guid.Empty).Subscribe(reference);
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            var reference = _grainFactory.CreateObjectReference<ITemperatureSensorGrainObserver>(_temperatureSensorObserver);
            await _grainFactory.GetGrain<IMonitoredBuildingGrain>(Guid.Empty).Unsubscribe(reference);
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var reference = _grainFactory.CreateObjectReference<ITemperatureSensorGrainObserver>(_temperatureSensorObserver);
                await _grainFactory.GetGrain<IMonitoredBuildingGrain>(Guid.Empty).Subscribe(reference);

                await Task.Delay(1000);
            }
        }
    }
}