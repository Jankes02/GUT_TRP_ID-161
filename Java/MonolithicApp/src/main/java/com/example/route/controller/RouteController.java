package com.example.route.controller;

import com.example.route.dto.RouteResponse;
import com.example.route.dto.ConnectionUsageDto;
import com.example.route.service.IRouteService;
import jakarta.servlet.http.HttpServletRequest;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDate;
import java.time.OffsetDateTime;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.NoSuchElementException;

@RestController
@RequestMapping(path = "/routes", produces = MediaType.APPLICATION_JSON_VALUE)
public class RouteController {

    private final IRouteService routeService;

    public RouteController(IRouteService routeService) {
        this.routeService = routeService;
    }

    // GET /routes/shortest?fromName=New%20York&toName=Miami
    @GetMapping("/shortest")
    public RouteResponse getShortestRoute(
            @RequestParam("fromName") String fromName,
            @RequestParam("toName") String toName,
            @RequestParam(value = "date", required = false)
            @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate date) {
        return routeService.shortestByStops(fromName, toName, date);
    }

    // (opcjonalnie, jeśli chcesz przetestować drugi serwisowy endpoint)
    // GET /routes/random-points?count=5
    @GetMapping("/random-points")
    public String[] randomPoints(@RequestParam("count") int count) {
        return routeService.findShortestRouteRandomPoints(count);
    }

    // === Proste mapowanie wyjątków na sensowne HTTP ===

    @ResponseStatus(HttpStatus.NOT_FOUND)
    @ExceptionHandler(NoSuchElementException.class)
    public Map<String, Object> handleNotFound(NoSuchElementException ex, HttpServletRequest req) {
        Map<String, Object> body = new HashMap<>();
        body.put("timestamp", OffsetDateTime.now().toString());
        body.put("status", 404);
        body.put("error", "Not Found");
        body.put("message", ex.getMessage());
        body.put("path", req.getRequestURI());
        return body;
    }

    @ResponseStatus(HttpStatus.BAD_REQUEST)
    @ExceptionHandler(IllegalArgumentException.class)
    public Map<String, Object> handleBadRequest(IllegalArgumentException ex, HttpServletRequest req) {
        Map<String, Object> body = new HashMap<>();
        body.put("timestamp", OffsetDateTime.now().toString());
        body.put("status", 400);
        body.put("error", "Bad Request");
        body.put("message", ex.getMessage());
        body.put("path", req.getRequestURI());
        return body;
    }

    @GetMapping("/most-used")
    public List<ConnectionUsageDto> getMostUsedConnections(
            @RequestParam(value = "date", required = false)
            @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate date) {
        return routeService.getMostUsedConnections(date);
    }
    
}