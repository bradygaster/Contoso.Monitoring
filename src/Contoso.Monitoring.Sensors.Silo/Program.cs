using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

await Host.CreateDefaultBuilder(args)
    .UseOrleans(siloBuilder =>
    {
        siloBuilder
            .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "ContosoMonitoring";
                })
            .AddMemoryGrainStorageAsDefault()
            .UseLocalhostClustering(siloPort: 11111, gatewayPort: 30000);

        siloBuilder
            .UseDashboard();

    }).RunConsoleAsync();
