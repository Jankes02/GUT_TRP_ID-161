## Dependencies:

- docker deamon ~ 27.4

## To start

```sh
docker compose up
```

## ports

| Container Name      | Port Mapping | Type       |
| ------------------- | ------------ | ---------- |
| postgres            | 5432:5432    | Database   |
| python_monolith     | 8000:8000    | Python API |
| python_microservice | 8001:8001    | Python API |

## to connect to db use URL:
```sh
psql postgres://myuser:mypassword@localhost:5432/mydb
```