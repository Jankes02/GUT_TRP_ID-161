using System.Collections.Generic;

namespace RouteService.DTOs
{
    public class RouteResponse
    {
        public string From { get; set; }
        public string To { get; set; }
        public int Stops { get; set; }
        public List<string> Path { get; set; }

        public RouteResponse(string from, string to, int stops, List<string> path)
        {
            From = from;
            To = to;
            Stops = stops;
            Path = path;
        }
    }
}
