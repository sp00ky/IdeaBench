# Docker Compose

This setup runs the UI and applies migrations into a persistent SQLite volume.

## Build and run

```
docker compose up --build
```

- UI: http://localhost:5188
- OpenAPI: http://localhost:5188/openapi/v1.json

## Stop

```
docker compose down
```

## Reset data

```
docker compose down -v
```

The database lives in the `ideabench-data` volume at `/data/ideabench.db` inside the containers.
