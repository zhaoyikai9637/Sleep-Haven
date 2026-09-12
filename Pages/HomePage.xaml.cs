using System.Collections.ObjectModel;

namespace SleepHaven;

public partial class HomePage : ContentPage
{
    private readonly DatabaseService _databaseService = new();
    private readonly WeatherService _weatherService = new(new HttpClient());
    private IDispatcherTimer? _carouselTimer;
    private bool _isFirstLoad = true;
    private bool _hasAnimated;

    public ObservableCollection<CarouselItem> CarouselItems { get; } =
    [
        new("p012", "restful_recovery_memory_pillow_bedroom.png", "Restful Recovery Memory Pillow"),
        new("p007", "mulberrysilk_summerquilt_bedroom.png", "Mulberry Silk Summer Quilt"),
        new("p003", "tencelcotton_abdual_use_bedroom.png", "Tencel Cotton Bedding Set")
    ];

    public ObservableCollection<Product> SearchSuggestions { get; } = [];
    public ObservableCollection<SeasonSection> SeasonSections { get; } = [];

    public HomePage()
    {
        InitializeComponent();
        SuggestionsCollectionView.ItemsSource = SearchSuggestions;
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (SeasonSections.Count == 0)
        {
            await LoadSeasonSectionsAsync();
        }

        if (_isFirstLoad)
        {
            _isFirstLoad = false;
            await FetchWeatherAndRecommendAsync("Singapore");
        }

        StartCarousel();
        await AnimateEntryAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _carouselTimer?.Stop();
    }

    private async Task LoadSeasonSectionsAsync()
    {
        var products = await _databaseService.GetAllProductsAsync();
        var definitions = new[]
        {
            (Key: "Spring", Title: "Spring layers", Summary: "Breathable cotton and soft structure"),
            (Key: "Summer", Title: "Summer lightness", Summary: "Silk and cooling natural fibres"),
            (Key: "Autumn", Title: "Autumn balance", Summary: "Comfort for cooler, drier nights"),
            (Key: "Winter", Title: "Winter warmth", Summary: "Insulating loft without excess weight")
        };

        foreach (var definition in definitions)
        {
            var matches = products.Where(product =>
                product.Category.Contains(definition.Key, StringComparison.OrdinalIgnoreCase));
            SeasonSections.Add(new SeasonSection(
                definition.Key,
                definition.Title,
                definition.Summary,
                matches,
                definition.Key == "Spring"));
        }

        SeasonLoadingState.IsVisible = false;
        SeasonSectionsLayout.IsVisible = true;
    }

    private async Task AnimateEntryAsync()
    {
        if (_hasAnimated || !MotionPreferences.AreAnimationsEnabled)
        {
            return;
        }

        _hasAnimated = true;
        HomeContent.Opacity = 0;
        HomeContent.TranslationY = 12;
        await Task.WhenAll(
            HomeContent.FadeToAsync(1, 280, Easing.CubicOut),
            HomeContent.TranslateToAsync(0, 0, 280, Easing.CubicOut));
    }

    private void StartCarousel()
    {
        if (!MotionPreferences.AreAnimationsEnabled)
        {
            return;
        }

        _carouselTimer ??= CreateCarouselTimer();
        if (!_carouselTimer.IsRunning)
        {
            _carouselTimer.Start();
        }
    }

    private IDispatcherTimer CreateCarouselTimer()
    {
        var timer = Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromSeconds(4.5);
        timer.Tick += (_, _) =>
        {
            if (CarouselItems.Count == 0)
            {
                return;
            }

            var nextIndex = (BannerCarousel.Position + 1) % CarouselItems.Count;
            BannerCarousel.ScrollTo(nextIndex, animate: true);
        };
        return timer;
    }

    private async void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
    {
        var query = e.NewTextValue?.Trim();
        if (string.IsNullOrEmpty(query))
        {
            ShowMainContent();
            return;
        }

        var products = await _databaseService.GetAllProductsAsync();
        ReplaceItems(SearchSuggestions, products.Where(product =>
            product.Name.Trim(' ', '"', '\'').StartsWith(query, StringComparison.OrdinalIgnoreCase)));

        SearchSuggestionsOverlay.IsVisible = true;
        MainContentScrollView.IsVisible = false;
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
        await Navigation.PushAsync(new ProductDetailPage(product));
    }

    private async void OnSearchButtonPressed(object sender, EventArgs e)
    {
        var query = MainSearchBar.Text?.Trim();
        if (!string.IsNullOrEmpty(query))
        {
            MainSearchBar.Text = string.Empty;
            await Navigation.PushAsync(new SearchPage(query));
        }
    }

    private void OnSeasonTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is SeasonSection season)
        {
            season.IsExpanded = !season.IsExpanded;
        }
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
            await Navigation.PushAsync(new ProductDetailPage(product));
        }
    }

    private async void OnSingaporeClicked(object sender, EventArgs e) => await SelectCityAsync("Singapore");
    private async void OnQingdaoClicked(object sender, EventArgs e) => await SelectCityAsync("Qingdao");

    private async Task SelectCityAsync(string cityName)
    {
        SetCityButtonState(BtnSingapore, cityName == "Singapore");
        SetCityButtonState(BtnQingdao, cityName == "Qingdao");
        await FetchWeatherAndRecommendAsync(cityName);
    }

    private static void SetCityButtonState(Button button, bool selected)
    {
        var resources = Application.Current?.Resources;
        var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;
        button.BackgroundColor = selected
            ? (Color?)resources?["Accent"] ?? Color.FromArgb("#2F6B5F")
            : (Color?)resources?[isDark ? "DarkSurfaceMuted" : "LightSurfaceMuted"] ?? Color.FromArgb("#E6ECEA");
        button.TextColor = selected
            ? (Color?)resources?["OnAccent"] ?? Color.FromArgb("#F7FAF9")
            : (Color?)resources?[isDark ? "DarkTextSecondary" : "LightTextSecondary"] ?? Color.FromArgb("#586762");
    }

    private async Task FetchWeatherAndRecommendAsync(string cityName)
    {
        WeatherRecommendationCard.IsVisible = true;
        WeatherTitleLabel.Text = $"Checking {cityName} weather";
        WeatherBodyLabel.Text = "Selecting a comfortable seasonal match.";

        try
        {
            var weather = await _weatherService.GetCurrentAsync(cityName);
            ApplyWeatherRecommendation(weather);
        }
        catch
        {
            WeatherTitleLabel.Text = "Weather is unavailable";
            WeatherBodyLabel.Text = "You can still explore every seasonal collection below.";
        }
    }

    private void ApplyWeatherRecommendation(WeatherSnapshot weather)
    {
        var targetSeason = GetRecommendedSeason(weather);
        foreach (var season in SeasonSections)
        {
            season.IsExpanded = season.Key == targetSeason;
        }

        WeatherTitleLabel.Text = $"{weather.CityName}, {weather.Temperature:F1}°C";
        WeatherBodyLabel.Text = targetSeason switch
        {
            "Summer" => $"Humidity is {weather.Humidity}%. Start with light, breathable layers.",
            "Autumn" => "Rain is in the forecast. Start with balanced, cosy layers.",
            "Winter" => "The air is cold. Start with insulating quilts and bedding.",
            _ => "The weather is mild. Start with breathable spring layers."
        };
    }

    private static string GetRecommendedSeason(WeatherSnapshot weather)
    {
        if (weather.CityName == "Singapore")
        {
            return weather.IsRainy ? "Autumn" : "Summer";
        }

        if (weather.Temperature < 10)
        {
            return "Winter";
        }

        return weather.Temperature <= 25 ? "Spring" : "Summer";
    }

    private static void ReplaceItems<T>(ObservableCollection<T> target, IEnumerable<T> items)
    {
        target.Clear();
        foreach (var item in items)
        {
            target.Add(item);
        }
    }
}
