using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RouteService.Database;
using RouteService.Database.Model;
using RouteService.DTOs;
using RouteService.Services.intf;

namespace RouteService.Services
{
    public class RouteService : IRouteService
    {
        private readonly AppDbContext _dbContext;

        public RouteService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public RouteResponse ShortestByStops(string fromName, string toName, DateTime? date)
        {
            var effectiveDate = date ?? DateTime.Today;

            var cities = _dbContext.Cities.ToList();
            var fromCity = cities.FirstOrDefault(c => c.Name.Equals(fromName, StringComparison.OrdinalIgnoreCase));
            var toCity = cities.FirstOrDefault(c => c.Name.Equals(toName, StringComparison.OrdinalIgnoreCase));

            if (fromCity == null) throw new Exception("City not found: " + fromName);
            if (toCity == null) throw new Exception("City not found: " + toName);

            var connections = _dbContext.Connections
                .Include(c => c.FromCity)
                .Include(c => c.ToCity)
                .Where(c => c.ValidFrom <= effectiveDate && c.ValidTo >= effectiveDate)
                .ToList();

            var adj = new Dictionary<string, List<string>>();
            foreach (var c in connections)
            {
                if (!adj.ContainsKey(c.FromCity.Id))
                    adj[c.FromCity.Id] = new List<string>();
                adj[c.FromCity.Id].Add(c.ToCity.Id);
            }

            var parent = new Dictionary<string, string>();
            var q = new Queue<string>();
            var vis = new HashSet<string>();

            q.Enqueue(fromCity.Id);
            vis.Add(fromCity.Id);

            while (q.Count > 0)
            {
                var cur = q.Dequeue();
                if (cur.Equals(toCity.Id, StringComparison.OrdinalIgnoreCase)) break;

                if (adj.ContainsKey(cur))
                {
                    foreach (var nb in adj[cur])
                    {
                        if (vis.Add(nb))
                        {
                            parent[nb] = cur;
                            q.Enqueue(nb);
                        }
                    }
                }
            }

            if (!fromCity.Id.Equals(toCity.Id, StringComparison.OrdinalIgnoreCase) && !parent.ContainsKey(toCity.Id))
            {
                throw new Exception("No route between " + fromName + " and " + toName + " on " + effectiveDate.ToShortDateString());
            }

            var pathIds = new List<string>();
            var currentId = toCity.Id;
            pathIds.Add(currentId);
            while (!currentId.Equals(fromCity.Id, StringComparison.OrdinalIgnoreCase))
            {
                currentId = parent[currentId];
                pathIds.Add(currentId);
            }
            pathIds.Reverse();

            var idToName = cities.ToDictionary(c => c.Id, c => c.Name);
            var pathNames = pathIds.Select(id => idToName[id]).ToList();

            return new RouteResponse(fromCity.Name, toCity.Name, pathNames.Count - 1, pathNames);
        }

        public string[] FindShortestRouteRandomPoints(int count)
        {
            var r = new Random();
            var points = new Point[count];
            for (int i = 0; i < count; i++) 
            {
                points[i] = new Point("Loc" + i, r.NextDouble() * 90.0, r.NextDouble() * 180.0);
            }

            var route = HeuristicNearestNeighbor(points);
            return route.Select(p => p.Name).ToArray();
        }

        private static List<Point> HeuristicNearestNeighbor(Point[] points)
        {
            var rnd = new Random();
            int start = rnd.Next(points.Length);
            var vis = new bool[points.Length];
            var route = new List<Point>(points.Length);
            Point cur = points[start];
            vis[start] = true;
            route.Add(cur);
            int visited = 1;
            while (visited < points.Length)
            {
                double best = double.PositiveInfinity;
                int bestIdx = -1;
                for (int i = 0; i < points.Length; i++)
                {
                    if (vis[i]) continue;
                    double d = Euclidean(cur, points[i]);
                    if (d < best) { best = d; bestIdx = i; }
                }
                vis[bestIdx] = true;
                cur = points[bestIdx];
                route.Add(cur);
                visited++;
            }
            return route;
        }

        private static double Euclidean(Point a, Point b) => Math.Sqrt(Math.Pow(a.Lat - b.Lat, 2) + Math.Pow(a.Lon - b.Lon, 2));

        internal class Point
        {
            public string Name { get; set; }
            public double Lat { get; set; }
            public double Lon { get; set; }
            public Point(string name, double lat, double lon) { this.Name = name; this.Lat = lat; this.Lon = lon; }
        }

        public List<ConnectionUsageDto> GetMostUsedConnections(DateTime? date)
        {
            var effectiveDate = date ?? DateTime.Today;

            var cities = _dbContext.Cities.ToList();
            var connections = _dbContext.Connections
                .Include(c => c.FromCity)
                .Include(c => c.ToCity)
                .Where(c => c.ValidFrom <= effectiveDate && c.ValidTo >= effectiveDate)
                .ToList();

            var idToCity = cities.ToDictionary(c => c.Id, c => c);

            var edgeByIds = connections.ToDictionary(c => c.FromCity.Id + "->" + c.ToCity.Id, c => c);

            var adj = new Dictionary<string, List<string>>();
            foreach (var c in connections)
            {
                if (!adj.ContainsKey(c.FromCity.Id))
                    adj[c.FromCity.Id] = new List<string>();
                adj[c.FromCity.Id].Add(c.ToCity.Id);
            }

            var usageByConnectionId = new Dictionary<string, long>();

            foreach (var fromCity in cities)
            {
                foreach (var toCity in cities)
                {
                    if (fromCity.Id == toCity.Id) continue;

                    var src = fromCity.Id;
                    var dst = toCity.Id;

                    var parent = new Dictionary<string, string>();
                    var q = new Queue<string>();
                    q.Enqueue(src);
                    parent[src] = src;

                    bool reached = false;
                    while(q.Count > 0 && !reached)
                    {
                        var u = q.Dequeue();
                        if (adj.ContainsKey(u))
                        {
                            foreach (var v in adj[u])
                            {
                                if (!parent.ContainsKey(v))
                                {
                                    parent[v] = u;
                                    if (v == dst) { reached = true; break; }
                                    q.Enqueue(v);
                                }
                            }
                        }
                    }

                    if (!reached) continue;

                    var pathIds = new List<string>();
                    var cur = dst;
                    while (cur != src)
                    {
                        var p = parent[cur];
                        pathIds.Add(p + "->" + cur);
                        cur = p;
                    }
                    pathIds.Reverse();

                    foreach (var edge in pathIds)
                    {
                        var conn = edgeByIds.GetValueOrDefault(edge);
                        if (conn != null)
                        {
                            if (!usageByConnectionId.ContainsKey(conn.Id))
                                usageByConnectionId[conn.Id] = 0;
                            usageByConnectionId[conn.Id]++;
                        }
                    }
                }
            }

            return usageByConnectionId.Select(e =>
            {
                var conn = connections.First(c => c.Id == e.Key);
                return new ConnectionUsageDto(conn.Id, conn.FromCity.Name, conn.ToCity.Name, e.Value);
            })
            .OrderByDescending(dto => dto.UsageCount)
            .ToList();
        }

        public HeatmapResponse HeatmapUsage(HeatmapRequest request)
        {
            if (request == null || request.Requests == null || !request.Requests.Any())
            {
                throw new ArgumentException("requests must not be empty");
            }

            var cities = _dbContext.Cities.ToList();
            var nameToCity = cities.ToDictionary(c => c.Name, c => c, StringComparer.OrdinalIgnoreCase);

            var connections = _dbContext.Connections
                .Include(c => c.FromCity)
                .Include(c => c.ToCity)
                .ToList();

            var adj = new Dictionary<string, List<string>>();
            foreach (var c in connections)
            {
                if (!adj.ContainsKey(c.FromCity.Id))
                    adj[c.FromCity.Id] = new List<string>();
                adj[c.FromCity.Id].Add(c.ToCity.Id);
                if (!request.Directed)
                {
                     if (!adj.ContainsKey(c.ToCity.Id))
                        adj[c.ToCity.Id] = new List<string>();
                     adj[c.ToCity.Id].Add(c.FromCity.Id);
                }
            }

            var byFromName = request.Requests.GroupBy(p => p.From, StringComparer.OrdinalIgnoreCase);
            var edgeCounts = new ConcurrentDictionary<string, long>();
            var missing = new ConcurrentBag<RequestPair>();

            Parallel.ForEach(byFromName, group =>
            {
                var fromName = group.Key;
                if (!nameToCity.TryGetValue(fromName, out var fromCity))
                {
                    foreach(var item in group) missing.Add(item);
                    return;
                }

                var dests = group.ToDictionary(p => p.To, p => p, StringComparer.OrdinalIgnoreCase);
                var targetNames = new HashSet<string>(dests.Keys, StringComparer.OrdinalIgnoreCase);

                var parent = new Dictionary<string, string>();
                var q = new Queue<string>();
                q.Enqueue(fromCity.Id);
                parent[fromCity.Id] = fromCity.Id;

                int remaining = targetNames.Count;

                while (q.Count > 0 && remaining > 0)
                {
                    var u = q.Dequeue();
                    if (adj.ContainsKey(u))
                    {
                        foreach (var v in adj[u])
                        {
                            if (!parent.ContainsKey(v))
                            {
                                parent[v] = u;
                                if (targetNames.Contains(nameToCity[v].Name)) remaining--;
                                q.Enqueue(v);
                                if (remaining == 0) break;
                            }
                        }
                    }
                }

                foreach (var dest in dests.Values)
                {
                    if (!nameToCity.TryGetValue(dest.To, out var toCity) || !parent.ContainsKey(toCity.Id))
                    {
                        missing.Add(dest);
                        continue;
                    }

                    var pathIds = new List<string>();
                    var cur = toCity.Id;
                    while (cur != fromCity.Id)
                    {
                        var p = parent[cur];
                        pathIds.Add(p + "->" + cur);
                        cur = p;
                    }

                    foreach (var edge in pathIds)
                    {
                        edgeCounts.AddOrUpdate(edge, 1, (key, count) => count + 1);
                    }
                }
            });

            var edges = edgeCounts.Select(e =>
            {
                var parts = e.Key.Split(new[] { "->" }, StringSplitOptions.None);
                var fromName = cities.FirstOrDefault(c => c.Id == parts[0])?.Name ?? parts[0];
                var toName = cities.FirstOrDefault(c => c.Id == parts[1])?.Name ?? parts[1];
                return new HeatmapEdgeUsage(fromName, toName, e.Value);
            })
            .OrderByDescending(e => e.Count)
            .ThenBy(e => e.From)
            .ThenBy(e => e.To)
            .ToList();

            return new HeatmapResponse
            {
                TotalRequests = request.Requests.Count,
                Edges = edges,
                Missing = missing.ToList()
            };
        }
    }
}
