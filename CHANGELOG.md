# Changelog

All notable changes to SleepHaven will be documented in this file. The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and the project intends to use [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Added an ASP.NET Core 10 backend with PostgreSQL 18, EF Core migrations, health checks, and catalogue seeding.
- Added real PostgreSQL integration tests covering the MAUI API client, migrations, health, product reads, favourite writes, and per-client isolation.
- Added Docker Compose development services and Huawei/Honor LAN connection instructions.
- Added a platform-neutral Core project and an xUnit test project covering catalogue integrity, API contracts, weather rules, search cancellation, and navigation guards.
- Adopted the MIT License, copyright 2026 Zhao Yikai.
- Added a product-image provenance statement and permission-record template.
- Open-source contribution, conduct, security, issue, and pull-request guidance.
- Windows and Android GitHub Actions build validation.
- Roadmap, demo checklist, release notes, repository-settings guidance, and application draft.

### Changed

- Replaced all on-device SQLite packages, attributes, migrations, and services with an HTTP product store backed by PostgreSQL.
- Moved favourites into a dedicated backend table keyed by installation client ID and product ID.
- Replaced display-string product prices and categories with structured price, currency, type, season, and material fields.
- Registered database, catalogue, weather, navigation, and page dependencies through MAUI dependency injection.
- Added 250 ms cancellable search debounce, catalogue caching, weather-result caching, request timeout/cancellation, and duplicate-navigation protection.
- Losslessly optimized PNG resources by 20.53% while preserving decoded pixels and dimensions.
- Clarified the repository's prototype status and supported use cases.
- Normalised the planned first public version to `1.0.0`.

## Planned first release: 1.0.0

The repository has not been tagged or released. `v1.0.0` is the proposed first public source release because the application already used display version `1.0`; the release must remain blocked until asset redistribution rights are documented.

[Unreleased]: https://github.com/zhaoyikai9637/Sleep-Haven/commits/main
