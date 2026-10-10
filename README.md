# Happy Headlines

## Services

| Service       | Port            | Description                                 |
|---------------|-----------------|---------------------------------------------|
| ArticleApi    | 8080 (nginx LB) | Articles, sharded by continent behind nginx |
| CommentApi    | 8081            | Comments, filtered through ProfanityApi     |
| ProfanityApi  | 8082            | Profanity filter                            |
| DraftApi      | 8083            | Article drafts                              |
| PublisherApi  | 8084            | Publishes articles via RabbitMQ             |
| NewsletterApi | –               | Sends immediate and daily newsletters       |

Shared logging, tracing and messaging in `src/ServiceDefaults`.

## Run locally

```sh
docker compose up --build
```

| Tool                                     | URL                    |
|------------------------------------------|------------------------|
| Seq (logs)                               | http://localhost:5342  |
| Zipkin (traces)                          | http://localhost:9411  |
| RabbitMQ                                 | http://localhost:15672 |
| Prometheus                               | http://localhost:9090  |
| Grafana (cache hit ratio, admin/grafana) | http://localhost:3000  |

## Caching

Redis (`localhost:6379`) caches articles and comments.

- **ArticleCache**: a background worker fills it every hour with articles from the last 14 days. Entries expire 14 days
  after publish.
- **CommentCache**: filled on cache miss. Holds comments for the 30 most recently accessed articles where least recently
  used is evicted (sorted set `CommentApi:lru`).

## Diagrams

C4 context and container diagrams are in `docs/diagrams/`.

