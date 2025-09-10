package com.example.route.dto;

import java.util.ArrayList;
import java.util.List;

/**
 * Response payload with aggregated edge usage counts.
 */
public class HeatmapResponse {
    private int totalRequests;
    private List<HeatmapEdgeUsage> edges = new ArrayList<>();
    private List<HeatmapRequest.RequestPair> missing = new ArrayList<>();

    public HeatmapResponse() {}

    public int getTotalRequests() { return totalRequests; }
    public void setTotalRequests(int totalRequests) { this.totalRequests = totalRequests; }

    public List<HeatmapEdgeUsage> getEdges() { return edges; }
    public void setEdges(List<HeatmapEdgeUsage> edges) { this.edges = edges; }

    public List<HeatmapRequest.RequestPair> getMissing() { return missing; }
    public void setMissing(List<HeatmapRequest.RequestPair> missing) { this.missing = missing; }
}