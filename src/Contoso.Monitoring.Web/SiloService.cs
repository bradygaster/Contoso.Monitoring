using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contoso.Monitoring.Grains.Interfaces;
using Orleans;

namespace Contoso.Monitoring.Web
{
    public class SiloService
    {
        private IGrainFactory _grainFactory;
        private IBuildingDashboardGrain _buildingDashboardGrain;

        public SiloService(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;

            _buildingDashboardGrain = _grainFactory.GetGrain<IBuildingDashboardGrain>(Guid.Empty);
        }

        public async Task<List<MonitoredArea>> GetMonitoredAreas()
        {
            var areas = await _grainFactory.GetGrain<IMonitoredBuildingGrain>(Guid.Empty).GetMonitoredAreas();
            return areas;
        }
    }
}