from locust import HttpUser, task, between
import random

class CityApiUser(HttpUser):
    wait_time = between(1, 3)  # Wait between 1 and 3 seconds between tasks

    # Example city names from your database
    city_names = [
        "New York", "Los Angeles", "Chicago", "Houston", "Miami",
        "Denver", "Seattle", "Atlanta", "Boston", "Phoenix",
        "Kansas City", "Salt Lake City", "Nashville", "Portland", "Buffalo"
    ]

    # Sample city pairs for shortest route tests
    city_pairs = [
        ("Boston", "Miami"),
        ("New York", "Los Angeles"),
        ("Chicago", "Seattle"),
        ("Denver", "Phoenix"),
        ("Houston", "Nashville"),
        ("Atlanta", "Kansas City"),
        ("Phoenix", "Houston")
    ]

    @task(1)
    def health_check(self):
        self.client.get("/health")

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
        # Random date parameter (optional) can be added or omitted in tests
        self.client.get(f"/routes/shortest?fromName={from_city}&toName={to_city}")

    @task(1)
    def get_random_points(self):
        # count query param set to a random number between 1 and 5
        count = random.randint(1, 5)
        self.client.get(f"/routes/random-points?count={count}")

    @task(1)
    def get_most_used_connections(self):
        # Date is optional, sometimes include it
        if random.random() > 0.5:
            self.client.get("/routes/most-used")
        else:
            self.client.get("/routes/most-used?date=2025-11-01")

