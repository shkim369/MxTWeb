# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Internal Blazor Server web app for MxT Biotech. Single project (`MxTWeb/MxTWeb.csproj`) targeting **.NET 9.0** with **MudBlazor 9.x**, EF Core 9, and ASP.NET Core Identity. Deployed as a Linux Docker container behind a reverse proxy.

The app manages biotech lab inventory and workflows: cryopreserved samples, reagents, equipment, chip usage/pressure logs, components/BOMs, and shipments. It also accepts data and file uploads from external clients (e.g. instruments / desktop tools) via API-key-authenticated controllers.

## Commands

Run from the repo root (`MxTWeb.sln`) unless noted; `--project MxTWeb` targets the inner project.

```powershell
# Run locally (HTTPS dev server)
dotnet run --project MxTWeb

# Build / restore
dotnet build
dotnet restore

# EF Core migrations (requires Microsoft.EntityFrameworkCore.Tools, already a PackageReference)
# Both DbContexts live in MxTWeb.Data — specify which one when scaffolding.
dotnet ef migrations add <Name> --project MxTWeb --context ApplicationDbContext
dotnet ef migrations add <Name> --project MxTWeb --context MxTWebContext
dotnet ef database update --project MxTWeb --context ApplicationDbContext
dotnet ef database update --project MxTWeb --context MxTWebContext

# Container build / run (Dockerfile lives in MxTWeb/, build context = MxTWeb/)
docker build -t mxtweb:latest ./MxTWeb
docker compose -f MxTWeb/docker-compose.yml up -d
```

There is no test project and no linter configured — `dotnet build` is the only correctness gate.

## Architecture

### Two DbContexts, two databases

This is the single most important thing to know. The app uses **two separate `IDbContextFactory<T>`s** wired to two different connection strings in `appsettings.json`:

- **`ApplicationDbContext`** → `LoginConnection` → identity store (`AspNetUsers`, roles, `LoginFailureRecords`, `UpdatePosts`). Inherits `IdentityDbContext<ApplicationUser>`.
- **`MxTWebContext`** → `WebContextConnection` → business domain (`Cyropreserved`, `Reagent`, `Equipment`, `ChipUsage`, `ChipPressure`, `Component`, `Bom`, `Shipment`, request tables, etc.).

When adding an entity, decide which context it belongs in first, then register its `DbSet` and add the migration **against the matching `--context`**. The two databases are deployed independently.

Razor components inject `IDbContextFactory<T>` (not `DbContext` directly) because Blazor Server circuits live longer than EF contexts — always create a short-lived context per operation with `await using var db = factory.CreateDbContext()`.

### Identity + admin bootstrap

`Program.cs` seeds the `Admin` role and a hardcoded admin user (`admin@mxtbiotech.com`) on every startup if missing. Authorization uses the `AdminOnly` policy (`RequireRole("Admin")`). Identity is configured with `RequireConfirmedAccount = false` and a custom `AllowedUserNameCharacters` set that includes `@`.

`ActiveUserService` (singleton) tracks live Blazor circuits keyed by circuit ID — used by `AdminMonitor.razor`. `LoginFailureCleanupService` is an `IHostedService` for periodic cleanup of `LoginFailureRecords`.

### API controllers live under `Services/`

Despite the folder name, `Services/*Controller.cs` files (e.g. `ChipPressureController`, `FileUploadController`) are real `[ApiController]` MVC controllers, **not** DI services. They expose `api/<name>` endpoints consumed by external clients (instruments / desktop apps) and are guarded by the custom `[ApiKeyAuth]` attribute, which checks the `X-Api-Key` header against `appsettings.json:ApiKey`.

OpenAPI is generated via `AddOpenApi` and surfaced through **Scalar** at `/scalar/v1` in Development. The OpenAPI document is configured with the `ApiKey` security scheme so the Scalar UI prompts for it.

### Component / page conventions

- Razor pages live in `MxTWeb/Components/Pages/<Domain>Pages/`. Each domain typically has `Index.razor`, `Edit.razor`, `Delete.razor`, and for request-flow entities (`Cyropreserved`, `ChipUsage`) also `RequestIndex.razor`, `RequestEdit.razor`, `RequestApproval.razor`, `RequestDelete.razor`. The "Request" tables (`CyropreservedRequest`, `ChipusageRequest`) drive an approval workflow before the main record is written — preserve this two-step pattern when adding similar features.
- Identity UI lives under `Components/Account/Pages/` (the standard scaffolded set) — do not duplicate those flows elsewhere.
- Adding a new top-level page requires editing `Components/Layout/NavMenu.razor` to expose it.
- Korean (`ko`) UI strings and comments are first-class; `HangulSearchHelper` provides initial-consonant (초성) search used by list pages. Don't strip or "fix" the Korean comments and identifiers.

### File uploads + reverse proxy

`FileUploadController` writes to two paths read from configuration: `UploadPath` (general files) and `UpdateUploadPath` (app update artifacts). In production these map to a NAS volume (`/volume1/docker/web/web-uploads`) bind-mounted into the container at `/docker/web/web-uploads`. `Program.cs` configures `ForwardedHeaders` to honor `X-Forwarded-For`/`Proto` from the reverse proxy — relevant when reading client IPs (e.g. login failure logging).

## Configuration

- `appsettings.json` ships with **plain-text passwords and an API key** committed to the repo (legacy state). When adding new secrets, use **User Secrets** (`UserSecretsId` is already set in the csproj) or environment variables / `docker-compose.yml` overrides instead of adding more secrets to `appsettings.json`.
- Connection strings to be aware of: `LoginConnection`, `WebContextConnection`. The docker-compose file additionally overrides `ConnectionStrings__DefaultConnection`, `ConnectionStrings__MxTWebContext`, and `ConnectionStrings__DockerConnection` — only the first two map onto what `Program.cs` actually reads, so keep names aligned if you touch either side.

## Conventions

- Comments and UI strings are commonly Korean; keep them as-is unless explicitly changing copy.
- C#: PascalCase for types/methods/properties, camelCase for locals/parameters, `_camelCase` for private fields.
- Prefer `IDbContextFactory<T>` + `await using` over injecting a `DbContext` directly into a component.
- When wiring a new API endpoint, follow the existing pattern: `[ApiController]` + `[Route("api/[controller]")]` + `[ApiKeyAuth]`, place the file under `Services/`.
