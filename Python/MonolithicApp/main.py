from fastapi import FastAPI
import psycopg2
import os
from contextlib import contextmanager

app = FastAPI()

DATABASE_URL = os.getenv("DATABASE_URL")


@contextmanager
def get_db_connection():
    """Context manager for database connection"""
    conn = None
    try:
        conn = psycopg2.connect(DATABASE_URL)
        yield conn
    finally:
        if conn:
            conn.close()


@app.get("/")
def read_root():
    return {"Hello": "World"}


@app.get("/db-check")
def db_check():
    try:
        with get_db_connection() as conn:
            with conn.cursor() as cur:
                cur.execute("SELECT NOW();")
                result = cur.fetchone()
                return {"db_time": result[0], "version": "monolith"}
    except Exception as e:
        return {"error": str(e)}


@app.get("/cities")
def get_cities():
    """Fetch all rows from cities table"""
    try:
        with get_db_connection() as conn:
            with conn.cursor() as cur:
                cur.execute("SELECT id, name, state, population FROM cities;")
                rows = cur.fetchall()
                return [
                    {"id": r[0], "name": r[1], "state": r[2], "population": r[3]}
                    for r in rows
                ]
    except Exception as e:
        return {"error": str(e)}


@app.get("/points")
def get_points():
    """Fetch all rows from points table"""
    try:
        with get_db_connection() as conn:
            with conn.cursor() as cur:
                cur.execute("SELECT id, name, lat, lon FROM points;")
                rows = cur.fetchall()
                return [
                    {"id": r[0], "name": r[1], "lat": r[2], "lon": r[3]} for r in rows
                ]
    except Exception as e:
        return {"error": str(e)}
