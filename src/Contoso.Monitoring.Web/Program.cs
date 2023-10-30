namespace Contoso.Monitoring.Web;

public class Program
{
    static int _delta = 11;

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
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
