using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contoso.Monitoring;
using Contoso.Monitoring.Grains.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;

namespace Contoso.Monitoring.Sensors.Temperature
{
    public class TemperatureSensorClientWorker : BackgroundService
    {
        private readonly ILogger<TemperatureSensorClientWorker> _logger;
        private readonly ContosoMonitoringClientService _clusterService;
        private readonly ITemperatureSensorClient _temperatureSensorClient;
        private ITemperatureSensorGrain _temperatureSensorGrain;
        private IMonitoredBuildingGrain _monitoredBuildingGrain;

        public TemperatureSensorClientWorker(ILogger<TemperatureSensorClientWorker> logger,
            ContosoMonitoringClientService clusterService,
            ITemperatureSensorClient temperatureSensorClient)
        {
            _logger = logger;
            _clusterService = clusterService;
            _temperatureSensorClient = temperatureSensorClient;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Connecting...");
            await _clusterService.Connect(cancellationToken);
            _logger.LogInformation("Connected.");
            await base.StartAsync(cancellationToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_clusterService.Stop(cancellationToken));
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
                    _temperatureSensorGrain ??= _clusterService.Client.GetGrain<ITemperatureSensorGrain>(reading.SensorName);
                    await _temperatureSensorGrain.ReceiveTemperatureReading(reading);

                    // get the monitored building grain
                    _monitoredBuildingGrain ??= _clusterService.Client.GetGrain<IMonitoredBuildingGrain>(Guid.Empty);
                    await _monitoredBuildingGrain.MonitorArea(reading.SensorName);
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
}
