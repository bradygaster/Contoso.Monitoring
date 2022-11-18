using Contoso.Monitoring.Grains;

namespace Contoso.Monitoring.Web
{
    public class ObserverHostWorker : BackgroundService
    {
        private IGrainFactory _grainFactory;
        private ITemperatureSensorGrainObserver _temperatureSensorObserver;

        public ObserverHostWorker(IGrainFactory grainFactory, ITemperatureSensorGrainObserver temperatureSensorObserver)
        {
            _grainFactory = grainFactory;
            _temperatureSensorObserver = temperatureSensorObserver;
        }

        public async Task<List<TemperatureSensor>> GetSensors()
            => await _grainFactory.GetGrain<ISensorRegistryGrain>(Guid.Empty).GetSensors();

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var reference = _grainFactory.CreateObjectReference<ITemperatureSensorGrainObserver>(_temperatureSensorObserver);
            await _grainFactory.GetGrain<ISensorRegistryGrain>(Guid.Empty).Subscribe(reference);
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            var reference = _grainFactory.CreateObjectReference<ITemperatureSensorGrainObserver>(_temperatureSensorObserver);
            await _grainFactory.GetGrain<ISensorRegistryGrain>(Guid.Empty).Unsubscribe(reference);
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var reference = _grainFactory.CreateObjectReference<ITemperatureSensorGrainObserver>(_temperatureSensorObserver);
                await _grainFactory.GetGrain<ISensorRegistryGrain>(Guid.Empty).Subscribe(reference);

                await Task.Delay(1000);
            }
        }
    }
}