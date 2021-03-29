using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Contoso.Monitoring.Grains;

namespace Contoso.Monitoring.Sensors.Silo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseOrleans(siloBuilder =>
                {
                    siloBuilder
                        .Configure<ClusterOptions>(options =>
                            {
                                options.ClusterId = "dev";
                                options.ServiceId = "ContosoMonitoring";
                            })
                        .UseLocalhostClustering();

                    siloBuilder.ConfigureApplicationParts(appParts => 
                    {
                        appParts.AddApplicationPart(typeof(TemperatureSensorGrain).Assembly).WithReferences();
                    });

                    siloBuilder.UseDashboard();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });
    }
}
