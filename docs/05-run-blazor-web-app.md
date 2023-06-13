# Phase 4 - Connect an ASP.NET Core Web project to Orleans

One of the projects is a web-based table showing the various temperature sensors and their latest values. This ASP.NET Core Blazor project is named `Contoso.Monitoring.Web`. There are a few code changes you'll need to make that will call the Grains hosted in the Silo to get data to be displayed in the browser. 

## Adding code to the web app

The front end Blazor web app hosts an instance of a `ITemperatureSensorGrainObserver` in a `BackgroundService` class. This means that each web server hosting the front end can behave as an independent client to the back-end system. This is highly useful when hosting a multi-node front end using SignalR, as each SignalR Hub is isolated to an individual server. This way, you can see how one front-end server can host an individual Grain Observer for use to receive changes in Grain state to each front-end Web server in your cluster. 

1. Open the `ObserverHostWorker.cs` file from the `Contoso.Monitoring.Web` project. Note how the `StartAsync` method creates an instance of a `ITemperatureSensorGrainObserver` implementor, and then uses that instance to subscribe to the sensor registry. 

    ```csharp
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _temperatureSensorObserverRef = _grainFactory.CreateObjectReference<ITemperatureSensorGrainObserver>(_temperatureSensorObserver);
        await _grainFactory.GetGrain<ISensorRegistryGrain>(Guid.Empty).Subscribe(_temperatureSensorObserverRef);
        await base.StartAsync(cancellationToken);
    }
    ```

1. The `TemperatureSensorGrainObserver` class implements `ITemperatureSensorGrainObserver`, and simply takes whatever messages it receives from individual Grain classes and hands those off to a SignalR Hub class running on the server:

```csharp
public class TemperatureSensorGrainObserver : ITemperatureSensorGrainObserver
{
    protected string MachineName = Environment.MachineName;
    private IHubContext<BuildingMonitorHub, ITemperatureSensorGrainObserver> _hub;
    private ILogger<TemperatureSensorGrainObserver> _logger;

    public TemperatureSensorGrainObserver(IHubContext<BuildingMonitorHub, ITemperatureSensorGrainObserver> hub, 
        ILogger<TemperatureSensorGrainObserver> logger)
    {
        _hub = hub;
        _logger = logger;
    }

    public bool Equals(ITemperatureSensorGrainObserver x, ITemperatureSensorGrainObserver y)
        => (x as TemperatureSensorGrainObserver).MachineName.ToLower()
            .Equals((y as TemperatureSensorGrainObserver).MachineName.ToLower());

    public int GetHashCode([DisallowNull] ITemperatureSensorGrainObserver obj)
        => (obj as TemperatureSensorGrainObserver).MachineName.ToLower().GetHashCode();

    public Task OnTemperatureReadingReceived(TemperatureSensor reading)
    {
        _hub.Clients.All.OnTemperatureReadingReceived(reading);
        return Task.CompletedTask;
    }
}
```

In `Startup.cs`, you'll see all the services and endpoints for the `BuildingMonitorHub` configured so the app will host the Hub when it starts up: 

```csharp
public IConfiguration Configuration { get; }

public void ConfigureServices(IServiceCollection services)
{
    services.AddSignalR();
    services.AddRazorPages();
    services.AddServerSideBlazor();
    services.AddSingleton<ITemperatureSensorGrainObserver, TemperatureSensorGrainObserver>();
    services.AddHostedService<ObserverHostWorker>();
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapHub<BuildingMonitorHub>("/hubs/monitor");
        endpoints.MapBlazorHub();
        endpoints.MapFallbackToPage("/_Host");
    });
}
```

## Running the web app 

Using Visual Studio or Visual Studio Code, run the `Contoso.Monitoring.Web` project.

> Note: You don't need to close the already-running Silo project's active terminal window. Visual Studio Code supports multiple simultaneous terminal windows. 

When the browser opens you'll see the table that will include the list of temperature sensors. If the web browser doesn't automatically open, browse to http://localhost:5000 to see it.

![The temperature page open in a browser.](media/06-browser-empty.png)

The web app is somewhat boring at the moment, but once the temperature sensors start feeding data into the silo the web app will start showing the incoming sensor data.

---

## Next Steps

With the Silo running and the Blazor web app running and ready to receive data, you'll run a few instances of the Temperature sensor worker to emulate physical sensors sending live data.

[Go to Phase 6](06-temperature-worker-service.md)