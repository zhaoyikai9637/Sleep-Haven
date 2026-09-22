using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SleepHaven;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var clientId = Preferences.Default.Get("SleepHaven.ClientId", string.Empty);
        if (string.IsNullOrWhiteSpace(clientId))
        {
            clientId = Guid.NewGuid().ToString("N");
            Preferences.Default.Set("SleepHaven.ClientId", clientId);
        }

        var backendUrl = typeof(MauiProgram).Assembly
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .Single(attribute => attribute.Key == "SleepHavenBackendUrl")
            .Value!;
        builder.Services.AddSingleton(new BackendConnectionOptions(backendUrl, clientId));
        builder.Services.AddHttpClient<IProductStore, BackendProductStore>(client =>
            client.Timeout = TimeSpan.FromSeconds(12));
        builder.Services.AddSingleton<ProductCatalogService>();
        builder.Services.AddSingleton<SeasonCatalogService>();
        builder.Services.AddSingleton(new DebouncedSearchService<Product>(TimeSpan.FromMilliseconds(250)));
        builder.Services.AddSingleton<AsyncNavigationGuard>();
        builder.Services.AddSingleton<MotionPreferences>();
        builder.Services.AddHttpClient<WeatherService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(8);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("SleepHaven/1.0");
        });

        builder.Services.AddSingleton<HomePage>();
        builder.Services.AddSingleton<CategoryPage>();
        builder.Services.AddSingleton<CollectionPage>();
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<LoadingPage>();

        return builder.Build();
    }
}
