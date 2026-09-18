# Changelog

All notable changes to SleepHaven will be documented in this file. The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and the project intends to use [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Added a platform-neutral Core project and an xUnit test project covering catalogue integrity, weather rules, search cancellation, navigation guards, persistence, and legacy migration.
- Added an explicit SQLite schema-version migration policy and stable database filename.
- Adopted the MIT License, copyright 2026 Zhao Yikai.
- Added a product-image provenance statement and permission-record template.
- Open-source contribution, conduct, security, issue, and pull-request guidance.
- Windows and Android GitHub Actions build validation.
- Roadmap, demo checklist, release notes, repository-settings guidance, and application draft.

### Changed

- Replaced display-string product prices and categories with structured price, currency, type, season, and material fields.
- Registered database, catalogue, weather, navigation, and page dependencies through MAUI dependency injection.
- Added 250 ms cancellable search debounce, catalogue caching, weather-result caching, request timeout/cancellation, and duplicate-navigation protection.
- Losslessly optimized PNG resources by 20.53% while preserving decoded pixels and dimensions.
- Clarified the repository's prototype status and supported use cases.
- Normalised the planned first public version to `1.0.0`.

## Planned first release: 1.0.0

The repository has not been tagged or released. `v1.0.0` is the proposed first public source release because the application already used display version `1.0`; the release must remain blocked until asset redistribution rights are documented.

[Unreleased]: https://github.com/zhaoyikai9637/Sleep-Haven/commits/main
