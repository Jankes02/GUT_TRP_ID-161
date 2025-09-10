import random
import math
from typing import List, Optional
from datetime import date
from collections import deque, defaultdict

from src.models.routes import RouteResponse, HeatmapRequest, HeatmapResponse, HeatmapEdgeUsage, ConnectionUsageDto
from src.repos.cityRepository import CityRepository
from src.repos.connectionRepository import ConnectionRepository


class Point:
    def __init__(self, name: str, lat: float, lon: float):
        self.name = name
        self.lat = lat
        self.lon = lon


def euclidean_distance(a: Point, b: Point) -> float:
    return math.sqrt((a.lat - b.lat) ** 2 + (a.lon - b.lon) ** 2)


def heuristic_nearest_neighbor(points: List[Point]) -> List[Point]:
    if not points:
        return []

    start_idx = random.randint(0, len(points) - 1)
    visited = [False] * len(points)
    route = []

    current = points[start_idx]
    visited[start_idx] = True
    route.append(current)
    visited_count = 1

    while visited_count < len(points):
        best_distance = float('inf')
        best_idx = -1

        for i, point in enumerate(points):
            if visited[i]:
                continue
            distance = euclidean_distance(current, point)
            if distance < best_distance:
                best_distance = distance
                best_idx = i

        visited[best_idx] = True
        current = points[best_idx]
        route.append(current)
        visited_count += 1

    return route


def generate_random_points(count: int) -> List[Point]:
    points = []
    for i in range(count):
        name = f"Loc{i}"
        lat = random.uniform(0, 90)
        lon = random.uniform(0, 180)
        points.append(Point(name, lat, lon))
    return points


class RouteService:
    def __init__(self, city_repo: CityRepository, conn_repo: ConnectionRepository):
        self.city_repo = city_repo
        self.conn_repo = conn_repo

    def find_shortest_route_random_points(self, count: int) -> List[str]:
        """Generate random points and find shortest route using nearest neighbor heuristic"""
        points = generate_random_points(count)
        route = heuristic_nearest_neighbor(points)
        return [point.name for point in route]

    def shortest_by_stops(self, from_name: str, to_name: str, date_val: Optional[date] = None) -> RouteResponse:
        """Find shortest route by number of stops using BFS"""
        if date_val is None:
            date_val = date.today()

        # Find cities
        from_city = self.city_repo.find_by_name(from_name.lower())
        if not from_city:
            raise ValueError(f"City not found: {from_name}")

        to_city = self.city_repo.find_by_name(to_name.lower())
        if not to_city:
            raise ValueError(f"City not found: {to_name}")

        # Get valid connections for the date
        valid_connections = self.conn_repo.find_all_valid_on(date_val)

        # Build adjacency list
        adj = defaultdict(list)
        for conn in valid_connections:
            adj[conn.from_city_id].append(conn.to_city_id)
            # Add reverse direction for undirected graph (remove if directed)
            adj[conn.to_city_id].append(conn.from_city_id)

        # BFS to find shortest path
        parent = {}
        queue = deque([from_city.id])
        visited = {from_city.id}

        while queue:
            current = queue.popleft()
            if current == to_city.id:
                break

            for neighbor in adj.get(current, []):
                if neighbor not in visited:
                    visited.add(neighbor)
                    parent[neighbor] = current
                    queue.append(neighbor)

        # Check if path exists
        if from_city.id != to_city.id and to_city.id not in parent:
            raise ValueError(f"No route between {from_name} and {to_name} on {date_val}")

        # Reconstruct path
        path_ids = []
        current = to_city.id
        path_ids.append(current)

        while current != from_city.id:
            current = parent[current]
            path_ids.append(current)

        path_ids.reverse()

        # Convert IDs to names
        cities = self.city_repo.find_all()
        id_to_name = {city.id: city.name for city in cities}
        path_names = [id_to_name[city_id] for city_id in path_ids]

        stops = max(len(path_names) - 1, 0)

        return RouteResponse(
            from_city=from_city.name,
            to_city=to_city.name,
            stops=stops,
            path=path_names
        )

    def heatmap_usage(self, request: HeatmapRequest) -> HeatmapResponse:
        """Process heatmap request - simplified version of Java implementation"""
        if not request.requests:
            raise ValueError("Requests must not be empty")

        total = len(request.requests)
        directed = request.directed

        # Load cities and connections
        cities = self.city_repo.find_all()
        name_to_city = {city.name.lower(): city for city in cities}
        id_to_city = {city.id: city for city in cities}

        connections = self.conn_repo.find_all()

        # Build adjacency list
        adj = defaultdict(list)
        for conn in connections:
            adj[conn.from_city_id].append(conn.to_city_id)
            if not directed:
                adj[conn.to_city_id].append(conn.from_city_id)

        edge_counts = defaultdict(int)
        missing = []

        # Process each request
        for req_pair in request.requests:
            from_city = name_to_city.get(req_pair.from_city.lower())
            to_city = name_to_city.get(req_pair.to_city.lower())

            if not from_city or not to_city:
                missing.append(req_pair)
                continue

            # BFS to find path
            parent = {}
            queue = deque([from_city.id])
            parent[from_city.id] = from_city.id

            found = False
            while queue and not found:
                current = queue.popleft()
                if current == to_city.id:
                    found = True
                    break

                for neighbor in adj.get(current, []):
                    if neighbor not in parent:
                        parent[neighbor] = current
                        queue.append(neighbor)

            if not found:
                missing.append(req_pair)
                continue

            # Reconstruct path and count edges
            path_ids = []
            current = to_city.id
            path_ids.append(current)

            while current != from_city.id:
                current = parent[current]
                path_ids.append(current)

            path_ids.reverse()

            # Count edge usage
            for i in range(len(path_ids) - 1):
                edge_key = f"{path_ids[i]}->{path_ids[i + 1]}"
                edge_counts[edge_key] += 1

        # Build response
        edges = []
        for edge_key, count in edge_counts.items():
            from_id, to_id = edge_key.split("->")
            from_city = id_to_city.get(from_id)
            to_city = id_to_city.get(to_id)

            if from_city and to_city:
                edges.append(HeatmapEdgeUsage(
                    from_city=from_city.name,
                    to_city=to_city.name,
                    count=count
                ))

        # Sort by count descending
        edges.sort(key=lambda x: (-x.count, x.from_city, x.to_city))

        return HeatmapResponse(
            total_requests=total,
            edges=edges,
            missing=missing
        )

    def get_most_used_connections(self, date_val: Optional[date] = None) -> List[ConnectionUsageDto]:
        """Get most used connections based on shortest paths between all city pairs"""
        if date_val is None:
            date_val = date.today()

        cities = self.city_repo.find_all()
        valid_connections = self.conn_repo.find_all_valid_on(date_val)

        id_to_city = {city.id: city for city in cities}

        # Build directed adjacency list and connection mapping
        adj = defaultdict(list)
        edge_to_connection = {}

        for conn in valid_connections:
            adj[conn.from_city_id].append(conn.to_city_id)
            edge_key = f"{conn.from_city_id}->{conn.to_city_id}"
            edge_to_connection[edge_key] = conn

        usage_counts = defaultdict(int)

        # For each pair of cities, find shortest path and count connection usage
        for from_city in cities:
            for to_city in cities:
                if from_city.id == to_city.id:
                    continue

                # BFS for shortest path
                parent = {}
                queue = deque([from_city.id])
                parent[from_city.id] = from_city.id

                found = False
                while queue and not found:
                    current = queue.popleft()
                    if current == to_city.id:
                        found = True
                        break

                    for neighbor in adj.get(current, []):
                        if neighbor not in parent:
                            parent[neighbor] = current
                            queue.append(neighbor)

                if not found:
                    continue

                # Reconstruct path and count connection usage
                path_ids = []
                current = to_city.id
                path_ids.append(current)

                while current != from_city.id:
                    current = parent[current]
                    path_ids.append(current)

                path_ids.reverse()

                # Count each connection in the path
                for i in range(len(path_ids) - 1):
                    edge_key = f"{path_ids[i]}->{path_ids[i + 1]}"
                    if edge_key in edge_to_connection:
                        conn = edge_to_connection[edge_key]
                        usage_counts[conn.id] += 1

        # Build result DTOs
        result = []
        for conn_id, count in usage_counts.items():
            # Find the connection
            connection = None
            for conn in valid_connections:
                if conn.id == conn_id:
                    connection = conn
                    break

            if connection:
                from_city = id_to_city[connection.from_city_id]
                to_city = id_to_city[connection.to_city_id]
                result.append(ConnectionUsageDto(
                    connection_id=conn_id,
                    from_city=from_city.name,
                    to_city=to_city.name,
                    usage_count=count
                ))

        # Sort by usage count descending
        result.sort(key=lambda x: -x.usage_count)
        return result
