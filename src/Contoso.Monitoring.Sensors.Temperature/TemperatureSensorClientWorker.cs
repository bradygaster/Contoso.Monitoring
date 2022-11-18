using Contoso.Monitoring.Grains;

namespace Contoso.Monitoring.Sensors.Temperature
{
    public class TemperatureSensorClientWorker : IHostedService
    {
        private readonly ILogger<TemperatureSensorClientWorker> _logger;
        private readonly ITemperatureSensorReceiveRequestObserver _temperatureSensorClient;
        private readonly IGrainFactory _grainFactory;
        private ITemperatureSensorReceiveRequestObserver _temperatureSensorClientRef;
        private ITemperatureSensorGrain _temperatureSensorGrain;

        public TemperatureSensorClientWorker(ILogger<TemperatureSensorClientWorker> logger,
            ITemperatureSensorReceiveRequestObserver temperatureSensorClient,
            IGrainFactory grainFactory)
        {
            _logger = logger;
            _temperatureSensorClient = temperatureSensorClient;
            _grainFactory = grainFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker starting at: {time}", DateTimeOffset.Now);
            var reading = await _temperatureSensorClient.GetTemperatureReading();

            _temperatureSensorClientRef = _grainFactory.CreateObjectReference<ITemperatureSensorReceiveRequestObserver>(_temperatureSensorClient);
            _temperatureSensorGrain ??= _grainFactory.GetGrain<ITemperatureSensorGrain>(reading.SensorName);
            await _temperatureSensorGrain.ListenForRequests(_temperatureSensorClientRef);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker stopping at: {time}", DateTimeOffset.Now);
            await _temperatureSensorGrain.StopListeningForRequests();
        }
    }
}
