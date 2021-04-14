# Contoso Monitoring

Welcome to Contoso Monitoring. Our software is used by hotels, apartments, and shopping malls so their owners can monitor their property from a secure web app. 

Our system consists of an ASP.NET Core web app, built using Blazor WebAssembly, and a variety of sensors that run on IoT devices. The system is built using Microsoft Orleans, which provides us the ability to write .NET Core code that runs on small IoT clients that send sensor data, as well as use ASP.NET Core to provide a dashboard snapshot of the sensor activity.

## Goals

You will complete the following tasks in this tutorial:

1. Get the Contoso Monitoring application server cluster running locally.
1. Get the Contoso Monitoring temperature sensor worker service running locally.
1. Use the Orleans Dashboard to see how your temperature sensor worker instance(s) are creating and using Grains.
1. Add code to the ASP.NET Core Blazor dashboard to show the status of the temperature sensors in a web-based dashboard.

## Questions and Discussion

In the first phase of the tutorial you'll validate that you have the required prerequisites so you're ready to start the tutorial. Before you start the tutorial, we have a few questions and tasks to complete. 

---

## Next Steps

Now that the initial discussion and exploration is complete, you can continue on the start the tutorial. 

[Go to Phase 2](docs/01-setup.md)
