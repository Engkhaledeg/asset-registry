# Asset Registry

An internal register of IT equipment: who owns which laptop, where it sits, and what state it's in.
The domain was picked because it needs a real relationship (assets belong to locations), a lifecycle
(in stock → deployed → under repair → retired), and a list worth filtering — enough surface to show
layering without inflating the test.

## Stack

| Layer | Choice |
| --- | --- |
| Backend | .NET 8 (LTS), ASP.NET Core Web API |
| Data | SQL Server via EF Core 8, code-first migrations |
| Frontend | Angular 18 (standalone components), TypeScript, SCSS |
| Identity | Microsoft Entra ID, OpenID Connect / OAuth 2.0 |
| Tests | xUnit |

## Projects

```
src/
  AssetRegistry.Domain          entities, enums, invariants — no framework references
  AssetRegistry.Application     services, DTOs, repository interfaces
  AssetRegistry.Infrastructure  EF Core context, configurations, repositories
  AssetRegistry.Api             controllers, auth, error handling, composition root
  ClientApp                     Angular SPA
tests/
  AssetRegistry.UnitTests       domain rules and service behaviour
```

Dependencies point inwards: `Api → Infrastructure → Application → Domain`. The Application layer
declares the interfaces it needs (`IAssetRepository`, `IUnitOfWork`, `IClock`) and Infrastructure
implements them, so services can be tested without a database — see `AssetServiceTests`.

## Running it

**Prerequisites:** .NET 8 SDK, Node 18+, a SQL Server instance (LocalDB or Docker).

### 1. Database

Set the connection string in `src/AssetRegistry.Api/appsettings.json`, then create the initial
migration and let the app apply it:

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate \
  --project src/AssetRegistry.Infrastructure \
  --startup-project src/AssetRegistry.Api
```

The API runs `DatabaseInitialiser` on startup, which migrates the database and seeds four locations.

### 2. Entra ID app registrations

Two registrations in the Azure portal:

- **API** — expose a scope named `access_as_user`; note the client ID and tenant ID.
- **SPA** — platform "Single-page application", redirect URI `http://localhost:4200`; grant it
  delegated permission to the API scope above.

Fill in `appsettings.json` (`AzureAd` section) and `src/ClientApp/src/environments/environment.ts`.

### 3. Run

```bash
dotnet run --project src/AssetRegistry.Api     # https://localhost:7043, Swagger at /swagger
cd src/ClientApp && npm install && npm start   # http://localhost:4200
dotnet test                                    # unit tests
```

## Decisions worth flagging

**Entra ID rather than ADFS.** The brief recommends ADFS, which needs a Windows Server domain to
stand up. Entra ID is its managed successor and speaks the same OIDC protocol, so the integration
code — `AddMicrosoftIdentityWebApi` on the API, MSAL redirect flow plus a bearer-token interceptor
in Angular — is what you would write against either. Swapping the authority and audience in config
is the whole difference.

**Entities protect their own invariants.** `Asset` has a private parameterless constructor for EF
and no public setters. Dates, prices and lengths are validated in the constructor, and a retired
asset refuses further changes. That rule lives in the entity rather than in the service, so it holds
no matter which caller reaches it.

**Asset tags are immutable.** They map to a physical sticker on the hardware, so the tag is set at
creation and the edit form disables it. `UpdateAssetRequest` has no tag field at all rather than
accepting one and ignoring it.

**No AutoMapper, no MediatR, no FluentValidation.** The brief asks for libraries only where they
earn their place. Mapping is six lines of extension method, request validation is data annotations
the framework already honours, and one service class per aggregate is easier to follow than a
handler per operation at this size.

**Errors become ProblemDetails in one place.** `ExceptionHandlingMiddleware` translates
`NotFoundException`, `ConflictException` and `DomainValidationException` into 404/409/400 so that
controllers contain no try/catch and no status-code plumbing.

**Query composition is separated from the repository.** `AssetQueryExtensions` holds filtering,
sorting and paging as composable `IQueryable` steps, which keeps `AssetRepository` readable and
makes a new filter a one-line addition.

**The clock is injected.** `IClock` exists so "purchase date cannot be in the future" is testable
without waiting for tomorrow.

## What I would add next

- Integration tests over the API using `WebApplicationFactory` and Testcontainers for SQL Server.
- Optimistic concurrency (`rowversion`) on `Asset`, surfaced as a 409 when two editors collide.
- Role-based authorisation — currently any authenticated user in the tenant can write.
- Structured logging and a health endpoint before anything goes near production.
