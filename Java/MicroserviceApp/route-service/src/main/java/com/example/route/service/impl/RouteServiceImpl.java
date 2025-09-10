package com.example.route.service.impl;

import com.example.route.domain.City;
import com.example.route.domain.Connection;
import com.example.route.dto.RouteResponse;
import com.example.route.dto.ConnectionUsageDto;
import com.example.route.dto.HeatmapRequest;
import com.example.route.dto.HeatmapResponse;
import com.example.route.dto.HeatmapEdgeUsage;
import com.example.route.repository.CityRepository;
import com.example.route.repository.ConnectionRepository;
import com.example.route.service.IRouteService;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDate;
import java.util.*;
import java.util.stream.Collectors;

@Service
@Transactional(readOnly = true)
public class RouteServiceImpl implements IRouteService {

    private final CityRepository cityRepo;
    private final ConnectionRepository connRepo;

    public RouteServiceImpl(CityRepository cityRepo, ConnectionRepository connRepo) {
        this.cityRepo = cityRepo; this.connRepo = connRepo;
    }

    static class Point {
        String name; double lat; double lon;
        Point(String name, double lat, double lon) { this.name=name; this.lat=lat; this.lon=lon; }
    }
    private static double euclidean(Point a, Point b) {
        return Math.sqrt((a.lat-b.lat)*(a.lat-b.lat) + (a.lon-b.lon)*(a.lon-b.lon));
    }
    private static List<Point> heuristicNearestNeighbor(Point[] points) {
        Random rnd = new Random();
        int start = rnd.nextInt(points.length);
        boolean[] vis = new boolean[points.length];
        List<Point> route = new ArrayList<>(points.length);
        Point cur = points[start]; vis[start] = true; route.add(cur);
        int visited = 1;
        while (visited < points.length) {
            double best = Double.POSITIVE_INFINITY; int bestIdx = -1;
            for (int i=0;i<points.length;i++) {
                if (vis[i]) continue;
                double d = euclidean(cur, points[i]);
                if (d < best) { best=d; bestIdx=i; }
            }
            vis[bestIdx] = true; cur = points[bestIdx]; route.add(cur); visited++;
        }
        return route;
    }
    private static Point[] randomPoints(int count) {
        Random r = new Random();
        Point[] arr = new Point[count];
        for (int i=0;i<count;i++) arr[i] = new Point("Loc"+i, r.nextDouble()*90.0, r.nextDouble()*180.0);
        return arr;
    }

    @Override
    public String[] findShortestRouteRandomPoints(int count) {
        Point[] input = randomPoints(count);
        List<Point> route = heuristicNearestNeighbor(input);
        return route.stream().map(p -> p.name).toArray(String[]::new);
    }

    @Override
    public RouteResponse shortestByStops(String fromName, String toName, LocalDate date) {
        // domyślnie "dziś", jeśli nie podano
        if (date == null) date = LocalDate.now();

        City from = cityRepo.findByNameIgnoreCase(fromName)
                .orElseThrow(() -> new NoSuchElementException("City not found: " + fromName));
        City to = cityRepo.findByNameIgnoreCase(toName)
                .orElseThrow(() -> new NoSuchElementException("City not found: " + toName));

        List<Connection> validConnections = connRepo.findAllValidOn(date);

        Map<String, List<String>> adj = new HashMap<>();
        for (Connection c : validConnections) {
            // UWAGA: jeśli chcesz kierunkowo – zostaw tylko pierwszą linię
            adj.computeIfAbsent(c.getFrom().getId(), k -> new ArrayList<>()).add(c.getTo().getId());
            adj.computeIfAbsent(c.getTo().getId(), k -> new ArrayList<>()).add(c.getFrom().getId()); // to usuń, jeśli strict directed
        }


        Map<String, String> parent = new HashMap<>();
        Deque<String> q = new ArrayDeque<>();
        Set<String> vis = new HashSet<>();
        q.add(from.getId()); vis.add(from.getId());

        while (!q.isEmpty()) {
            String cur = q.removeFirst();
            if (cur.equals(to.getId())) break;
            for (String nb : adj.getOrDefault(cur, Collections.emptyList())) {
                if (vis.add(nb)) { parent.put(nb, cur); q.addLast(nb); }
            }
        }

        if (!from.getId().equals(to.getId()) && !parent.containsKey(to.getId())) {
            throw new NoSuchElementException("No route between " + fromName + " and " + toName + " on " + date);
        }

        List<String> ids = new ArrayList<>();
        String cur = to.getId();
        ids.add(cur);
        while (!cur.equals(from.getId())) {
            cur = parent.get(cur);
            ids.add(cur);
        }
        Collections.reverse(ids);

        Map<String, String> idToName = cityRepo.findAllById(ids).stream()
                .collect(Collectors.toMap(City::getId, City::getName));
        List<String> path = ids.stream().map(idToName::get).collect(Collectors.toList());
        int stops = Math.max(path.size() - 1, 0);
        return new RouteResponse(from.getName(), to.getName(), stops, path);
    }


    @Override
    @Transactional(readOnly = true)
    public HeatmapResponse heatmapUsage(HeatmapRequest request) {
        if (request == null || request.getRequests() == null || request.getRequests().isEmpty()) {
            throw new IllegalArgumentException("requests must not be empty");
        }
        final int total = request.getRequests().size();
        final boolean directed = request.isDirected();
        final int parallelism = (request.getParallelism() == null || request.getParallelism() <= 0)
                ? Math.max(Runtime.getRuntime().availableProcessors(), 1)
                : request.getParallelism();

        // Preload graph (cities + connections) once
        List<City> cities = cityRepo.findAll();
        Map<String, City> nameToCity = cities.stream()
                .collect(Collectors.toMap(c -> c.getName().toLowerCase(), c -> c));
        Map<String, City> idToCity = cities.stream().collect(Collectors.toMap(City::getId, c -> c));

        List<Connection> conns = connRepo.findAll();
        Map<String, List<String>> out = new HashMap<>();
        Map<String, List<String>> in = new HashMap<>();
        for (Connection c : conns) {
            String f = c.getFrom().getId();
            String t = c.getTo().getId();
            out.computeIfAbsent(f, k -> new ArrayList<>()).add(t);
            in.computeIfAbsent(t, k -> new ArrayList<>()).add(f);
            if (!directed) {
                out.computeIfAbsent(t, k -> new ArrayList<>()).add(f);
                in.computeIfAbsent(f, k -> new ArrayList<>()).add(t);
            }
        }

        // Group queries by 'from' (case-insensitive names)
        Map<String, List<HeatmapRequest.RequestPair>> byFromName = request.getRequests().stream()
                .collect(Collectors.groupingBy(p -> p.getFrom().toLowerCase()));

        // Edge counters keyed by "fromId->toId"
        Map<String, Long> edgeCounts = new java.util.concurrent.ConcurrentHashMap<>();
        List<HeatmapRequest.RequestPair> missing = java.util.Collections.synchronizedList(new ArrayList<>());

        // Parallel over unique sources
        java.util.concurrent.ForkJoinPool pool = new java.util.concurrent.ForkJoinPool(parallelism);
        try {
            pool.submit(() -> byFromName.entrySet().parallelStream().forEach(entry -> {
                String fromNameKey = entry.getKey();
                City fromCity = nameToCity.get(fromNameKey);
                if (fromCity == null) {
                    // All pairs for this source are missing
                    synchronized (missing) {
                        missing.addAll(entry.getValue());
                    }
                    return;
                }
                String fromId = fromCity.getId();

                // For multiplicity, group by destination name and count duplicates
                Map<String, Long> destNameToCount = entry.getValue().stream()
                        .collect(Collectors.groupingBy(p -> p.getTo().toLowerCase(), Collectors.counting()));

                Set<String> targetIds = new HashSet<>();
                Map<String, String> destNameToId = new HashMap<>();
                for (String destNameKey : destNameToCount.keySet()) {
                    City destCity = nameToCity.get(destNameKey);
                    if (destCity != null) {
                        targetIds.add(destCity.getId());
                        destNameToId.put(destNameKey, destCity.getId());
                    } else {
                        // individual missing pairs with unknown dest
                        synchronized (missing) {
                            long k = destNameToCount.get(destNameKey);
                            for (long i = 0; i < k; i++) {
                                missing.add(new HeatmapRequest.RequestPair(fromCity.getName(), destNameKey));
                            }
                        }
                    }
                }

                if (targetIds.isEmpty()) return;

                // BFS (unweighted shortest path by edges) from 'fromId' until all targets reached
                Map<String, String> parent = new HashMap<>();
                java.util.ArrayDeque<String> q = new java.util.ArrayDeque<>();
                q.add(fromId);
                parent.put(fromId, fromId);
                int remaining = targetIds.contains(fromId) ? targetIds.size() - 1 : targetIds.size();

                while (!q.isEmpty() && remaining > 0) {
                    String u = q.poll();
                    List<String> nbrs = out.getOrDefault(u, java.util.Collections.emptyList());
                    for (String v : nbrs) {
                        if (!parent.containsKey(v)) {
                            parent.put(v, u);
                            if (targetIds.contains(v)) remaining--;
                            q.add(v);
                            if (remaining == 0) break;
                        }
                    }
                }

                // For each destination, reconstruct path and add edge counts * multiplicity
                for (Map.Entry<String, Long> destEntry : destNameToCount.entrySet()) {
                    String destNameKey = destEntry.getKey();
                    Long multiplicity = destEntry.getValue();
                    String destId = destNameToId.get(destNameKey);
                    if (destId == null) continue; // already accounted as missing

                    // If unreachable
                    if (!parent.containsKey(destId) && !fromId.equals(destId)) {
                        synchronized (missing) {
                            for (long i = 0; i < multiplicity; i++) {
                                missing.add(new HeatmapRequest.RequestPair(fromCity.getName(), idToCity.get(destId).getName()));
                            }
                        }
                        continue;
                    }

                    // Reconstruct path ids
                    java.util.List<String> ids = new java.util.ArrayList<>();
                    String cur = destId;
                    ids.add(cur);
                    while (!cur.equals(fromId)) {
                        cur = parent.get(cur);
                        if (cur == null) break; // safety
                        ids.add(cur);
                    }
                    java.util.Collections.reverse(ids);

                    // Increment edge counts for each step
                    for (int i = 0; i < ids.size() - 1; i++) {
                        String a = ids.get(i), b = ids.get(i + 1);
                        String key = a + "->" + b;
                        edgeCounts.merge(key, multiplicity, Long::sum);
                    }
                }
            })).join();
        } finally {
            pool.shutdown();
        }

        // Build response entries with city NAMES (not ids)
        java.util.List<HeatmapEdgeUsage> entries = new java.util.ArrayList<>();
        for (Map.Entry<String, Long> e : edgeCounts.entrySet()) {
            String[] parts = e.getKey().split("->", 2);
            City cf = idToCity.get(parts[0]);
            City ct = idToCity.get(parts[1]);
            String fromName = (cf != null ? cf.getName() : parts[0]);
            String toName   = (ct != null ? ct.getName() : parts[1]);
            entries.add(new HeatmapEdgeUsage(fromName, toName, e.getValue()));
        }
        // Sort by descending count, then by from/to for stability
        entries.sort(java.util.Comparator.comparingLong(HeatmapEdgeUsage::getCount).reversed()
                .thenComparing(HeatmapEdgeUsage::getFrom)
                .thenComparing(HeatmapEdgeUsage::getTo));

        HeatmapResponse resp = new HeatmapResponse();
        resp.setTotalRequests(total);
        resp.setEdges(entries);
        resp.setMissing(missing);
        return resp;
    }

    @Override
    public List<ConnectionUsageDto> getMostUsedConnections(LocalDate date) {
        if (date == null) date = LocalDate.now();

        // 1) Załaduj miasta i połączenia ważne w danym dniu
        List<City> cities = cityRepo.findAll();
        List<Connection> validConnections = connRepo.findAllValidOn(date);

        // Mapy pomocnicze
        Map<String, City> idToCity = cities.stream()
                .collect(Collectors.toMap(City::getId, c -> c));

        // Indeks połączeń po kluczu "fromId->toId" (D I R E C T E D)
        Map<String, Connection> edgeByIds = new HashMap<>();
        for (Connection c : validConnections) {
            String key = c.getFrom().getId() + "->" + c.getTo().getId();
            edgeByIds.put(key, c);
        }

        // Adjacency (wyjścia) tylko dla ważnych połączeń w tym dniu
        Map<String, List<String>> out = new HashMap<>();
        for (Connection c : validConnections) {
            String f = c.getFrom().getId();
            String t = c.getTo().getId();
            out.computeIfAbsent(f, k -> new ArrayList<>()).add(t);
        }

        // 2) Liczniki użycia krawędzi
        Map<String, Long> usageByConnectionId = new HashMap<>();

        // 3) Dla każdej pary (A,B), A != B: znajdź ścieżkę najkrótszą (BFS po "out")
        for (City from : cities) {
            for (City to : cities) {
                if (from.equals(to)) continue;

                String src = from.getId();
                String dst = to.getId();

                // BFS po grafie krawędzi ważnych na 'date'
                Map<String, String> parent = new HashMap<>();
                ArrayDeque<String> q = new ArrayDeque<>();
                q.add(src);
                parent.put(src, src);

                boolean reached = false;
                while (!q.isEmpty() && !reached) {
                    String u = q.poll();
                    for (String v : out.getOrDefault(u, Collections.emptyList())) {
                        if (!parent.containsKey(v)) {
                            parent.put(v, u);
                            if (v.equals(dst)) { reached = true; break; }
                            q.add(v);
                        }
                    }
                }

                // Jeśli nieosiągalny, pomijamy
                if (!reached && !src.equals(dst)) continue;

                // Odtwórz ścieżkę (ids)
                List<String> ids = new ArrayList<>();
                String cur = dst;
                ids.add(cur);
                while (!cur.equals(src)) {
                    cur = parent.get(cur);
                    if (cur == null) { // safety
                        ids.clear();
                        break;
                    }
                    ids.add(cur);
                }
                if (ids.isEmpty()) continue;
                Collections.reverse(ids);

                // Zlicz użycie każdej krawędzi w ścieżce
                for (int i = 0; i < ids.size() - 1; i++) {
                    String a = ids.get(i);
                    String b = ids.get(i + 1);
                    Connection edge = edgeByIds.get(a + "->" + b);
                    if (edge != null) {
                        usageByConnectionId.merge(edge.getId(), 1L, Long::sum);
                    }
                }
            }
        }

        // 4) Zbuduj DTO i posortuj malejąco po usage
        return usageByConnectionId.entrySet().stream()
                .map(e -> {
                    Connection c = edgeByIds.values().stream()
                            .filter(x -> x.getId().equals(e.getKey()))
                            .findFirst().orElse(null);
                    if (c == null) return null;
                    return new ConnectionUsageDto(
                            c.getId(),
                            idToCity.get(c.getFrom().getId()).getName(),
                            idToCity.get(c.getTo().getId()).getName(),
                            e.getValue()
                    );
                })
                .filter(Objects::nonNull)
                .sorted((a, b) -> Long.compare(b.getUsageCount(), a.getUsageCount()))
                .collect(Collectors.toList());
    }
    
}
