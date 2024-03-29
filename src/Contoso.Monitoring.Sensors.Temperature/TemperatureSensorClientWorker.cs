namespace Contoso.Monitoring.Sensors.Temperature;

public class TemperatureSensorClientWorker : BackgroundService
{
    private readonly ILogger<TemperatureSensorClientWorker> _logger;
    private readonly ITemperatureSensorClient _temperatureSensorClient;
    private readonly IGrainFactory _grainFactory;
    private ITemperatureSensorGrain _temperatureSensorGrain;
    private ISensorRegistryGrain _monitoredBuildingGrain;

    public TemperatureSensorClientWorker(ILogger<TemperatureSensorClientWorker> logger,
        ITemperatureSensorClient temperatureSensorClient,
        IGrainFactory grainFactory)
    {
        _logger = logger;
        _temperatureSensorClient = temperatureSensorClient;
        _grainFactory = grainFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // get the temperature
                var reading = await _temperatureSensorClient.GetTemperatureReading();

                // get the temp sensor grain
                _temperatureSensorGrain ??= _grainFactory.GetGrain<ITemperatureSensorGrain>(reading.SensorName);
                await _temperatureSensorGrain.ReceiveTemperatureReading(reading);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting grain.");
            }

            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(100, stoppingToken);
        }
    }
}
