package com.example.route.dto;

import java.util.List;

public class RouteResponse {
    private String from;
    private String to;
    private int stops;
    private java.util.List<String> path;

    public RouteResponse(String from, String to, int stops, java.util.List<String> path) {
        this.from = from; this.to = to; this.stops = stops; this.path = path;
    }
    public String getFrom() { return from; }
    public String getTo() { return to; }
    public int getStops() { return stops; }
    public java.util.List<String> getPath() { return path; }
}
