-- Measures the latest query with and without the index on Articles.PublishDate.
-- Seeds 100,000 articles, prints EXPLAIN ANALYZE for both, then deletes the seed data.
-- Bash:
--   docker compose exec -T europe-db psql -U postgres -d articledb < scripts/explain-publishdate.sql

INSERT INTO "Articles" ("Id", "Title", "Content", "Author", "PublishDate", "Region")
SELECT gen_random_uuid(), 'Title ' || i, repeat('x', 2000), 'seed', now() - i * interval '1 minute', 4
FROM generate_series(1, 100000) i;
ANALYZE
"Articles";

BEGIN;
DROP INDEX IF EXISTS "IX_Articles_PublishDate";
EXPLAIN
(ANALYZE, BUFFERS)
SELECT "Id", "Title", substring("Content", 1, 200), "Author", "PublishDate", "Region"
FROM "Articles"
ORDER BY "PublishDate" DESC LIMIT 50;
ROLLBACK;

BEGIN;
CREATE INDEX IF NOT EXISTS "IX_Articles_PublishDate" ON "Articles" ("PublishDate");
EXPLAIN
(ANALYZE, BUFFERS)
SELECT "Id", "Title", substring("Content", 1, 200), "Author", "PublishDate", "Region"
FROM "Articles"
ORDER BY "PublishDate" DESC LIMIT 50;
ROLLBACK;

DELETE
FROM "Articles"
WHERE "Author" = 'seed';
