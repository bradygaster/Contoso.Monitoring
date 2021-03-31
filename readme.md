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

You will complete the following tasks in this tutorial:

1. Get the Contoso Monitoring application server cluster running locally.
1. Get the Contoso Monitoring temperature sensor worker service running locally.
1. Use the Orleans Dashboard to see how your temperature sensor worker instance(s) are creating and using Grains.
1. Add code to the ASP.NET Core Blazor dashboard to show the status of the temperature sensors in a web-based dashboard.

---

## Next Steps

Next, you'll learn about the Contoso Monitoring app's components to get some context for when you dive in to start making changes.

[Go to Phase 2](docs/02-orleans-grains.md)
