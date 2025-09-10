from datetime import datetime

from fastapi import FastAPI, HTTPException, status, Depends
from typing import List, Optional
import uvicorn


from src.models.routes import RouteResponse, ConnectionUsageDto, HeatmapResponse, HeatmapRequest
from src.repos.connectionRepository import ConnectionRepository
from src.services.routeService import RouteService
from src.repos.cityRepository import CityRepository
from src.utils.get_db_connection import get_db_connection
from src.services.cityService import CityService

# Initialize service
repo = CityRepository()
city_service = CityService(repo)


def get_city_service() -> CityService:
    return city_service

connection_repo = ConnectionRepository()
route_service = RouteService(repo, connection_repo)


def get_route_service() -> RouteService:
    return route_service


# FastAPI app
app = FastAPI(title="City API", version="1.0.0")


# Health check endpoint
@app.get("/health")
async def health_check():
    """Health check endpoint"""
    try:
        with get_db_connection() as conn:
            with conn.cursor() as cursor:
                cursor.execute("SELECT 1")
                cursor.fetchone()
        return {"status": "healthy", "database": "connected"}
    except Exception as e:
        raise HTTPException(
            status_code=status.HTTP_503_SERVICE_UNAVAILABLE,
            detail=f"Database connection failed: {str(e)}"
        )

@app.get("/routes/shortest", response_model=RouteResponse)
async def get_shortest_route(
        fromName: str,
        toName: str,
        date: Optional[str] = None,
        service: RouteService = Depends(get_route_service)
):
    """Get shortest route between two cities"""
    try:
        route_date = None
        if date:
            route_date = datetime.strptime(date, "%Y-%m-%d").date()

        return service.shortest_by_stops(fromName, toName, route_date)
    except ValueError as e:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail=str(e))
    except Exception as e:
        raise HTTPException(status_code=status.HTTP_500_INTERNAL_SERVER_ERROR, detail=str(e))


@app.get("/routes/random-points", response_model=List[str])
async def random_points(
        count: int,
        service: RouteService = Depends(get_route_service)
):
    """Generate random points and find shortest route"""
    try:
        return service.find_shortest_route_random_points(count)
    except Exception as e:
        raise HTTPException(status_code=status.HTTP_500_INTERNAL_SERVER_ERROR, detail=str(e))


@app.get("/routes/most-used", response_model=List[ConnectionUsageDto])
async def get_most_used_connections(
        date: Optional[str] = None,
        service: RouteService = Depends(get_route_service)
):
    """Get most used connections"""
    try:
        usage_date = None
        if date:
            usage_date = datetime.strptime(date, "%Y-%m-%d").date()

        return service.get_most_used_connections(usage_date)
    except Exception as e:
        raise HTTPException(status_code=status.HTTP_500_INTERNAL_SERVER_ERROR, detail=str(e))


@app.post("/routes/heatmap", response_model=HeatmapResponse)
async def heatmap(
        request: HeatmapRequest,
        service: RouteService = Depends(get_route_service)
):
    """Generate heatmap data based on route usage"""
    try:
        return service.heatmap_usage(request)
    except ValueError as e:
        raise HTTPException(status_code=status.HTTP_400_BAD_REQUEST, detail=str(e))
    except Exception as e:
        raise HTTPException(status_code=status.HTTP_500_INTERNAL_SERVER_ERROR, detail=str(e))


if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=8002)
