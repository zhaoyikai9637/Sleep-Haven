using System.Collections.ObjectModel;

namespace SleepHaven;

public partial class HomePage : ContentPage
{
    private readonly DatabaseService _databaseService = new();
    private readonly WeatherService _weatherService = new(new HttpClient());
    private bool _isFirstLoad = true;
    private bool _hasAnimated;

    public ObservableCollection<CarouselItem> CarouselItems { get; } =
    [
        new("p012", "restful_recovery_memory_pillow_bedroom.png", "01", "SUPPORT / RECOVERY", "Restful Recovery Memory Pillow", "Pressure-aware support for slower, deeper nights."),
        new("p007", "mulberrysilk_summerquilt_bedroom.png", "02", "LIGHT / BREATHABLE", "Mulberry Silk Summer Quilt", "A weightless layer selected for warm, humid air."),
        new("p003", "tencelcotton_abdual_use_bedroom.png", "03", "SOFT / ADAPTABLE", "Tencel Cotton Bedding Set", "Two-sided comfort with a cool, fluid hand feel.")
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

        await AnimateEntryAsync();
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
            var matches = products.Where(product => product.Category.Contains(definition.Key, StringComparison.OrdinalIgnoreCase));
            SeasonSections.Add(new SeasonSection(definition.Key, definition.Title, definition.Summary, matches, definition.Key == "Spring"));
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

    private void MoveHeroBy(int offset)
    {
        if (CarouselItems.Count == 0)
        {
            return;
        }

        var current = Math.Clamp(BannerCarousel.Position, 0, CarouselItems.Count - 1);
        var target = (current + offset + CarouselItems.Count) % CarouselItems.Count;
        var crossesBoundary = Math.Abs(target - current) > 1;
        BannerCarousel.ScrollTo(target, animate: !crossesBoundary);
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
            product.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            product.Description.Contains(query, StringComparison.OrdinalIgnoreCase)));

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
            ? (Color?)resources?["Accent"] ?? Color.FromArgb("#0878F9")
            : (Color?)resources?[isDark ? "DarkSurfaceMuted" : "AccentMist"] ?? Color.FromArgb("#DDF2FF");
        button.TextColor = selected
            ? Colors.White
            : (Color?)resources?[isDark ? "AccentDark" : "AccentDeep"] ?? Color.FromArgb("#064A9B");
    }

    private async Task FetchWeatherAndRecommendAsync(string cityName)
    {
        WeatherRecommendationCard.IsVisible = true;
        WeatherTitleLabel.Text = $"Reading {cityName}'s night";
        WeatherBodyLabel.Text = "Selecting a comfortable material profile.";

        try
        {
            var weather = await _weatherService.GetCurrentAsync(cityName);
            ApplyWeatherRecommendation(weather);
        }
        catch
        {
            WeatherTitleLabel.Text = "Weather signal unavailable";
            WeatherBodyLabel.Text = "Every seasonal edit remains available below.";
        }
    }

    private void ApplyWeatherRecommendation(WeatherSnapshot weather)
    {
        var targetSeason = GetRecommendedSeason(weather);
        foreach (var season in SeasonSections)
        {
            season.IsExpanded = season.Key == targetSeason;
        }

        WeatherTitleLabel.Text = $"{weather.CityName} / {weather.Temperature:F1}°C";
        WeatherBodyLabel.Text = targetSeason switch
        {
            "Summer" => $"Humidity is {weather.Humidity}%. Begin with light, breathable layers.",
            "Autumn" => "Rain is in the forecast. Begin with balanced, cosy layers.",
            "Winter" => "The air is cold. Begin with insulating quilts and bedding.",
            _ => "The weather is mild. Begin with breathable spring layers."
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
