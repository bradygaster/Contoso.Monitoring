using System.Threading.Tasks;

namespace Contoso.Monitoring.Grains.Interfaces
{
    public interface ITemperatureSensorGrain : Orleans.IGrainWithStringKey
    {
        Task ReceiveTemperatureReading(TemperatureReading temperatureReading);
    }
}