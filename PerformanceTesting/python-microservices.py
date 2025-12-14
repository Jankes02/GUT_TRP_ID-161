from locust import HttpUser, task, between, events
from datetime import datetime
import random
import csv

# Store response times globally
response_times = []

@events.request.add_listener
def on_request(request_type, name, response_time, response_length, **kwargs):
    response_times.append({
        "request_type": request_type,
        "name": name,
        "response_time_ms": response_time,
        "response_length": response_length
    })

@events.quitting.add_listener
def on_quitting(environment, **kwargs):
    try:
        print('file opened')
        with open(f"./logs/python-microservices/response_times_{datetime.now()}.csv", "w", newline="") as csvfile:
            fieldnames = ["request_type", "name", "response_time_ms", "response_length"]
            writer = csv.DictWriter(csvfile, fieldnames=fieldnames)
            writer.writeheader()
            for entry in response_times:
                writer.writerow(entry)
    except:
        print('FAILURE!')
    print("Response times saved to response_times.csv")

class CityApiUser(HttpUser):
    wait_time = between(1, 3)

    city_names = [
        "New York", "Los Angeles", "Chicago", "Houston", "Miami",
        "Denver", "Seattle", "Atlanta", "Boston", "Phoenix",
        "Kansas City", "Salt Lake City", "Nashville", "Portland", "Buffalo"
    ]

    city_pairs = [
        ("Boston", "Miami"),
        ("New York", "Los Angeles"),
        ("Chicago", "Seattle"),
        ("Denver", "Phoenix"),
        ("Houston", "Nashville"),
        ("Atlanta", "Kansas City"),
        ("Phoenix", "Houston")
    ]

    # Sample request pairs for heatmap (from some of your connections)
    heatmap_request_pairs = [
        {"from_city": "Boston", "to_city": "Buffalo"},
        {"from_city": "Buffalo", "to_city": "New York"},
        {"from_city": "New York", "to_city": "Atlanta"},
        {"from_city": "Atlanta", "to_city": "Miami"},
        {"from_city": "Chicago", "to_city": "Kansas City"},
        {"from_city": "Denver", "to_city": "Salt Lake City"},
        {"from_city": "Los Angeles", "to_city": "Phoenix"}
    ]

    @task(1)
    def get_city(self):
        city = random.choice(self.city_names)
        self.client.get(f"/city/{city}")

    @task(1)
    def get_city_info(self):
        city = random.choice(self.city_names)
        self.client.get(f"/city/info/{city}")

    @task(1)
    def get_all_cities(self):
        self.client.get("/cities")

    @task(1)
    def get_shortest_route(self):
        from_city, to_city = random.choice(self.city_pairs)
        self.client.get(f"/routes/shortest?fromName={from_city}&toName={to_city}")

    @task(1)
    def get_random_points(self):
        count = random.randint(1, 5)
        self.client.get(f"/routes/random-points?count={count}")

    @task(1)
    def get_most_used_connections(self):
        if random.random() > 0.5:
            self.client.get("/routes/most-used")
        else:
            self.client.get("/routes/most-used?date=2025-11-01")

    @task(1)
    def post_heatmap(self):
        payload = {
            "requests": self.heatmap_request_pairs,
            "directed": bool(random.random() > 0.5),
            "parallelism": random.randint(1, 5)
        }
        self.client.post("/routes/heatmap", json=payload)
