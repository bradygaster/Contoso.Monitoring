using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Hosting;
using System.Threading.Tasks;

namespace Contoso.Monitoring.Sensors.Temperature
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseOrleansClient(client =>
                {
                    Task.Delay(5000).Wait();
                    client.UseLocalhostClustering(gatewayPort: 30000, serviceId: "ContosoMonitoring", clusterId: "dev");
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<ITemperatureSensorClient, FakeTemperatureSensorClient>();
                    services.AddHostedService<TemperatureSensorClientWorker>();
                });
    }
}
