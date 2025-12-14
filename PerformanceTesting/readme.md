To run performance tests run:

```sh
locust -f <script_name>.py
```

And go to `http://localhost:8089`.

# Locust setup in GUI

- Max concurrent users: 10000
- Users ramp up: 100 users per second
- Time: 5 minutes (5m)

Waiting period is set to 1-3 seconds. It's how frequent user do requests, that's why requests concurrency (RPS) changes over time.

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
