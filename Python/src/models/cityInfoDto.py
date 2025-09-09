from typing import Optional
from pydantic import BaseModel


class CityInfoDto(BaseModel):
    name: str
    state: Optional[str] = None
    population: int
    category: str  # "SMALL", "MEDIUM", or "LARGE"
    name_length: int
    vowels: int
    projection: int  # 5-year population projection
