## Dependencies:

- docker deamon ~ 27.4

## To start

```sh
docker compose up
```

## containers

| Container Name       | PORT | Type       | Description            |
| -------------------- | ---- | ---------- | ---------------------- |
| postgres             | 5432 | Database   |                        |
| python_monolith      | 8000 | Python API | Monolithic application |
| python_city_service  | 8001 | Python API | Cities Microservice    |
| python_route_service | 8002 | Python API | Routes Microservice    |
| java_monolith        | 8003 | Java API   | Monolithic application |
| java_city_service    | 8004 | Java API   | Cities Microservice    |
| java_route_service   | 8005 | Java API   | Routes Microservice    |
| dotnet_monolith      | 8006 | .NET API   | Monolithic application |
| dotnet_city_service  | 8007 | .NET API   | Cities Microservice    |
| dotnet_route_service | 8008 | .NET API   | Routes Microservice    |

## to connect to db use:

```sh
psql postgres://myuser:mypassword@localhost:5432/mydb
```

## to test if it works fetch to:

```request
GET localhost:PORT/health
```
