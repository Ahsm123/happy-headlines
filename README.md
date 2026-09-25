# Happy Headlines

Microservice based news platform built with .NET and Docker Compose.

## Services

| Service | Port | Description |
|---|---|---|
| ArticleService | 8080 (nginx LB) | Articles, sharded by continent behind nginx |
| CommentService | 8081 | Comments, filtered through ProfanityService |
| ProfanityService | 8082 | Profanity filter |
| DraftService | 8083 | Article drafts |
| PublisherService | 8084 | Publishes articles via RabbitMQ |
| NewsletterService | – | Sends immediate and daily newsletters |

Shared logging, tracing and messaging in `src/ServiceDefaults`.

## Run locally

```sh
docker compose up --build
```

| Tool | URL |
|---|---|
| Seq (logs) | http://localhost:5342 |
| Zipkin (traces) | http://localhost:9411 |
| RabbitMQ | http://localhost:15672 |

## Diagrams

C4 context and container diagrams are in `docs/diagrams/`.
