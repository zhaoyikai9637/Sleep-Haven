# SleepHaven

[![CI](https://github.com/zhaoyikai9637/Sleep-Haven/actions/workflows/ci.yml/badge.svg)](https://github.com/zhaoyikai9637/Sleep-Haven/actions/workflows/ci.yml)

SleepHaven is an actively maintained .NET MAUI prototype for browsing and presenting a small bedding catalogue. Its mobile/desktop client now connects to an ASP.NET Core API backed by PostgreSQL.

The repository is also intended as a compact reference for cross-platform .NET MAUI UI, a real HTTP backend, PostgreSQL persistence, and a small weather-backed recommendation flow.

> **Project status:** active prototype. Search, browsing, product details, API-backed favourites, database migrations, and the seeded catalogue are implemented. Checkout, payments, live inventory, orders, accounts, analytics, and merchant administration are not implemented.

## Why it exists

Small physical retailers often have useful product knowledge but no lightweight way to turn it into a coherent digital catalogue. SleepHaven demonstrates a focused product-discovery experience that can be shown in-store or used as a starting point for a retailer-specific app.

Current use cases:

- browse a seeded bedding catalogue by category and season;
- search product names, materials, and seasonal categories;
- save favourites in PostgreSQL under a per-installation client identifier;
- use Singapore or Qingdao weather data to select an initial seasonal edit;
- study a single-project .NET MAUI app targeting Android, iOS, Mac Catalyst, and Windows.

## Features

- Layered seasonal browsing with explicit previous/next and swipe controls.
- Weather-informed recommendations based on the upcoming local night temperature.
- Search suggestions, category browsing, and detailed product pages.
- ASP.NET Core API with PostgreSQL catalogue and per-client favourites.
- Light and dark themes, reduced-motion handling, and responsive layouts.
- No customer account or weather API key is required. Weather data comes from [Open-Meteo](https://open-meteo.com/).

## Demo and screenshots

The reproducible walkthrough and screenshot checklist live in [`docs/DEMO.md`](docs/DEMO.md). Verified application screenshots should be stored in [`docs/screenshots/`](docs/screenshots/) and must come from a real build. Reference artwork and design-research images are not accepted as product screenshots.

No current application screenshot is checked in yet because this maintenance pass could not access a supported native capture surface. That gap is tracked in the roadmap rather than filled with a mock image.

## Technology

| Area | Implementation |
| --- | --- |
| UI | .NET 10, .NET MAUI, XAML |
| Backend | ASP.NET Core 10 minimal API |
| Database | PostgreSQL 18 / EF Core migrations / Npgsql |
| Weather | Open-Meteo forecast API, no API key |
| Targets | Android 7.0+, iOS 15+, Mac Catalyst 15+, Windows 10 1809+ |
| Repository | GitHub Actions CI, Dependabot, issue and pull-request templates |

## Repository layout

- `SleepHaven.Core/` builds the platform-neutral API client, catalogue, search, and weather logic used by the app and tests.
- `SleepHaven.Api/` contains the HTTP API, EF Core mappings, migrations, and catalogue seeding.
- `SleepHaven.Api.Tests/` exercises the real client/API/database path against PostgreSQL.
- `SleepHaven.Tests/` contains fast xUnit validation and client-contract tests.
- `Data/` contains the seeded product catalogue.
- `Models/` contains structured product models; display strings are derived rather than stored.
- `Pages/` contains MAUI pages and their code-behind files.
- `Services/` contains motion preferences and weather integration.
- `Platforms/` and `Resources/` follow the .NET MAUI single-project layout.
- `docs/` contains design decisions, release preparation, roadmap, and application material.

## Build on Windows

Requirements:

- .NET 10 SDK (the repository pins the feature band in `global.json`)
- Visual Studio 2022 with MAUI support, or the required CLI workloads
- a running SleepHaven API; see [`docs/BACKEND_DATABASE.md`](docs/BACKEND_DATABASE.md)

```powershell
dotnet workload install maui-windows
dotnet restore SleepHaven.Core/SleepHaven.Core.csproj
dotnet restore SleepHaven.csproj --no-dependencies `
  -p:TargetFrameworks=net10.0-windows10.0.19041.0 `
  -r win-x64
dotnet build SleepHaven.csproj `
  -f net10.0-windows10.0.19041.0 `
  -p:TargetFrameworks=net10.0-windows10.0.19041.0 `
  -p:RuntimeIdentifier=win-x64 `
  --no-restore
```

Run the unpackaged Windows build from:

```text
bin/Debug/net10.0-windows10.0.19041.0/win-x64/SleepHaven.exe
```

## Build for Android

Requirements:

- .NET 10 SDK and Android workload
- JDK 21
- Android SDK platform and build tools for API 36
- a backend URL reachable from the emulator or phone

```powershell
dotnet workload install android
dotnet restore SleepHaven.Core/SleepHaven.Core.csproj
dotnet restore SleepHaven.csproj --no-dependencies `
  -p:TargetFrameworks=net10.0-android `
  -r android-arm64
dotnet build SleepHaven.csproj `
  -f net10.0-android `
  -p:TargetFrameworks=net10.0-android `
  -p:RuntimeIdentifier=android-arm64 `
  -p:SleepHavenBackendUrl=http://192.168.1.20:5080 `
  --no-restore
```

Release APKs must be signed with a maintainer-controlled keystore. Never commit keystores or passwords. See [`docs/ANDROID_INSTALL_ZH.txt`](docs/ANDROID_INSTALL_ZH.txt) for the current Chinese installation guide.

## Verification

Before opening a pull request, run the repository checks described in [`CONTRIBUTING.md`](CONTRIBUTING.md). CI builds Windows and Android targets independently so that unavailable Apple workloads do not block contributors on Windows runners.

```powershell
dotnet restore SleepHaven.Tests/SleepHaven.Tests.csproj
dotnet test SleepHaven.Tests/SleepHaven.Tests.csproj --no-restore
```

The fast suite validates seed IDs and image references, structured commerce fields, API-client contracts, weather recommendation boundaries and cancellation, cached search behaviour, and duplicate-navigation protection. The integration suite starts the real API against PostgreSQL and verifies migrations, health, seeding, favourite round-trips, and client isolation. Backend setup and migration policy are documented in [`docs/BACKEND_DATABASE.md`](docs/BACKEND_DATABASE.md).

Product PNGs are losslessly optimized. Their decoded pixels and dimensions were verified unchanged; the tracked image payload was reduced from 28,340,171 to 22,521,049 bytes.

## Roadmap

The evidence-based roadmap and issue-ready backlog are in [`docs/ROADMAP.md`](docs/ROADMAP.md). Immediate priorities are automated tests, verified screenshots, asset provenance, packaging documentation, and accessibility validation. Commerce features will only be advertised after the corresponding models and integrations exist.

## Contributing and security

- Read [`CONTRIBUTING.md`](CONTRIBUTING.md) before proposing changes.
- Follow [`CODE_OF_CONDUCT.md`](CODE_OF_CONDUCT.md) in project spaces.
- Report vulnerabilities according to [`SECURITY.md`](SECURITY.md); do not post sensitive exploit details in a public issue.

## Maintenance and evidence

The repository owner is currently the primary maintainer. Maintenance activity is visible in the public commit history. Adoption claims, download counts, merchant deployments, and testimonials are intentionally not stated without evidence. See [`docs/CODEX_OPEN_SOURCE_APPLICATION.md`](docs/CODEX_OPEN_SOURCE_APPLICATION.md) for a fact-checked application draft and its remaining evidence gaps.

## License

SleepHaven source code is available under the [MIT License](LICENSE), copyright 2026 Zhao Yikai.

Bundled photographs, icons, fonts, and other non-code assets may have separate ownership or licence terms and are not automatically relicensed under MIT. See [`docs/ASSET_PROVENANCE.md`](docs/ASSET_PROVENANCE.md) before redistributing application assets.
