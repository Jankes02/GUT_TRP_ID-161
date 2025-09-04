package com.example.route.service;

import com.example.route.dto.ConnectionUsageDto;
import com.example.route.dto.RouteResponse;
import com.example.route.dto.HeatmapRequest;
import com.example.route.dto.HeatmapResponse;

import java.time.LocalDate;
import java.util.List;

public interface IRouteService {
    String[] findShortestRouteRandomPoints(int count);
    RouteResponse shortestByStops(String fromName, String toName, LocalDate date);
    HeatmapResponse heatmapUsage(HeatmapRequest request);

    List<ConnectionUsageDto> getMostUsedConnections(LocalDate date);
}
