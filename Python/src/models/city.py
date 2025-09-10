from typing import Optional
from pydantic import BaseModel

class City(BaseModel):
    id: Optional[str] = None
    name: str
    state: Optional[str] = None  # Added this field
    population: Optional[int] = None
