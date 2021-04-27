using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Contoso.Monitoring.Grains;

await Host.CreateDefaultBuilder(args)
    .UseOrleans(siloBuilder =>
    {
        siloBuilder
            .AddMemoryGrainStorage(name: "contosoMonitoringStore")
            .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "ContosoMonitoring";
                })
            .UseLocalhostClustering();

        siloBuilder
            .ConfigureApplicationParts(applicationParts => 
                applicationParts.AddApplicationPart(typeof(TemperatureSensorGrain).Assembly).WithReferences())
            .UseDashboard();
            
    }).RunConsoleAsync();
