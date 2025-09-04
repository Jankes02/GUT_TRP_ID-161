package com.example.route.dto;

import java.util.List;

/**
 * Request payload for computing an edge-usage heatmap.
 */
public class HeatmapRequest {
    private List<RequestPair> requests;
    private boolean directed = true;
    private Integer parallelism; // null or <=0 => use availableProcessors

    public HeatmapRequest() {}

    public List<RequestPair> getRequests() { return requests; }
    public void setRequests(List<RequestPair> requests) { this.requests = requests; }

    public boolean isDirected() { return directed; }
    public void setDirected(boolean directed) { this.directed = directed; }

    public Integer getParallelism() { return parallelism; }
    public void setParallelism(Integer parallelism) { this.parallelism = parallelism; }

    public static class RequestPair {
        private String from;
        private String to;
        public RequestPair() {}
        public RequestPair(String from, String to) { this.from = from; this.to = to; }
        public String getFrom() { return from; }
        public void setFrom(String from) { this.from = from; }
        public String getTo() { return to; }
        public void setTo(String to) { this.to = to; }
    }
}