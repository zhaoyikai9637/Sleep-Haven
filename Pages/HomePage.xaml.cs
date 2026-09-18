using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;

namespace SleepHaven;

public partial class HomePage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private readonly ProductCatalogService _catalogService;
    private readonly SeasonCatalogService _seasonCatalogService;
    private readonly WeatherService _weatherService;
    private readonly DebouncedSearchService<Product> _searchService;
    private readonly AsyncNavigationGuard _navigationGuard;
    private readonly MotionPreferences _motionPreferences;
    private readonly ILogger<HomePage> _logger;
    private CancellationTokenSource _pageLifetime = new();
    private CancellationTokenSource? _weatherRequest;
    private bool _isFirstLoad = true;
    private bool _hasAnimated;
    private int _heroIndex;
    private int _seasonIndex;
    private bool _hasThemeHandler;
    private bool? _isCompactSeasonLayout;
    private bool _hasManualSeasonSelection;
    private int _weatherRequestVersion;

    private static readonly SeasonPalette[] SeasonPalettes =
    [
        new("Spring", "#DCE9DF", "#213F34", "#456955", "#F1F6F0", "#213B32", "#1B3029", "#E5F1E8"),
        new("Summer", "#D8E9EC", "#1D3B49", "#416675", "#EFF6F7", "#1C3640", "#19313A", "#E0EFF1"),
        new("Autumn", "#E8DED4", "#48382F", "#705844", "#F5F0E9", "#3E312C", "#382C27", "#F0E5D8"),
        new("Winter", "#DDE3EB", "#293950", "#4E637D", "#F0F3F7", "#26364B", "#202D40", "#E5ECF3")
    ];

    private IReadOnlyList<CarouselItem> HeroItems { get; } =
    [
        new("p012", "restful_recovery_memory_pillow_bedroom.png", "01", "SUPPORT / RECOVERY", "Restful Recovery Memory Pillow", "Pressure-aware support for slower, deeper nights."),
        new("p007", "mulberrysilk_summerquilt_bedroom.png", "02", "LIGHT / BREATHABLE", "Mulberry Silk Summer Quilt", "A weightless layer selected for warm, humid air."),
        new("p003", "tencelcotton_abdual_use_bedroom.png", "03", "SOFT / ADAPTABLE", "Tencel Cotton Bedding Set", "Two-sided comfort with a cool, fluid hand feel.")
    ];

    public ObservableCollection<Product> SearchSuggestions { get; } = [];
    public ObservableCollection<SeasonSection> SeasonSections { get; } = [];
    public ObservableCollection<Product> ActiveSeasonProducts { get; } = [];

    public HomePage(
        DatabaseService databaseService,
        ProductCatalogService catalogService,
        SeasonCatalogService seasonCatalogService,
        WeatherService weatherService,
        DebouncedSearchService<Product> searchService,
        AsyncNavigationGuard navigationGuard,
        MotionPreferences motionPreferences,
        ILogger<HomePage> logger)
    {
        InitializeComponent();
        _databaseService = databaseService;
        _catalogService = catalogService;
        _seasonCatalogService = seasonCatalogService;
        _weatherService = weatherService;
        _searchService = searchService;
        _navigationGuard = navigationGuard;
        _motionPreferences = motionPreferences;
        _logger = logger;
        SuggestionsCollectionView.ItemsSource = SearchSuggestions;
        BindingContext = this;
        RenderHero();
        SeasonStage.IsVisible = false;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_pageLifetime.IsCancellationRequested)
        {
            _pageLifetime.Dispose();
            _pageLifetime = new CancellationTokenSource();
        }
        if (!_hasThemeHandler && Application.Current is { } app)
        {
            app.RequestedThemeChanged += OnRequestedThemeChanged;
            _hasThemeHandler = true;
        }

        if (SeasonSections.Count == 0)
        {
            await LoadSeasonSectionsAsync();
        }

        if (_isFirstLoad)
        {
            _isFirstLoad = false;
            await FetchWeatherAndRecommendAsync("Singapore");
        }

        await AnimateEntryAsync();
    }

    protected override void OnDisappearing()
    {
        if (_hasThemeHandler && Application.Current is { } app)
        {
            app.RequestedThemeChanged -= OnRequestedThemeChanged;
            _hasThemeHandler = false;
        }

        _searchService.Cancel();
        _weatherRequest?.Cancel();
        _pageLifetime.Cancel();

        base.OnDisappearing();
    }

    private void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e) => RenderSeason();

    private async Task LoadSeasonSectionsAsync()
    {
        foreach (var section in await _seasonCatalogService.GetSectionsAsync(_pageLifetime.Token))
            SeasonSections.Add(section);

        SeasonLoadingState.IsVisible = false;
        SeasonProductsLayout.IsVisible = true;
        RenderSeason();
    }

    private async Task AnimateEntryAsync()
    {
        if (_hasAnimated || !_motionPreferences.AreAnimationsEnabled)
        {
            return;
        }

        _hasAnimated = true;
        HomeContent.Opacity = 0;
        HomeContent.TranslationY = 10;
        await Task.WhenAll(HomeContent.FadeToAsync(1, 260, Easing.CubicOut), HomeContent.TranslateToAsync(0, 0, 260, Easing.CubicOut));
    }

    private void OnPreviousHeroClicked(object sender, EventArgs e)
    {
        MoveHeroBy(-1);
    }

    private void OnNextHeroClicked(object sender, EventArgs e)
    {
        MoveHeroBy(1);
    }

    private void OnHeroSwiped(object sender, SwipedEventArgs e)
    {
        if (e.Direction == SwipeDirection.Left)
        {
            MoveHeroBy(1);
        }
        else if (e.Direction == SwipeDirection.Right)
        {
            MoveHeroBy(-1);
        }
    }

    private void MoveHeroBy(int offset)
    {
        if (HeroItems.Count == 0)
        {
            return;
        }

        _heroIndex = (_heroIndex + offset + HeroItems.Count) % HeroItems.Count;
        RenderHero();
    }

    private void RenderHero()
    {
        if (HeroItems.Count == 0)
        {
            HeroStage.IsVisible = false;
            return;
        }

        HeroStage.IsVisible = true;
        var hero = HeroItems[_heroIndex];
        HeroImage.Source = hero.ImageUrl;
        SemanticProperties.SetDescription(HeroImage, hero.Title);
        HeroNumberLabel.Text = hero.Number;
        HeroEyebrowLabel.Text = hero.Eyebrow;
        HeroTitleLabel.Text = hero.Title;
        HeroSummaryLabel.Text = hero.Summary;
        HeroPositionLabel.Text = $"{_heroIndex + 1:00} / {HeroItems.Count:00}";
    }

    private async void OnCurrentHeroTapped(object sender, TappedEventArgs e)
    {
        if (HeroItems.Count == 0)
        {
            return;
        }

        var product = await _databaseService.GetProductByIdAsync(HeroItems[_heroIndex].Id);
        if (product is not null)
        {
            await _navigationGuard.TryRunAsync(() =>
                Navigation.PushAsync(new ProductDetailPage(product, _databaseService, _catalogService)));
        }
    }

    private async void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
    {
        var query = e.NewTextValue?.Trim();
        if (string.IsNullOrEmpty(query))
        {
            ShowMainContent();
            return;
        }

        try
        {
            var matches = await _searchService.SearchAsync(
                query,
                _catalogService.SearchAsync,
                _pageLifetime.Token);
            ReplaceItems(SearchSuggestions, matches);
            SearchSuggestionsOverlay.IsVisible = true;
            MainContentScrollView.IsVisible = false;
        }
        catch (OperationCanceledException)
        {
            // Expected when the user continues typing or leaves the page.
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Search suggestions could not be loaded.");
            ShowMainContent();
        }
    }

    private void ShowMainContent()
    {
        SearchSuggestions.Clear();
        SearchSuggestionsOverlay.IsVisible = false;
        MainContentScrollView.IsVisible = true;
    }

    private async void OnSuggestionSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Product product)
        {
            return;
        }

        SuggestionsCollectionView.SelectedItem = null;
        MainSearchBar.Text = string.Empty;
        await _navigationGuard.TryRunAsync(() =>
            Navigation.PushAsync(new ProductDetailPage(product, _databaseService, _catalogService)));
    }

    private async void OnSearchButtonPressed(object sender, EventArgs e)
    {
        var query = MainSearchBar.Text?.Trim();
        if (!string.IsNullOrEmpty(query))
        {
            MainSearchBar.Text = string.Empty;
            await _navigationGuard.TryRunAsync(() =>
                Navigation.PushAsync(new SearchPage(
                    query,
                    _databaseService,
                    _catalogService,
                    _navigationGuard,
                    _logger)));
        }
    }

    private void OnPreviousSeasonClicked(object sender, EventArgs e) => MoveSeasonBy(-1);
    private void OnNextSeasonClicked(object sender, EventArgs e) => MoveSeasonBy(1);
    private void OnPreviousSeasonTapped(object sender, TappedEventArgs e) => MoveSeasonBy(-1);
    private void OnNextSeasonTapped(object sender, TappedEventArgs e) => MoveSeasonBy(1);

    private void OnSeasonSwiped(object sender, SwipedEventArgs e)
    {
        if (e.Direction == SwipeDirection.Left) MoveSeasonBy(1);
        else if (e.Direction == SwipeDirection.Right) MoveSeasonBy(-1);
    }

    private void MoveSeasonBy(int offset)
    {
        if (SeasonSections.Count == 0) return;
        _hasManualSeasonSelection = true;
        _seasonIndex = (_seasonIndex + offset + SeasonSections.Count) % SeasonSections.Count;
        RenderSeason();
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        var compact = width < 760;
        if (_isCompactSeasonLayout == compact) return;

        _isCompactSeasonLayout = compact;
        PreviousSeasonPreview.IsVisible = !compact;
        NextSeasonPreview.IsVisible = !compact;
        SeasonPreviewGrid.ColumnDefinitions[0].Width = new GridLength(compact ? 0 : 18, GridUnitType.Star);
        SeasonPreviewGrid.ColumnDefinitions[1].Width = new GridLength(compact ? 1 : 64, GridUnitType.Star);
        SeasonPreviewGrid.ColumnDefinitions[2].Width = new GridLength(compact ? 0 : 18, GridUnitType.Star);
        SeasonStage.HeightRequest = compact ? 390 : 430;
    }

    private async void OnSeasonFeatureTapped(object sender, TappedEventArgs e)
    {
        if (SeasonSections.Count == 0 || SeasonSections[_seasonIndex].Products.FirstOrDefault() is not { } product) return;
        await _navigationGuard.TryRunAsync(() =>
            Navigation.PushAsync(new ProductDetailPage(product, _databaseService, _catalogService)));
    }

    private void RenderSeason()
    {
        if (SeasonSections.Count == 0) return;

        var current = SeasonSections[_seasonIndex];
        var previous = SeasonSections[(_seasonIndex - 1 + SeasonSections.Count) % SeasonSections.Count];
        var next = SeasonSections[(_seasonIndex + 1) % SeasonSections.Count];
        var palette = SeasonPalettes.First(p => p.Key == current.Key);
        var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;
        var background = Color.FromArgb(isDark ? palette.DarkBackground : palette.LightBackground);
        var foreground = Color.FromArgb(isDark ? "#F0F5F4" : palette.LightForeground);
        var muted = Color.FromArgb(isDark ? "#C5D2D2" : palette.LightAccent);
        var accent = Color.FromArgb(isDark ? palette.DarkAccent : palette.LightAccent);
        var productBackground = Color.FromArgb(isDark ? palette.DarkSurface : palette.LightSurface);

        SeasonStage.IsVisible = true;
        SeasonStage.BackgroundColor = background;
        SeasonProductArea.BackgroundColor = productBackground;
        SeasonSequenceLabel.Text = "THE SEASONAL EDIT";
        SeasonSequenceLabel.TextColor = accent;
        SeasonTitleLabel.Text = current.Key;
        SeasonTitleLabel.TextColor = foreground;
        SeasonSummaryLabel.Text = current.Summary;
        SeasonSummaryLabel.TextColor = muted;
        SeasonCountLabel.Text = $"{current.Products.Count} PIECES";
        SeasonCountLabel.TextColor = foreground;
        SeasonPositionLabel.Text = $"{_seasonIndex + 1} OF {SeasonSections.Count}";
        SeasonPositionLabel.TextColor = muted;
        SeasonProductsKicker.Text = $"{current.Key.ToUpperInvariant()} COLLECTION";
        SeasonProductsKicker.TextColor = accent;
        SeasonProductCountLabel.Text = $"{current.Products.Count} pieces";
        SeasonEmptyState.IsVisible = current.Products.Count == 0;
        foreach (var button in new[] { PreviousSeasonButton, NextSeasonButton })
        {
            button.BackgroundColor = accent;
            button.TextColor = isDark ? Color.FromArgb(palette.DarkBackground) : Colors.White;
        }

        PreviousSeasonLabel.Text = previous.Key.ToUpperInvariant();
        PreviousSeasonLabel.TextColor = foreground;
        PreviousSeasonLabel.BackgroundColor = background;
        PreviousSeasonTint.Color = background;
        PreviousSeasonImage.Source = previous.Products.FirstOrDefault()?.LandscapeUrl;
        NextSeasonLabel.Text = next.Key.ToUpperInvariant();
        NextSeasonLabel.TextColor = foreground;
        NextSeasonLabel.BackgroundColor = background;
        NextSeasonTint.Color = background;
        NextSeasonImage.Source = next.Products.FirstOrDefault()?.LandscapeUrl;

        var feature = current.Products.FirstOrDefault();
        CurrentSeasonImage.Source = feature?.LandscapeUrl;
        SeasonFeatureLabel.Text = feature?.Name ?? "New pieces coming soon";
        SemanticProperties.SetDescription(CurrentSeasonImage, feature?.Name ?? current.Title);
        ActiveSeasonProducts.Clear();
        foreach (var product in current.Products) ActiveSeasonProducts.Add(product);
    }

    private async void OnProductTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not string productId)
        {
            return;
        }

        var product = await _databaseService.GetProductByIdAsync(productId);
        if (product is not null)
        {
            await _navigationGuard.TryRunAsync(() =>
                Navigation.PushAsync(new ProductDetailPage(product, _databaseService, _catalogService)));
        }
    }

    private async void OnSingaporeClicked(object sender, EventArgs e) => await SelectCityAsync("Singapore");
    private async void OnQingdaoClicked(object sender, EventArgs e) => await SelectCityAsync("Qingdao");

    private async Task SelectCityAsync(string cityName)
    {
        _hasManualSeasonSelection = false;
        SetCityButtonState(BtnSingapore, cityName == "Singapore");
        SetCityButtonState(BtnQingdao, cityName == "Qingdao");
        await FetchWeatherAndRecommendAsync(cityName);
    }

    private static void SetCityButtonState(Button button, bool selected)
    {
        var resources = Application.Current?.Resources;
        var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;
        button.BackgroundColor = selected
            ? (Color?)resources?["Accent"] ?? Color.FromArgb("#0878F9")
            : (Color?)resources?[isDark ? "DarkSurfaceMuted" : "AccentMist"] ?? Color.FromArgb("#DDF2FF");
        button.TextColor = selected
            ? Colors.White
            : (Color?)resources?[isDark ? "AccentDark" : "AccentDeep"] ?? Color.FromArgb("#064A9B");
    }

    private async Task FetchWeatherAndRecommendAsync(string cityName)
    {
        var requestVersion = ++_weatherRequestVersion;
        _weatherRequest?.Cancel();
        _weatherRequest?.Dispose();
        _weatherRequest = CancellationTokenSource.CreateLinkedTokenSource(_pageLifetime.Token);
        WeatherRecommendationCard.IsVisible = true;
        WeatherTitleLabel.Text = $"Reading {cityName}'s night";
        WeatherBodyLabel.Text = "Selecting a comfortable material profile.";

        try
        {
            var weather = await _weatherService.GetComfortForecastAsync(cityName, _weatherRequest.Token);
            if (requestVersion == _weatherRequestVersion) ApplyWeatherRecommendation(weather);
        }
        catch (OperationCanceledException)
        {
            // Expected when a newer city request replaces this one or the page closes.
        }
        catch (Exception exception)
        {
            if (requestVersion != _weatherRequestVersion) return;
            _logger.LogWarning(exception, "The weather recommendation could not be loaded for {CityName}.", cityName);
            WeatherTitleLabel.Text = "Weather signal unavailable";
            WeatherBodyLabel.Text = "Every seasonal edit remains available below.";
        }
    }

    private void ApplyWeatherRecommendation(WeatherSnapshot weather)
    {
        var targetSeason = WeatherService.GetRecommendedSeason(weather);
        var recommendedIndex = SeasonSections.ToList().FindIndex(season => season.Key == targetSeason);
        if (recommendedIndex >= 0 && !_hasManualSeasonSelection)
        {
            _seasonIndex = recommendedIndex;
            RenderSeason();
        }

        WeatherTitleLabel.Text = $"{weather.CityName} / Night low {weather.NightLowTemperature:F1}°C";
        var advice = targetSeason switch
        {
            "Summer" => "Choose light, breathable layers.",
            "Autumn" => "Choose balanced layers for cooler nights.",
            "Winter" => "Choose insulating quilts and warmer bedding.",
            _ => "Choose breathable layers with gentle warmth."
        };
        WeatherBodyLabel.Text = $"Now {weather.Temperature:F1}°C. {targetSeason} edit: {advice}";
    }

    private static void ReplaceItems<T>(ObservableCollection<T> target, IEnumerable<T> items)
    {
        target.Clear();
        foreach (var item in items)
        {
            target.Add(item);
        }
    }

    private sealed record SeasonPalette(
        string Key,
        string LightBackground,
        string LightForeground,
        string LightAccent,
        string LightSurface,
        string DarkBackground,
        string DarkSurface,
        string DarkAccent);
}
