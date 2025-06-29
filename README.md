# MokaMetrics

## Architecture

![mokametrics architecture](https://i.imgur.com/DM8wT2j.png)

## Migrations
```bash
dotnet ef migrations add <migration name> -p ..\MokaMetrics.DataAccess -o Migrations -c ApplicationDbContext

dotnet ef database update
```