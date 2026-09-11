using System.Collections.ObjectModel;

namespace SleepHaven;

public partial class HomePage : ContentPage
{
    private readonly DatabaseService _databaseService = new();
    private readonly WeatherService _weatherService = new(new HttpClient());
    private IDispatcherTimer? _carouselTimer;
    private bool _isFirstLoad = true;

    public ObservableCollection<CarouselItem> CarouselItems { get; } =
    [
        new("p012", "restful_recovery_memory_pillow_bedroom.png", "Recovery Sleep | Restful Recovery Memory Pillow"),
        new("p007", "mulberrysilk_summerquilt_bedroom.png", "Summer Silk | Silk summer dress"),
        new("p003", "tencelcotton_abdual_use_bedroom.png", "Tencel Comfort | Tencel Cotton AB Dual-Purpose Bedding Set")
    ];

    public ObservableCollection<Product> SearchSuggestions { get; } = [];

    public HomePage()
    {
        InitializeComponent();
        SuggestionsCollectionView.ItemsSource = SearchSuggestions;
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_isFirstLoad)
        {
            _isFirstLoad = false;
            await FetchWeatherAndRecommendAsync("Singapore");
        }

        StartCarousel();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _carouselTimer?.Stop();
    }

    private void StartCarousel()
    {
        _carouselTimer ??= CreateCarouselTimer();
        if (!_carouselTimer.IsRunning)
        {
            _carouselTimer.Start();
        }
    }

    private IDispatcherTimer CreateCarouselTimer()
    {
        var timer = Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromSeconds(3);
        timer.Tick += (_, _) =>
        {
            if (CarouselItems.Count == 0)
            {
                return;
            }

            var nextIndex = (BannerCarousel.Position + 1) % CarouselItems.Count;
            BannerCarousel.ScrollTo(nextIndex, animate: nextIndex != 0);
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
        if (e.CurrentSelection.FirstOrDefault() is not Product selectedProduct)
        {
            return;
        }

        SuggestionsCollectionView.SelectedItem = null;
        MainSearchBar.Text = string.Empty;
        await Navigation.PushAsync(new ProductDetailPage(selectedProduct));
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

    private void OnSpringTapped(object sender, EventArgs e) => ToggleSeason(SpringGrid, SpringArrow);
    private void OnSummerTapped(object sender, EventArgs e) => ToggleSeason(SummerGrid, SummerArrow);
    private void OnAutumnTapped(object sender, EventArgs e) => ToggleSeason(AutumnGrid, AutumnArrow);
    private void OnWinterTapped(object sender, EventArgs e) => ToggleSeason(WinterGrid, WinterArrow);

    private static void ToggleSeason(VisualElement panel, Label arrow)
    {
        panel.IsVisible = !panel.IsVisible;
        arrow.Text = panel.IsVisible ? "▲" : "▼";
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
        var singaporeSelected = cityName == "Singapore";
        SetCityButtonState(BtnSingapore, singaporeSelected);
        SetCityButtonState(BtnQingdao, !singaporeSelected);
        await FetchWeatherAndRecommendAsync(cityName);
    }

    private static void SetCityButtonState(Button button, bool selected)
    {
        button.BackgroundColor = Color.FromArgb(selected ? "#1A2980" : "#F5F5F5");
        button.TextColor = selected ? Colors.White : Colors.Gray;
    }

    private async Task FetchWeatherAndRecommendAsync(string cityName)
    {
        try
        {
            var weather = await _weatherService.GetCurrentAsync(cityName);
            await ApplyWeatherRecommendationAsync(weather);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Network Error", $"Unable to retrieve weather: {ex.Message}", "OK");
        }
    }

    private async Task ApplyWeatherRecommendationAsync(WeatherSnapshot weather)
    {
        CollapseSeasons();

        if (weather.CityName == "Singapore" && weather.IsRainy)
        {
            await ShowRecommendationAsync(AutumnGrid, AutumnArrow, "It's raining in Singapore 🌧️\nWe've selected cozy autumn products for you.", "Explore Now");
        }
        else if (weather.CityName == "Singapore")
        {
            await ShowRecommendationAsync(SummerGrid, SummerArrow, $"Singapore humidity is {weather.Humidity}% 💦\nWe've selected breathable summer products for you.", "Stay Cool");
        }
        else if (weather.CityName == "Qingdao" && weather.Temperature is >= 10 and <= 25)
        {
            await ShowRecommendationAsync(SpringGrid, SpringArrow, $"Qingdao is {weather.Temperature:F1}°C 🌸\nWe've selected spring products for you.", "View Recommendations");
        }
        else if (weather.Temperature < 10)
        {
            await ShowRecommendationAsync(WinterGrid, WinterArrow, $"It's {weather.Temperature:F1}°C ❄️\nWe've selected warm winter products for you.", "Stay Warm");
        }
        else
        {
            await ShowRecommendationAsync(SpringGrid, SpringArrow, $"The weather is pleasant at {weather.Temperature:F1}°C 🌸\nHere are today's picks.", "View");
        }
    }

    private void CollapseSeasons()
    {
        foreach (var (panel, arrow) in GetSeasonControls())
        {
            panel.IsVisible = false;
            arrow.Text = "▼";
        }
    }

    private async Task ShowRecommendationAsync(VisualElement panel, Label arrow, string message, string buttonText)
    {
        panel.IsVisible = true;
        arrow.Text = "▲";
        await DisplayAlertAsync("Smart Assistant", message, buttonText);
    }

    private IEnumerable<(VisualElement Panel, Label Arrow)> GetSeasonControls()
    {
        yield return (SpringGrid, SpringArrow);
        yield return (SummerGrid, SummerArrow);
        yield return (AutumnGrid, AutumnArrow);
        yield return (WinterGrid, WinterArrow);
    }

    private static void ReplaceItems<T>(ObservableCollection<T> target, IEnumerable<T> items)
    {
        target.Clear();
        foreach (var item in items)
        {
            target.Add(item);
        }
    }

    public sealed record CarouselItem(string Id, string ImageUrl, string Title);
}
