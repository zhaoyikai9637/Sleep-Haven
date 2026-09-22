# Contributing to SleepHaven

Thank you for considering a contribution. SleepHaven is currently a small, maintainer-led prototype, so focused changes with clear verification are easier to review than broad rewrites.

## Before you start

1. Search existing issues and pull requests before opening a duplicate.
2. Open an issue for a large feature, data-model change, new external service, or visual redesign.
3. Do not add product claims, adoption metrics, testimonials, or asset-licensing statements without a public source or maintainer-provided evidence.
4. Never commit API keys, signing files, passwords, personal data, build output, or locally generated databases.

## Development setup

Windows and Android are the currently verified maintainer build targets. Follow the commands in [`README.md`](README.md). Apple targets require the corresponding Apple hardware and toolchain and are not covered by the current CI workflow.

Create a branch from `main`:

```powershell
git switch main
git pull --ff-only
git switch -c feature/short-description
```

## Project conventions

- Keep nullable reference types enabled and avoid suppressing warnings without an explanation.
- Prefer small, cohesive XAML/code-behind changes that preserve keyboard, touch, theme, and reduced-motion behaviour.
- Keep persistence changes backward-compatible or document the migration path.
- Treat network responses as unavailable or malformed by default; the catalogue must remain usable when weather lookup fails.
- Reuse design tokens in `Resources/Styles/` before adding one-off colours or typography.
- Add or update documentation when behaviour, setup, or supported platforms change.

## Verification

Run the checks relevant to your change. A documentation-only change still needs link, diff, and repository checks; application changes need fresh builds.

```powershell
git diff --check
dotnet restore SleepHaven.Tests/SleepHaven.Tests.csproj
dotnet test SleepHaven.Tests/SleepHaven.Tests.csproj --no-restore
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

Changes to the API, database model, migrations, or product-store contract must also run the PostgreSQL integration suite described in [`docs/BACKEND_DATABASE.md`](docs/BACKEND_DATABASE.md). Use a disposable test database and never point tests at production.

For Android changes, also build `net10.0-android` for `android-arm64` as documented in the README. If you cannot run a required target, state that clearly in the pull request.

## Pull requests

- Explain the user-facing problem and the chosen scope.
- Link the related issue when one exists.
- Include before/after screenshots for UI changes, captured from a real build.
- List exact verification commands and results.
- Call out migrations, permissions, new dependencies, and unresolved risks.
- Keep generated binaries and signing material out of the pull request.

By participating, you agree to follow the [`CODE_OF_CONDUCT.md`](CODE_OF_CONDUCT.md). Security reports follow [`SECURITY.md`](SECURITY.md), not the public issue tracker.
