package com.example.route.dto;

public class ConnectionUsageDto {
    private String connectionId;
    private String from;
    private String to;
    private long usageCount;

    public ConnectionUsageDto(String connectionId, String from, String to, long usageCount) {
        this.connectionId = connectionId;
        this.from = from;
        this.to = to;
        this.usageCount = usageCount;
    }

    public String getConnectionId() { return connectionId; }
    public String getFrom() { return from; }
    public String getTo() { return to; }
    public long getUsageCount() { return usageCount; }
}