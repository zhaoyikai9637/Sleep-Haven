# SleepHaven

SleepHaven is a .NET MAUI product catalogue for bedding recommendations, search, and local favourites.

## Project layout

- `Data/` contains SQLite access and seed catalogue data.
- `Models/` contains persisted application models.
- `Pages/` contains MAUI pages and their code-behind files.
- `Services/` contains external integrations such as weather lookup.
- `Platforms/` and `Resources/` follow the standard .NET MAUI single-project layout.

## Build on Windows

Requirements:

- .NET 10 SDK
- `maui-windows` workload

```powershell
dotnet workload install maui-windows
dotnet restore SleepHaven.csproj -p:TargetFramework=net10.0-windows10.0.19041.0
dotnet build SleepHaven.csproj -f net10.0-windows10.0.19041.0 --no-restore
```

Weather recommendations use Open-Meteo and do not require an API key.
