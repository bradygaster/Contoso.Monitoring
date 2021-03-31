# The Microsft Orleans Dashboard

The `UseDashboard` extension method results in the dynamic generation and hosting of a web site that provides you a with a snapshot of the entire Orleans cluster. It allows you to see each instance of any type of Grain, and how all the objects in the system are distributed across the cluster. 

```csharp
.UseOrleans(siloBuilder =>
{
    siloBuilder.UseDashboard();
}
```

The Orleans Dashboard is a part of the larger Orleans Contrib project, which contains many smaller contributions from the open-source community who extend Orleans in new and exciting ways. 

> Note: This extension method requires the `AddApplicationPart` method be called to inform the dashbord of all the grains in the cluster.

The entire `Program.cs`, including the dashboard setup, is under 30 lines of code.

```csharp
await Host
        .CreateDefaultBuilder(args)
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

            siloBuilder.ConfigureApplicationParts(appParts => 
            {
                appParts.AddApplicationPart(typeof(TemperatureSensorGrain).Assembly).WithReferences();
            });

            siloBuilder.UseDashboard();
        }).RunConsoleAsync();
```

---

## Next Steps

Now that you have the Orleans Silo hosted in a .NET Core Generic Host, you'll use an Orleans client to connect to the Silo from another Generic Host.

[Go to Phase 4](04-temperature-worker-service.md)