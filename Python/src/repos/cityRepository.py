from src.models.city import City
from src.utils.get_db_connection import get_db_connection

from typing import Optional, List
import psycopg2
import psycopg2.extras


class CityRepository:
    """Repository class for City entity with string IDs"""

    def find_by_name(self, name: str) -> Optional[City]:
        """Find city by name"""
        with get_db_connection() as conn:
            with conn.cursor(cursor_factory=psycopg2.extras.RealDictCursor) as cursor:
                cursor.execute(
                    "SELECT id, name, state, population FROM cities WHERE LOWER(name) = LOWER(%s)",
                    (name,)
                )
                row = cursor.fetchone()
                if row:
                    return City(**dict(row))
                return None

    def save(self, city: City) -> City:
        """Save or update city"""
        with get_db_connection() as conn:
            with conn.cursor(cursor_factory=psycopg2.extras.RealDictCursor) as cursor:
                if city.id is None:
                    # Generate new ID (you might want a better strategy)
                    cursor.execute("SELECT COUNT(*) FROM cities")
                    count = cursor.fetchone()['count']
                    new_id = f"c{count + 1}"

                    cursor.execute(
                        """
                        INSERT INTO cities (id, name, state, population) 
                        VALUES (%s, %s, %s, %s) 
                        RETURNING id, name, state, population
                        """,
                        (new_id, city.name, city.state, city.population)
                    )
                else:
                    # Update existing city
                    cursor.execute(
                        """
                        UPDATE cities 
                        SET name = %s, state = %s, population = %s 
                        WHERE id = %s
                        RETURNING id, name, state, population
                        """,
                        (city.name, city.state, city.population, city.id)
                    )

                row = cursor.fetchone()
                conn.commit()
                return City(**dict(row))

    def find_by_id(self, city_id: str) -> Optional[City]:  # Changed parameter type
        """Find city by ID"""
        with get_db_connection() as conn:
            with conn.cursor(cursor_factory=psycopg2.extras.RealDictCursor) as cursor:
                cursor.execute(
                    "SELECT id, name, state, population FROM cities WHERE id = %s",
                    (city_id,)
                )
                row = cursor.fetchone()
                if row:
                    return City(**dict(row))
                return None

    def find_all(self) -> List[City]:
        """Find all cities"""
        with get_db_connection() as conn:
            with conn.cursor(cursor_factory=psycopg2.extras.RealDictCursor) as cursor:
                cursor.execute("SELECT id, name, state, population FROM cities ORDER BY name")
                rows = cursor.fetchall()
                return [City(**dict(row)) for row in rows]

    def delete_by_id(self, city_id: str) -> bool:  # Changed parameter type
        """Delete city by ID"""
        with get_db_connection() as conn:
            with conn.cursor() as cursor:
                cursor.execute("DELETE FROM cities WHERE id = %s", (city_id,))
                deleted_count = cursor.rowcount
                conn.commit()
                return deleted_count > 0

    def find_by_state(self, state: str) -> List[City]:
        """Find cities by state"""
        with get_db_connection() as conn:
            with conn.cursor(cursor_factory=psycopg2.extras.RealDictCursor) as cursor:
                cursor.execute(
                    "SELECT id, name, state, population FROM cities WHERE LOWER(state) = LOWER(%s) ORDER BY name",
                    (state,)
                )
                rows = cursor.fetchall()
                return [City(**dict(row)) for row in rows]
