## Dependencies:

- docker deamon ~ 27.4

## To start

```sh
docker compose up
```

## containers

| Container Name      | PORT | Type       | Description            |
| ------------------- |------| ---------- |------------------------|
| postgres            | 5432 | Database   |                        |
| python_monolith     | 8000 | Python API | Monolithic application |
| python_microservice | 8001 | Python API | Cities Microservice    |
| python_microservice | 8002 | Python API | Routes Microservice    |

## to connect to db use URL:
```sh
psql postgres://myuser:mypassword@localhost:5432/mydb
```

## to test ig it works fetch to:
```request
GET localhost:PORT/health
```