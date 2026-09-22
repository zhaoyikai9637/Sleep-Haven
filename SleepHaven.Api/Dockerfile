FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source
COPY SleepHaven.Api/SleepHaven.Api.csproj SleepHaven.Api/
COPY SleepHaven.Core/SleepHaven.Core.csproj SleepHaven.Core/
RUN dotnet restore SleepHaven.Api/SleepHaven.Api.csproj
COPY . .
RUN dotnet publish SleepHaven.Api/SleepHaven.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app .
USER app
ENTRYPOINT ["dotnet", "SleepHaven.Api.dll"]
