## Phase 3 - Hosting Orleans Grains in a Silo

The Contoso Monitoring solution has one silo, that is hosted using a .NET Core Generic Host. The `Program.cs`, including the dashboard setup is the only file C# in the `Contoso.Monitoring.Sensors.Silo` project. This is all the code required to host an Orleans silo.

Open the `Program.cs` file from the `Contoso.Monitoring.Sensors.Silo` project. 

## Start the Silo project

In Visual Studio or Visual Studio Code, start the `Contoso.Monitoring.Sensors.Silo` project. 

Once the project is running, open a web browser to http://localhost:8080. You should see the Orleans Dashboard, similar to this screen shot.

![Empty Orleans dashboard](media/03-dashboard.png)

In the next few steps you'll add a web app that serves as a client calling Grains hosted in the Orleans Silo, and you'll run multiple instances of a worker service that will serve as digital twins for temperature sensors in those monitored areas.

---

## Next Steps

Now that you have the Orleans Silo hosted in a .NET Core Generic Host you'll see how to react to changes in Grain state using Grain Observer classes. 

[Go to Phase 4](04-grain-observers.md)