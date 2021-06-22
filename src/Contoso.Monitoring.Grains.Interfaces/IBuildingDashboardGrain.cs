using System.Threading.Tasks;
using Orleans;

namespace Contoso.Monitoring.Grains.Interfaces
{
    public interface IBuildingDashboardGrain : IGrainWithGuidKey
    {
        Task UpdateBuildingDashboard(TemperatureReading reading);
    }
}