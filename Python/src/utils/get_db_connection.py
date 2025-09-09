from contextlib import contextmanager
import psycopg2
import os



@contextmanager
def get_db_connection():
    """Context manager for database connection"""
    database_url = os.getenv("DATABASE_URL")
    conn = None
    try:
        conn = psycopg2.connect(database_url)
        yield conn
    finally:
        if conn:
            conn.close()
