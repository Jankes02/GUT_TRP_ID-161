using System.Collections.Generic;

namespace MonolithicApp.DTOs
{
    public class HeatmapResponse
    {
        public int TotalRequests { get; set; }
        public List<HeatmapEdgeUsage> Edges { get; set; }
        public List<RequestPair> Missing { get; set; }
    }

    public class HeatmapEdgeUsage
    {
        public string From { get; set; }
        public string To { get; set; }
        public long Count { get; set; }

        public HeatmapEdgeUsage(string from, string to, long count)
        {
            From = from;
            To = to;
            Count = count;
        }
    }
}
