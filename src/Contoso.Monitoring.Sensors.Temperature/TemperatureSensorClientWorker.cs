using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contoso.Monitoring.Grains.Interfaces;
using Contoso.Monitoring.Sensors.Temperature.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;

namespace Contoso.Monitoring.Sensors.Temperature
{
    public class TemperatureSensorClientWorker : BackgroundService
    {
        private readonly ILogger<TemperatureSensorClientWorker> _logger;
        private readonly ITemperatureSensorClient _temperatureSensorClient;
        private ITemperatureSensorGrain _temperatureSensorGrain;
        private IMonitoredBuildingGrain _monitoredBuildingGrain;

        public IClusterClient Client { get; }

        public TemperatureSensorClientWorker(ILogger<TemperatureSensorClientWorker> logger,
            ITemperatureSensorClient temperatureSensorClient)
        {
            _logger = logger;
            _temperatureSensorClient = temperatureSensorClient;
            
            try
            {
                Client = new ClientBuilder()
                                .UseLocalhostClustering()
                                .Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to cluster");
                throw;
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
             _logger.LogInformation("Connecting...");

            var retries = 100;
            await Client.Connect(async error =>
            {
                if (--retries < 0)
                {
                    _logger.LogError("Could not connect to the cluster: {@Message}", error.Message);
                    return false;
                }
                else
                {
                    _logger.LogWarning(error, "Error Connecting: {@Message}", error.Message);
                }

                try
                {
                    await Task.Delay(1000, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return false;
                }

                return true;
            });

            _logger.LogInformation("Connected.");
            await base.StartAsync(cancellationToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            var cancellation = new TaskCompletionSource<bool>();
            cancellationToken.Register(() => cancellation.TrySetCanceled(cancellationToken));

            return Task.WhenAny(Client.Close(), cancellation.Task);
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
                    _temperatureSensorGrain ??= Client.GetGrain<ITemperatureSensorGrain>(reading.SensorName);
                    await _temperatureSensorGrain.ReceiveTemperatureReading(reading);

                    // get the monitored building grain
                    _monitoredBuildingGrain ??= Client.GetGrain<IMonitoredBuildingGrain>(Guid.Empty);
                    await _monitoredBuildingGrain.MonitorArea(reading.SensorName);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Error getting grain.");
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
