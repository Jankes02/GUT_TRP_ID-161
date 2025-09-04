package com.example.route.controller;

import com.example.route.dto.HeatmapRequest;
import com.example.route.dto.HeatmapResponse;
import com.example.route.service.IRouteService;
import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping(path = "/routes", produces = MediaType.APPLICATION_JSON_VALUE)
public class HeatmapController {

    private final IRouteService routeService;
    public HeatmapController(IRouteService routeService) { this.routeService = routeService; }

    @PostMapping(path = "/heatmap", consumes = MediaType.APPLICATION_JSON_VALUE)
    public HeatmapResponse heatmap(@RequestBody HeatmapRequest request) {
        return routeService.heatmapUsage(request);
    }
}