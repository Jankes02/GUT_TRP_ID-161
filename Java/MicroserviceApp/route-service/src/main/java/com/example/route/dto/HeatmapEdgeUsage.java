package com.example.route.dto;

/**
 * A single edge usage entry.
 */
public class HeatmapEdgeUsage {
    private String from;
    private String to;
    private long count;

    public HeatmapEdgeUsage() {}

    public HeatmapEdgeUsage(String from, String to, long count) {
        this.from = from; this.to = to; this.count = count;
    }

    public String getFrom() { return from; }
    public void setFrom(String from) { this.from = from; }

    public String getTo() { return to; }
    public void setTo(String to) { this.to = to; }

    public long getCount() { return count; }
    public void setCount(long count) { this.count = count; }
}