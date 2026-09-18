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

        builder.Services.AddSingleton(serviceProvider =>
        {
            var appData = FileSystem.AppDataDirectory;
            return new DatabaseService(
                Path.Combine(appData, "SleepHaven.db3"),
                Path.Combine(appData, "SleepHaven_v8.db3"),
                serviceProvider.GetRequiredService<ILogger<DatabaseService>>());
        });
        builder.Services.AddSingleton<IProductStore>(serviceProvider =>
            serviceProvider.GetRequiredService<DatabaseService>());
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
