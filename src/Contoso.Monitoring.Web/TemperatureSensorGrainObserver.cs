namespace Contoso.Monitoring.Web;

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

    public int GetHashCode([System.Diagnostics.CodeAnalysis.DisallowNull] ITemperatureSensorGrainObserver obj)
        => (obj as TemperatureSensorGrainObserver).MachineName.ToLower().GetHashCode();

    public Task OnTemperatureReadingReceived(TemperatureSensor reading)
    {
        _hub.Clients.All.OnTemperatureReadingReceived(reading);
        return Task.CompletedTask;
    }
}
