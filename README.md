# Gift of the Givers — Relief Platform

An ASP.NET Core 9.0 MVC web application for the **Gift of the Givers Foundation**: relief
projects, donations, volunteer sign-ups and an employee portal.

## Tech stack

| Layer          | Technology                                          |
|----------------|-----------------------------------------------------|
| Web            | ASP.NET Core MVC (.NET 9), Razor views               |
| Data           | Entity Framework Core 9 + SQLite (swap provider for SQL Server / Azure SQL) |
| UI             | Bootstrap 5.3 + Bootstrap Icons                      |
| Serverless     | Azure Functions (isolated worker, HTTP triggers)     |
| Shared library | `GiftOfTheGivers.Helpers` (NuGet-packable)           |
| Tests          | xUnit (`GiftOfTheGivers.Tests`)                      |
| CI             | GitHub Actions + `azure-pipelines.yml` (Azure DevOps)|

## Solution structure

```
GiftOfTheGivers/            Web app (MVC)
Function/                   Azure Functions: TaxCertificate, LogProjectUpdate
GiftOfTheGivers.Helpers/    Shared helper library (packaged as a NuGet package)
GiftOfTheGivers.Tests/      xUnit tests
docs/AZURE-SETUP.md         Step-by-step Azure deployment / DevOps guide
azure-pipelines.yml         Azure Pipelines CI (restore, build, test, pack)
```

## Getting started

> Requires the .NET 9 SDK (any 9.0.x) and internet access to nuget.org for package restore.

```bash
git clone <repo-url>
cd gifts
dotnet run --project GiftOfTheGivers.csproj
```

Or open `GiftOfTheGivers.sln` in Visual Studio and press **F5**.

On first run the app creates `giftsofthegivers.db` (SQLite, ignored by git) and seeds it
with relief projects and project updates.

## Features

### Implemented
- [x] Static site: Home, About, Contact, Privacy
- [x] Relief Projects listing + details page (data-driven from the database)
- [x] Domain models: `Project`, `ProjectUpdate`, `Donation`, `Volunteer`, `ContactMessage`
- [x] EF Core `AppDbContext` + automatic database creation & seeding on startup
- [x] Donation flow: quick-donate on landing page → full checkout form → saved to DB →
      confirmation page with reference number (e.g. `GTG-20260924-K7QX2M`) and
      placeholder tax certificate
- [x] Volunteer sign-up form with validation → saved to DB
- [x] Authentication & roles: ASP.NET Core Identity — register, login, logout,
      lockout protection, `Employee` role guarding the portal
- [x] Employee portal: live dashboard (projects, volunteers, updates, donations),
      recent donations & sign-ups, volunteer approvals, posting project updates
      (with optional progress-bar update)

### Seeded accounts
| Role     | Email               | Password      |
|----------|---------------------|---------------|
| Employee | `employee@gotg.org` | `Employee123` |

### Roadmap
- [ ] Contact form → save messages
- [ ] Migrations instead of `EnsureCreated`
- [ ] Donation history for logged-in donors

## Project structure

```
Controllers/     MVC controllers (Home, Account, Projects, Donation, Volunteer, Employee)
Data/            AppDbContext + database seeder
Models/          Domain entities
Views/           Razor views (pages)
wwwroot/         Static assets (css, js, lib)
```
