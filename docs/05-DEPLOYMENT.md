# Setup, configuration and deployment

## Requirements

.NET SDK 10, Node.js 20+, PostgreSQL 14+ (tested on 16; production runs 18).

## Configuration

All settings can be given as environment variables (`Section__Key`). **No secret is stored in the repository.**

| Setting | Required | Meaning |
|---|---|---|
| `ConnectionStrings__Default` | yes | `Host=…;Port=5432;Database=HemCompetition;Username=…;Password=…;Maximum Pool Size=200` |
| `Jwt__Key` | yes | Random secret, at least 32 bytes. Generate: `openssl rand -base64 48` |
| `Bootstrap__AdminUsername`, `Bootstrap__AdminPassword` | first start only | Creates the first Super Admin when `admin_users` is empty; a password change is forced at first sign-in. |
| `Storage__Root` | no | Folder for evidence and question images (default `App_Data/storage`). Must be outside `wwwroot`, writable, and backed up. |
| `ReverseProxy__Enabled`, `ReverseProxy__KnownProxies__0` | behind IIS/nginx | Trusts `X-Forwarded-For/Proto` from the listed proxy addresses. |
| `Jwt__AccessTokenMinutes`, `Jwt__StudentRefreshHours`, `Jwt__AdminRefreshHours` | no | Token lifetimes (15 min / 8 h / 12 h). |
| `SessionSettings__HeartbeatTimeoutSeconds` | no | Student session timeout without heartbeat (150). |
| `Bootstrap__SeedWordBank` | no | Load the starter word list into an empty word bank (true). |

Everything else (lockout, consent version, camera/fullscreen, evidence size and retention, appeals)
is changed at runtime under **Admin → Settings**.

> The old `appsettings.json` contained the JWT key, the SQL Server `sa` password and the PostgreSQL
> password, and the three admin accounts shared a password that is in the git history.
> **Rotate the PostgreSQL password and use a new JWT key.** Existing admin accounts are forced to
> choose a new password at their next sign-in.

## Development

```bash
createdb hemcompetition_dev                     # or use pgAdmin
cd Backend/HemCompetition.API && dotnet run     # http://localhost:5098, applies migrations and seeds
cd Frontend && npm install && npm run dev       # http://localhost:3000, proxies /api to the API
```

`appsettings.Development.json` holds development-only values (local database, dev signing key,
admin `admin` / `ChangeMe-Dev-2026`).

## Tests

```bash
cd Backend && dotnet test HemCompetition.Tests
```

Integration tests create and drop their own databases on the server given by `TEST_PG`
(default `Host=localhost;Username=postgres;Password=postgres`). Frontend: `npm run lint`, `npm run build`.

## Upgrading the existing production database

1. **Back up first** (see below) and verify the backup restores.
2. Stop the old application.
3. Deploy the new build with `ConnectionStrings__Default` pointing at the existing database.
4. Start it. The log shows `Existing tables detected; recorded …Baseline as already applied`
   and then applies `PlatformSchema`. Existing students, sessions and competitions are untouched.
5. Sign in as an existing admin, choose a new password.
6. Import content (`docs/04-MSSQL-EXPORT.md`), configure and publish quizzes, check
   **Competitions**, **Violation rules** and **Settings**.
7. Remove the SQL Server connection string and firewall rule.

To review the SQL before running it: `dotnet ef migrations script Baseline PlatformSchema -o upgrade.sql`.

## Production build

```bash
cd Frontend && npm ci && npm run build          # writes into Backend/HemCompetition.API/wwwroot
cd ../Backend/HemCompetition.API
dotnet publish -c Release -o ../../publish
```

Run `publish/HemCompetition.API.dll` (`dotnet HemCompetition.API.dll`) as a service or under IIS
(ASP.NET Core Hosting Bundle), with `ASPNETCORE_ENVIRONMENT=Production` and the settings above.

* Serve over **HTTPS only**. Browsers allow camera access only on HTTPS (or localhost).
* Run **one instance**. Rate limits, the settings cache and API diagnostics are in-process; running
  several instances needs shared storage for evidence and a distributed limiter.
* Allow uploads of at least 20 MB at the proxy (evidence clips).
* Keep the server clock synchronised (timers and deadlines are server-side).

## Backup and restore

```bash
pg_dump -h HOST -U USER -d HemCompetition -Fc -f hemcompetition_$(date +%F).dump
pg_restore -h HOST -U USER -d HemCompetition_restore --no-owner hemcompetition_2026-01-01.dump
```

Back up the `Storage__Root` folder together with the database: the database holds evidence
metadata, the folder holds the files. Take a dump before every deployment that adds a migration.

## Before a live competition

* Run a rehearsal with real devices: camera permission, fullscreen, and the face detector on the
  phones and laptops students will actually use, in their lighting.
* Decide the violation thresholds (**Violation rules**) and the consent wording with whoever is
  responsible for student data; parents' consent may be required for minors in your jurisdiction.
* Load-test at the expected concurrency; raise `Maximum Pool Size` and the rate limits in
  `Program.cs` if a whole school shares one public address.
