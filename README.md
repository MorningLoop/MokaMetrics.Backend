# MokaMetrics

> insert description here

# Migrations
```bash
dotnet ef migrations add <migration name> -p ..\MokaMetrics.DataAccess -o Migrations -c ApplicationDbContext

dotnet ef database update
```