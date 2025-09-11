from pydantic import BaseModel
from typing import Optional, List
from datetime import date

class Connection(BaseModel):
    id: str
    from_city_id: str
    to_city_id: str
    valid_from: date
    valid_to: date

class ConnectionUsageDto(BaseModel):
    connection_id: str
    from_city: str
    to_city: str
    usage_count: int

class HeatmapEdgeUsage(BaseModel):
    from_city: str
    to_city: str
    count: int

class RequestPair(BaseModel):
    from_city: str
    to_city: str

class HeatmapRequest(BaseModel):
    requests: List[RequestPair]
    directed: bool = True
    parallelism: Optional[int] = None

class HeatmapResponse(BaseModel):
    total_requests: int
    edges: List[HeatmapEdgeUsage]
    missing: List[RequestPair]

class RouteResponse(BaseModel):
    from_city: str
    to_city: str
    stops: int
    path: List[str]
