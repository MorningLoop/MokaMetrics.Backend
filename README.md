# MokaMetrics

## Architecture

![mokametrics architecture](https://imgur.com/a/QbP8A3H)

## Migrations
```bash
dotnet ef migrations add <migration name> -p ..\MokaMetrics.DataAccess -o Migrations -c ApplicationDbContext

dotnet ef database update
```