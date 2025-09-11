namespace RouteService.DTOs
{
    public class ConnectionUsageDto
    {
        public string ConnectionId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public long UsageCount { get; set; }

        public ConnectionUsageDto(string connectionId, string from, string to, long usageCount)
        {
            ConnectionId = connectionId;
            From = from;
            To = to;
            UsageCount = usageCount;
        }
    }
}
