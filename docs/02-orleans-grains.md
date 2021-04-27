# Phase 2 - Contoso Monitoring Review

In this phase you'll review the Contoso Monitoring source code to get familiar with how Orleans is being used in it. If you are not yet familiar with Microsoft Orleans Grains, please [review the Grains introduction documentation](https://dotnet.github.io/orleans/docs/grains/index.html). 

## Temperature Sensors

1. Review the `ITemperatureSensorGrain` interface from the `Contoso.Monitoring.Grains.Interfaces` project. It represents a "digital twin" for any temperature sensor in a monitored building. 

1. Review the `TemperatureSensorGrain` class in from the `Contoso.Monitoring.Grains` project. For each temperature sensor in a monitored building, there would be one `TemperatureSensorGrain` sensor class instance collecting data.

## Monitored Buildings or Spaces

1. Review the `IMonitoredBuildingGrain` interface from the `Contoso.Monitoring.Grains.Interfaces` project. It represents a collection of sensors in a physical building. 

1. Review the `MonitoredBuildingGrain` class in from the `Contoso.Monitoring.Grains` project. 

---

## Next Steps

Ready to get the app up and running? In the next step you'll run the Orleans Silo, host the temperature sensor and monitoring grains in it, and host the Orleans Dashboard.

[Go to Phase 3](03-hosting-grains.md)
