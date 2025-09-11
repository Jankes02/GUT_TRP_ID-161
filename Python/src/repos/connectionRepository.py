from typing import List, Optional
from datetime import date
import psycopg2.extras
from src.models.routes import Connection
from src.models.city import City
from src.utils.get_db_connection import get_db_connection

class ConnectionRepository:
    def find_by_from(self, from_city: City) -> List[Connection]:
        """Find connections from a city"""
        with get_db_connection() as conn:
            with conn.cursor(cursor_factory=psycopg2.extras.RealDictCursor) as cursor:
                cursor.execute(
                    "SELECT id, from_city_id, to_city_id, valid_from, valid_to FROM connections WHERE from_city_id = %s",
                    (from_city.id,)
                )
                rows = cursor.fetchall()
                return [Connection(**dict(row)) for row in rows]

    def find_by_to(self, to_city: City) -> List[Connection]:
        """Find connections to a city"""
        with get_db_connection() as conn:
            with conn.cursor(cursor_factory=psycopg2.extras.RealDictCursor) as cursor:
                cursor.execute(
                    "SELECT id, from_city_id, to_city_id, valid_from, valid_to FROM connections WHERE to_city_id = %s",
                    (to_city.id,)
                )
                rows = cursor.fetchall()
                return [Connection(**dict(row)) for row in rows]

    def find_by_from_and_to(self, from_city: City, to_city: City) -> Optional[Connection]:
        """Find connection between two cities"""
        with get_db_connection() as conn:
            with conn.cursor(cursor_factory=psycopg2.extras.RealDictCursor) as cursor:
                cursor.execute(
                    "SELECT id, from_city_id, to_city_id, valid_from, valid_to FROM connections WHERE from_city_id = %s AND to_city_id = %s",
                    (from_city.id, to_city.id)
                )
                row = cursor.fetchone()
                return Connection(**dict(row)) if row else None

    def find_all_valid_on(self, date_val: date) -> List[Connection]:
        """Find all connections valid on a specific date"""
        with get_db_connection() as conn:
            with conn.cursor(cursor_factory=psycopg2.extras.RealDictCursor) as cursor:
                cursor.execute(
                    "SELECT id, from_city_id, to_city_id, valid_from, valid_to FROM connections WHERE %s BETWEEN valid_from AND valid_to",
                    (date_val,)
                )
                rows = cursor.fetchall()
                return [Connection(**dict(row)) for row in rows]

    def find_all(self) -> List[Connection]:
        """Find all connections"""
        with get_db_connection() as conn:
            with conn.cursor(cursor_factory=psycopg2.extras.RealDictCursor) as cursor:
                cursor.execute("SELECT id, from_city_id, to_city_id, valid_from, valid_to FROM connections")
                rows = cursor.fetchall()
                return [Connection(**dict(row)) for row in rows]
