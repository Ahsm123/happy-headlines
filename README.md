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

## Fetch and complete issues
### Start
- git switch main; git pull
- gh issue list
- gh issue edit N --add-assignee "@me"
- gh issue develop N --checkout

### Develop and commit
- git add -A
- git commit -m "feat: description (#N)"
- git push

### Pr + merge
- gh pr create --fill --body "Fixed: N"
- gh pr merge --squash --delete-branch
- git switch main; git pull

- Never commit to main
- Powershell use quotes on "@me"
- Use Fixed #N in pr to close issue automatically when merging