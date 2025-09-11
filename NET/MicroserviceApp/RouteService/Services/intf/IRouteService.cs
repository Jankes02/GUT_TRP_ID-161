using RouteService.DTOs;
using System.Collections.Generic;
using System;

namespace RouteService.Services.intf
{
    public interface IRouteService
    {
        RouteResponse ShortestByStops(string fromName, string toName, DateTime? date);
        string[] FindShortestRouteRandomPoints(int count);
        List<ConnectionUsageDto> GetMostUsedConnections(DateTime? date);
        HeatmapResponse HeatmapUsage(HeatmapRequest request);
    }
}
