To run performance tests run:

```sh
locust -f locustfile.py
```

And go to `http://localhost:8089`.

# All commands:

- `locust -f python-monolith.py`
- `locust -f python-microservices.py`
- `locust -f java-monolith.py`
- `locust -f java-microservices.py`

# Problems

## Java monolith:

- `/heatmap` - returns 500
- `/cities` - endpoint doesn't exist, returns 404

## Java microservicez:

- `/heatmap` - returns 500
- `/cities` - endpoint doesn't exist, returns 404
