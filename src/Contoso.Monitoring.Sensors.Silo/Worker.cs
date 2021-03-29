using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contoso.Monitoring.Grains.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;

namespace Contoso.Monitoring.Sensors.Silo
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IGrainFactory _grainFactory;
        private ITemperatureSensorGrain _temperatureSensorGrain;

        public Worker(ILogger<Worker> logger,
            IGrainFactory grainFactory)
        {
            _logger = logger;
            _grainFactory = grainFactory;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Creating grain of type {typeof(ITemperatureSensorGrain)}.");
            _temperatureSensorGrain = _grainFactory.GetGrain<ITemperatureSensorGrain>("Sensors Silo");
            _logger.LogInformation($"Created grain of type {_temperatureSensorGrain.GetType().FullName}.");

            await base.StartAsync(cancellationToken);
        }
        
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
