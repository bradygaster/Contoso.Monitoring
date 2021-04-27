using System.Threading.Tasks;
using Contoso.Monitoring.Grains.Interfaces;

namespace Contoso.Monitoring.Sensors.Temperature
{
    public interface ITemperatureSensorClient
    {
        Task<TemperatureReading> GetTemperatureReading();
    }
}