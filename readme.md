# Contoso Monitoring

Welcome to Contoso Monitoring. Our software is used by hotels, apartments, and shopping malls so their owners can monitor their property from a secure web app. 

Our system consists of an ASP.NET Core web app, built using Blazor WebAssembly, and a variety of sensors that run on IoT devices. The system is built using Microsoft Orleans, which provides us the ability to write .NET Core code that runs on small IoT clients that send sensor data, as well as use ASP.NET Core to provide a dashboard snapshot of the sensor activity.

## Prerequisites

This tutorial assumes that you have built a few ASP.NET Core applications, and that you are either actively building larger-scale ASP.NET Core apps or that you are interested in learning how to build larger-scale apps. You should also have the following installed.

1. [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0) runtime & SDK
2. [Visual Studio Code](https://code.visualstudio.com/)
3. OR you could install [Visual Studio 2019 Preview](https://visualstudio.microsoft.com/vs/preview/) to get all the components.

## Setup

Clone (or download) the contents of this repository. Please clone the `main` branch to make sure you get the tutorial code and not the finished code. If you'd like to clone the finished code into another local repository to reference whilst you work, clone the `finished` branch locally.

## Goals

You will complete the following goals in this tutorial:

1. Get the Contoso Monitoring application server cluster running locally.
2. Use the Orleans Dashboard to see how your Temperature worker instance(s) are creating and using Grains.
3. Add code to the ASP.NET Core Blazor dashboard to show the status of the temperature sensors in a web-based dashboard.

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