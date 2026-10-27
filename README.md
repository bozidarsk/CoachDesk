# CoachDesk

CoachDesk is a web app that connects coaches and clients. Coaches publish the hours they are free and the membership plans they offer. Clients find a coach, buy a plan and book sessions in the coach's free time.

It is an ASP.NET Core MVC project (Razor views, Entity Framework Core, SQL Server).

## Contents

- [Features](#features)
- [Tech stack](#tech-stack)
- [Getting started](#getting-started)
- [Demo data](#demo-data)
- [Database migrations](#database-migrations)
- [Troubleshooting](#troubleshooting)

## Features

- **Accounts and roles.** Register and log in as a client or a coach (ASP.NET Core Identity, role-based access).
- **Account page.** Change your name, bio, email (which is also your login) and password, or delete your account.
- **Coaches.**
  - A public profile page.
  - A dashboard with their clients, memberships and next sessions.
  - Weekly free hours (the times clients can book).
  - Membership plans with a price, a number of sessions and a validity period (create, edit, delete).
- **Clients.**
  - Search for coaches by availability and by plan price, duration and sessions included.
  - Buy a membership plan.
  - Book sessions on a calendar that only offers free times, and cancel them.
  - A dashboard with memberships (with expiry dates) and pending sessions.
- **Landing page** for visitors and **custom pages** for http 4xx and 5xx errors.

## Tech stack

- ASP.NET Core MVC (Razor views, Bootstrap 5)
- Entity Framework Core, Code First with migrations
- SQL Server
- ASP.NET Core Identity

## Getting started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) 10.0 (check with `dotnet --version`)
- An SQL Server instance. The simplest way on any OS is Docker (see below). On Windows you can also use SQL Server or LocalDB.
- [Docker](https://docs.docker.com/get-docker/), if you use the container
- `dotnet-ef`, the EF Core command line tool, for migrations:

  ```bash
  dotnet tool install --global dotnet-ef
  ```

### 1. Get the code

```bash
git clone https://github.com/bozidarsk/CoachDesk
cd CoachDesk
```

### 2. Start SQL Server

Skip this step if you already have a SQL Server you want to use. Otherwise start one in Docker:

```bash
docker container create --name mssql-server -p 1433:1433 -e ACCEPT_EULA=Y -e MSSQL_SA_PASSWORD=<SA_PASSWORD> mcr.microsoft.com/mssql/server:2025-latest
docker container start mssql-server
```

- SQL Server refuses weak passwords: at least 8 characters, with upper and lower case letters and a digit or symbol.
- Check that it is running with `docker ps`.
- Later, start it again with `docker container start mssql-server`.

### 3. Configure the connection string

`appsettings.json` and `appsettings.Development.json` both contain a default `ConnectionStrings:DefaultConnection`.

To use a different server or password, override the value with an environment variable instead of editing the files. Environment variables win over both `appsettings` files. Use a double underscore in place of `:`.

```bash
# Linux / macOS
export ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=CoachDesk;User Id=sa;Password=<SA_PASSWORD>;Encrypt=False;TrustServerCertificate=True;"
```

```powershell
# Windows PowerShell
$env:ConnectionStrings__DefaultConnection = "Server=(localdb)\MSSQLLocalDB;Database=CoachDesk;Trusted_Connection=True;TrustServerCertificate=True;"
```

```bat
:: Windows cmd
set ConnectionStrings__DefaultConnection=Server=localhost,1433;Database=CoachDesk;User Id=sa;Password=<SA_PASSWORD>;Encrypt=False;TrustServerCertificate=True;
```

`Trusted_Connection=True` (Windows authentication) only works when the app and SQL Server are on Windows. For SQL Server in Docker, or on Linux, use `User Id` and `Password`.

### 4. Create the database

In the Development environment the app applies pending migrations and loads the [demo data](#demo-data) when it starts, so you can go straight to step 5.

To apply the migrations yourself, run this from the solution folder:

```bash
dotnet ef database update --startup-project src/CoachDesk.Web/CoachDesk.Web.csproj --project src/CoachDesk.Data/CoachDesk.Data.csproj
```

### 5. Build and run

```bash
dotnet restore
dotnet build
dotnet run
```

Open the URL printed in the console (for example `https://localhost:8000`). If the browser warns about the development certificate, run `dotnet dev-certs https --trust` (Windows and macOS).

## Demo data

In Development the app also creates demo accounts, plans, memberships and sessions. It only does this when the database has no coaches or clients yet. Dates are relative to the day you run it, so there are always upcoming sessions.

All demo accounts use the password `Passw0rd!`. This is for local development only.

| Email | Role | What it shows |
|---|---|---|
| `marta.keller@coachdesk.test` | Coach | 3 plans (one with unlimited sessions), 3 weekly hour blocks, upcoming sessions |
| `daniel.okafor@coachdesk.test` | Coach | A plan that never expires, two blocks on the same day |
| `sofia.lindqvist@coachdesk.test` | Coach | No bio, evening hours, a client with an expired membership |
| `alex.novak@coachdesk.test` | Client | An active plan and sessions with two coaches |
| `jamie.reyes@coachdesk.test` | Client | A plan that expires in 3 days, and one that never expires |
| `priya.nair@coachdesk.test` | Client | A 10-session pass with sessions before and after today |
| `tomas.berg@coachdesk.test` | Client | An expired membership, nothing upcoming |
| `nina.costa@coachdesk.test` | Client | A brand new account with no data |

To start again from scratch, drop the database (`dotnet ef database drop`) and run the app again.

## Database migrations

Migrations are the versioned scripts that turn the C# entities into the SQL Server schema. Create a new one every time you change an entity or the model configuration, and commit the generated files in the `Migrations` folder.

You can use either tool. They do the same thing.

### With dotnet-ef (command line)

Run these from the solution folder with `--startup-project src/CoachDesk.Web/CoachDesk.Web.csproj` and `--project src/CoachDesk.Data/CoachDesk.Data.csproj`.

```bash
# create a migration from the current model
dotnet ef migrations add <MigrationName>

# list migrations and whether they are applied
dotnet ef migrations list

# apply all pending migrations (creates the database if it does not exist)
dotnet ef database update

# move the database to a specific migration (use 0 to undo every migration)
dotnet ef database update <MigrationName>
dotnet ef database update 0

# delete the most recent migration (it must not be applied to the database)
dotnet ef migrations remove

# generate SQL instead of applying it (add --idempotent for a script that is safe to re-run)
dotnet ef migrations script --idempotent -o migrate.sql

# delete the database
dotnet ef database drop
```

### With the Package Manager Console (Visual Studio)

Open it from **Tools > NuGet Package Manager > Package Manager Console**.

```powershell
# create a migration from the current model
Add-Migration <MigrationName>

# list migrations
Get-Migration

# apply all pending migrations (creates the database if it does not exist)
Update-Database

# move the database to a specific migration (use 0 to undo every migration)
Update-Database <MigrationName>
Update-Database 0

# delete the most recent migration (it must not be applied to the database)
Remove-Migration

# generate SQL instead of applying it
Script-Migration -Idempotent

# delete the database
Drop-Database
```

### Good to know

- Both tools read the same configuration as the app, so the environment variable override from step 3 applies to them as well. They run in the `Development` environment unless you say otherwise.
- `migrations remove` only works for a migration that has not been applied. If it has, first run `database update <PreviousMigration>`, then remove it.
- The CLI needs the `Microsoft.EntityFrameworkCore.Design` package in the startup project.

## Troubleshooting

- **`The target principal name is incorrect. Cannot generate SSPI context.`** The connection string uses `Trusted_Connection=True` against a SQL Server on Linux or in Docker. Switch to `User Id=sa;Password=...`.
- **`Login failed for user 'sa'`.** The password in the connection string differs from the `MSSQL_SA_PASSWORD` the container was created with. To see the container's value: `docker inspect mssql-server --format '{{range .Config.Env}}{{println .}}{{end}}' | grep MSSQL`
- **`A network-related or instance-specific error`, or connection refused.** SQL Server is not running (`docker ps`, `docker start mssql-server`) or port 1433 is used by something else.
- **`Could not execute because the specified command or file was not found` (for `dotnet ef`).** Install the tool with `dotnet tool install --global dotnet-ef`, and make sure `~/.dotnet/tools` is on your `PATH`.
