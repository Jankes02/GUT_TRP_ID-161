from src.models.city import City
from src.models.cityInfoDto import CityInfoDto
from src.repos.cityRepository import CityRepository

from fastapi import HTTPException, status
import math



class CityService:
    def __init__(self, repo: CityRepository):
        self.repo = repo

    def find_by_name(self, name: str) -> City:
        """Find city by name - throws 404 if not found (equivalent to @Transactional(readOnly = true))"""
        city = self.repo.find_by_name(name)
        if not city:
            # Equivalent to Spring's ResponseStatusException(HttpStatus.NOT_FOUND, "City not found")
            raise HTTPException(
                status_code=status.HTTP_404_NOT_FOUND,
                detail="City not found"
            )
        return city

    def add_city(self, city: City) -> City:
        """Add a new city (equivalent to @Transactional)"""
        return self.repo.save(city)

    def compute_info(self, name: str) -> CityInfoDto:
        """Compute city information (equivalent to @Transactional(readOnly = true))"""
        c = self.find_by_name(name)

        # Population handling
        pop = c.population if c.population is not None else 0

        # Category determination
        if pop < 50_000:
            cat = "SMALL"
        elif pop < 250_000:
            cat = "MEDIUM"
        else:
            cat = "LARGE"

        # Name length
        name_len = len(c.name) if c.name is not None else 0

        # Vowel counting (including Polish vowels: ą, ę, ó)
        vowels = sum(1 for ch in c.name.lower() if ch in "aeiouyąęó")

        # Population projection (5 years at 1.2% growth)
        projection = round(pop * math.pow(1 + 0.012, 5))

        return CityInfoDto(
            name=c.name,
            state=c.state,
            population=pop,
            category=cat,
            name_length=name_len,
            vowels=vowels,
            projection=projection
        )

    def get_all_cities(self) -> City:
        return self.repo.find_all()
