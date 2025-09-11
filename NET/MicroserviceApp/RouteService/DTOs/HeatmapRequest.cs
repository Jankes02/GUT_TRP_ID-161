using System.Collections.Generic;

namespace RouteService.DTOs
{
    public class HeatmapRequest
    {
        public bool Directed { get; set; }
        public int? Parallelism { get; set; }
        public List<RequestPair>? Requests { get; set; }
    }

    public class RequestPair
    {
        public string From { get; set; }
        public string To { get; set; }

        public RequestPair(string from, string to)
        {
            From = from;
            To = to;
        }
    }
}
