using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Hosting;

namespace Contoso.Monitoring.Web
{
    public class Program
    {
        static int _delta = 11;

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseOrleans((ctx, siloBuilder) =>
                {
                    siloBuilder.UseLocalhostClustering(
                        siloPort: 11111 + _delta,
                        gatewayPort: 30000 + _delta,
                        primarySiloEndpoint: new IPEndPoint(IPAddress.Loopback, 11111)
                    );
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
