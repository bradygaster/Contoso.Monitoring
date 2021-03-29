# Contoso Monitoring

Welcome to Contoso Monitoring. Our software is used by hotels, apartments, and shopping malls so their owners can monitor their property from a secure web app. 

Our system consists of an ASP.NET Core web app, built using Blazor WebAssembly, and a variety of sensors that run on IoT devices. The system is built using Microsoft Orleans, which provides us the ability to write .NET Core code that runs on small IoT clients that send sensor data, as well as use ASP.NET Core to provide a dashboard snapshot of the sensor activity.

## Orleans Grains

The main distributed services used to transmit data are Microsoft Orleans Grains. One example is the `ITemperatureSensorGrain` interface, which is in the `Contoso.Monitoring.Grains.Interfaces` project.

```csharp
namespace Contoso.Monitoring.Grains.Interfaces
{
    public interface ITemperatureSensorGrain : Orleans.IGrainWithStringKey
    {
        Task ReceiveTemperatureReading(TemperatureReading temperatureReading);
    }
}
```

The interface is implemented via the `TemperatureSensorGrain` class. There isn't much implementation yet, as we're just the stage of getting the topology constructed so that any variety of sensor clients can be added at any time without impacting any of the other clients or back-end systems that work as sensor data is received.

```csharp
public class TemperatureSensorGrain : Orleans.Grain, ITemperatureSensorGrain
{
    public TemperatureSensorGrain(ILogger<TemperatureSensorGrain> logger)
    {
        this.Logger = logger;
    }

    public ILogger<TemperatureSensorGrain> Logger { get; }

    public Task ReceiveTemperatureReading(TemperatureReading temperatureReading)
    {
        Logger.LogInformation($"Received {temperatureReading.Fahrenheit} from client {temperatureReading.SensorName} at {DateTime.UtcNow}.");
        return Task.FromResult(true);
    }
}
```

## The Temperature Sensor worker service 

Devices hosting the temperature sensor app, a .NET Core worker service project, implement the `ITemperatureSensorClient` interface to add code specific to whatever type of temperature-sensing behavior the host device supports.

```csharp
namespace Contoso.Monitoring.Sensors.Temperature.Services
{
    public interface ITemperatureSensorClient
    {
        Task<TemperatureReading> GetTemperatureReading();
    }
}
```

The job of this interface is to give the `TemperatureSensorClientWorker` class a way to get the temperature from a wide variety of host devices or platforms. Implemented by the `FakeTemperatureSensorClient` class, the current implementation generates fake temperature readings that are sent to the `ITemperatureSensorGrain` running in the Microsoft Orleans silo.

```csharp
public class FakeTemperatureSensorClient : ITemperatureSensorClient
{
    private string _randomSensorName;

    public FakeTemperatureSensorClient()
    {
        _randomSensorName = string.Concat(new Random().Next(1,10), new Random().Next(1,50));
    }

    public Task<TemperatureReading> GetTemperatureReading()
    {
        var fakeTempInF = new Random().Next(60, 80); 

        return Task.FromResult(new TemperatureReading
        {
            SensorName = _randomSensorName,
            Fahrenheit = fakeTempInF,
            Celsius = TemperatureReadingConverter.ToCelsius(fakeTempInF)
        });
    }
}
```