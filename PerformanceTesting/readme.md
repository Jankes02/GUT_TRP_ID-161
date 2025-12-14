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

## Java

### Java monolith

- `/heatmap` - returns 500
- `/cities` - endpoint doesn't exist, returns 404

### Java microservices

- `/heatmap` - returns 500
- `/cities` - endpoint doesn't exist, returns 404

## .NET

### .NET monolith

Almost none of the endpoint work. There are some db related logs: `MessageText: relation "Cities" does not exist`

- `/cities` - returns 404
- `/city/:city` - returns 404
- `/city/info/:city` - returns 404
- `/routes/heatmap` - returns 400
- `/routes/most-used` - returns 500
- `/routes/shortest` - returns 404

### .NET microservices

- `/cities` - returns 404
- `/routes/heatmap` - returns 500
- `/routes/most-used` - returns 500
- `/routes/shortest` - returns 404
