# Inventory Service (Sample)

A production-oriented **.NET** microservice for the **Inventory** domain, built with a layered architecture (**API**, **Application**, **Domain**, **Persistence**) and **CQRS**. It includes **integration** and **unit** tests. In a real microservice landscape, **authentication** would be a separate service; for simplicity in this sample it lives in the same solution. The service targets **MS‑SQL** and seeds a single admin user for quick login.

---

## ✨ Quick Start

```bash
# 1) Restore & build
dotnet restore
dotnet build

# 2) Apply migrations (creates DB if missing)
#    Ensure ConnectionStrings:Main points to a reachable MS‑SQL instance
dotnet ef database update --project Infrastructure  --startup-project Api

# 3) Run the API
dotnet run --project src/Inv.Api

# 4) Open Swagger
#    http://localhost:5000/swagger  (or the port shown in the console)
```

**Demo login**  
- **Username:** `admin@admin.com`  
- **Password:** `password`  

> Credentials are for demo/interview purposes only—do **not** reuse in non‑demo environments.

---

## ✅ Prerequisites

- **.NET SDK 8** (or the version in `global.json`)
- **MS‑SQL Server**
  - LocalDB: `(localdb)\MSSQLLocalDB`, or
  - A full SQL Server instance (on‑prem or Docker)
- Ability to create a database (Windows auth or SQL auth)

---

## 🧱 Tech Stack

- **ASP.NET Core** (controllers, middleware, Swagger)
- **EF Core** (code‑first, migrations) against **MS‑SQL**
- **CQRS** in the **Application** layer (commands/queries + validators)
- **xUnit** for integration tests (plus some unit tests)

---

## 🧭 Scope & Boundaries

- **Context:** This service represents the **Inventory** bounded context within a larger microservice architecture.  
- **Owns:** stock, availability, reservations per SKU/warehouse.  
- **Not owned:** pricing, product metadata, checkout/ordering (belong to other services in a full system).  
- **Simplifications (for the sample):**
  - Authentication is implemented **in-solution** (would be its own service in production).  
  - Database seeding creates one admin user for quick logins.  
- **Storage:** MS‑SQL via EF Core migrations.

---

## ⚙️ Configuration

Settings are loaded from the standard ASP.NET Core providers:

- `appsettings.json`
- `appsettings.{Environment}.json` (e.g., `Development`, `Test`, `Production`)
- Environment variables (e.g., `ConnectionStrings__Main`)
- (Optional) additional `AppSettings.{Environment}.json`, if present

### Connection String

Set **`ConnectionStrings:Main`** to your MS‑SQL instance.

**LocalDB (Windows):**
```json
"ConnectionStrings": {
  "Main": "Server=(localdb)\\MSSQLLocalDB;Database=Inventory;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

**SQL Server (username/password):**
```json
"ConnectionStrings": {
  "Main": "Server=localhost,1433;Database=Inventory;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True"
}
```

### Environment

Set via `ASPNETCORE_ENVIRONMENT` (e.g., `Development`, `Test`, `Production`).

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/Inv.Api
```

---

## 🗃️ Database & Seeding

- On first run, **migrations** create the schema.  
- The seed step creates **one** admin user:
  - `admin@admin.com` / `password`
- Some interview‑only code paths may drop/recreate the DB for repeatable demos—these lines are clearly commented and should be disabled in real environments.

---

## 🔬 Tests

- **Integration tests** boot the API and call endpoints end‑to‑end against a **real SQL database**.
- **Unit tests** cover selected behaviors in isolation.  
- Tests are **not exhaustive** due to time constraints.
- Some test scaffolding/comments were **AI‑assisted** to accelerate delivery.

Run:
```bash
dotnet test
```

> The integration test factory can override the environment to `Test` and supply a **fresh database** per run. See the test base class for details.

---

## 📁 Project Structure (high level)

```
src/
  Inv.Api/            # API layer (endpoints, filters, middleware, Swagger)
  Inv.Application/    # CQRS (commands/queries), validators, ports/abstractions
  Inv.Domain/         # Domain entities & rules (tech‑agnostic)
  Inv.Infrastructure/    # EF Core DbContext, configurations, migrations
tests/
  IntegrationTests/   # WebApplicationFactory‑based integration tests
  UnitTests/          # Unit tests (selected components)
```

---

## ⚠️ Notes & Limitations

- **Tests do not cover everything** (time‑boxed). Critical paths are prioritized.
- There are **ideas for improvement** commented inline where relevant.
- A portion of the structure was adapted from the author’s **personal project**.
- Some code—especially in tests—was **AI‑assisted** to present a complete working sample quickly.

---

## 🚀 Ideas for Improvement

- **Auth hardening:** JWT issuer/audience validation, rotating keys, richer roles/policies
- **Optimistic concurrency:** ETag/If‑Match on updates
- **Idempotency:** `Idempotency‑Key` support on POSTs
- **Outbox pattern** for reliable integration events
- **Observability:** OpenTelemetry tracing/metrics + structured logging
- **Contract tests:** Pact for API and outbound clients
- **Docker Compose:** API + SQL Server for one‑command local spin‑up
- **More test coverage:** negative paths, seeding journal, performance considerations
- **Secret management:** user‑secrets locally, vault in non‑dev

---

## 📣 Final Notes

This repo is intentionally **interview‑friendly**: it runs quickly on MS‑SQL, shows a pragmatic architecture, and includes enough tests and comments to discuss trade‑offs.  
If startup fails, first verify the **connection string** and **environment**; then try `dotnet ef database update` again.

Happy reviewing!
