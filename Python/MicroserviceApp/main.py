from fastapi import FastAPI
import psycopg2
import os

app = FastAPI()

# Read database URL from env
DATABASE_URL = os.getenv("DATABASE_URL")

@app.get("/")
def read_root():
    return {"Hello": "World"}

@app.get("/db-check")
def db_check():
    try:
        conn = psycopg2.connect(DATABASE_URL)
        cur = conn.cursor()
        cur.execute("SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' AND table_type = 'BASE TABLE';")
        result = cur.fetchone()
        cur.close()
        conn.close()
        return {"db_time": result, "version": "microservice"}
    except Exception as e:
        return {"error": str(e)}
