from locust import HttpUser, task, between

class FastAPIUser(HttpUser):
    # Simulate wait time between requests (realistic traffic)
    wait_time = between(1, 3)

    @task(3)  # weight = 3 (called more often)
    def get_cities(self):
        self.client.get("/cities")

    @task(2)  # weight = 2
    def get_points(self):
        self.client.get("/points")

    @task(1)  # weight = 1
    def db_check(self):
        self.client.get("/db-check")
