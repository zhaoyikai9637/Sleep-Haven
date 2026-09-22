# Backend and database

SleepHaven uses an ASP.NET Core 10 API and PostgreSQL 18. The MAUI app never connects directly to the database. It sends HTTP requests to the API, and the API owns schema migrations, catalogue seeding, and favourite persistence.

## Start the local stack

1. Copy `.env.example` to `.env` and replace the placeholder with a long local password.
2. Run `docker compose up --build` from the repository root.
3. Open `http://127.0.0.1:5080/health/database`. A successful response reports `Healthy` and `PostgreSQL`.

The database volume is named `sleephaven_postgres`. Rebuilding the API container does not erase it. Do not commit `.env`, production connection strings, database exports, or credentials.

To run the API without containers, provide the connection string through the environment and start the project:

```powershell
$env:ConnectionStrings__Catalog = 'Host=127.0.0.1;Port=5432;Database=sleephaven;Username=sleephaven;Password=your-local-password'
$env:ASPNETCORE_URLS = 'http://0.0.0.0:5080'
dotnet run --project SleepHaven.Api/SleepHaven.Api.csproj
```

## Phone and emulator addresses

- Windows client on the same PC: `http://127.0.0.1:5080`
- Android emulator: `http://10.0.2.2:5080`
- Huawei/Honor physical phone: the PC's private LAN address, for example `http://192.168.1.20:5080`

For a physical phone, keep the PC and phone on the same trusted Wi-Fi network, allow TCP port 5080 only on the private Windows network, and build with:

```powershell
dotnet build SleepHaven.csproj `
  -f net10.0-android `
  -p:TargetFrameworks=net10.0-android `
  -p:RuntimeIdentifier=android-arm64 `
  -p:SleepHavenBackendUrl=http://192.168.1.20:5080
```

Local HTTP is enabled in the Android manifest solely for LAN development. A deployed API must use HTTPS; before a public release, point `SleepHavenBackendUrl` to that HTTPS origin and disable cleartext traffic.

## Data ownership

`products` stores replaceable catalogue data. `favorites` stores a composite key of installation client ID and product ID, so one phone cannot change another phone's collection accidentally. The client ID is a random identifier generated on first launch; it is not an account, authentication token, or personal profile.

There is intentionally no automatic import from the former on-device database. Existing local favourites cannot be attributed safely to a backend installation identity and must be selected again after this architecture change.

## Migration policy

- EF Core migrations in `SleepHaven.Api/Migrations/` are the only schema-change mechanism.
- The API applies pending migrations before accepting requests.
- Product IDs are stable keys; editing seed text must not change an existing ID.
- Product seeding updates catalogue fields without deleting per-client favourites.
- Every schema change requires a forward migration and a PostgreSQL integration test.
- Production databases require automated backups and a tested restore procedure before serving real customer data.

## Integration verification

Set `SLEEPHAVEN_TEST_POSTGRES` to a disposable PostgreSQL database and run:

```powershell
$env:SLEEPHAVEN_TEST_POSTGRES = 'Host=127.0.0.1;Port=5432;Database=sleephaven_test;Username=postgres;Password=your-test-password;Pooling=false'
dotnet test SleepHaven.Api.Tests/SleepHaven.Api.Tests.csproj
```

The test database is migrated and seeded automatically. Never point this variable at a production database.
