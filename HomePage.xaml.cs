using System.Collections.ObjectModel;
using System.Net.Http;       // Used for sending network requests
using System.Text.Json;      // Used for parsing weather data

namespace SleepHaven;

public partial class HomePage : ContentPage
{
    // Add a switch to ensure that the pop-up window appears only once when the app is launched.
    // Avoid having the pop-up window appear every time a user switches back to the homepage from another page.
    private bool _isFirstLoad = true;

    private bool _isTimerRunning = false;

    // Each time the page is displayed on the screen, it will trigger.
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // If this is the first time loading
        if (_isFirstLoad)
        {
            _isFirstLoad = false;

            // Automatically trigger a weather query for Singapore and update the UI.
            await FetchWeatherAndRecommendAsync("Singapore");
        }

        if (!_isTimerRunning)
        {
            _isTimerRunning = true;

            Dispatcher.StartTimer(TimeSpan.FromSeconds(3), () =>
            {
                if (BannerCarousel == null || CarouselItems == null || CarouselItems.Count == 0)
                    return false;

                int nextIndex = (BannerCarousel.Position + 1) % CarouselItems.Count;
                bool shouldAnimate = nextIndex != 0;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    BannerCarousel.ScrollTo(nextIndex, animate: shouldAnimate);
                });

                return _isTimerRunning;
            });
        }
    }

    // The data set used for binding the carousel images
    public ObservableCollection<CarouselItem> CarouselItems { get; set; }

    // The list used for binding real-time search suggestions
    public ObservableCollection<Product> SearchSuggestions { get; set; } = new ObservableCollection<Product>();

    // Instantiate the database service and prepare to read data at any time.
    DatabaseService _databaseService = new DatabaseService();

    public HomePage()
    {
        InitializeComponent();

        // Bind the search suggestion list to the bottom mask layer
        SuggestionsCollectionView.ItemsSource = SearchSuggestions;

        // Create fake data for the homepage carousel display
        CarouselItems = [
            new() { Id = "p012", ImageUrl = "restful_recovery_memory_pillow_bedroom.png", Title = "Recovery Sleep | Restful Recovery Memory Pillow" },
            new() { Id = "p007", ImageUrl = "mulberrysilk_summerquilt_bedroom.png", Title = "Summer Silk | Silk summer dress" },
            new() { Id = "p003", ImageUrl = "tencelcotton_abdual_use_bedroom.png", Title = "Tencel Comfort | Tencel Cotton AB Dual-Purpose Bedding Set" }
        ];

        BindingContext = this;
    }


    // It triggers whenever the user types in a single letter.
    private async void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
    {
        string query = e.NewTextValue?.Trim();

        // If the search box is empty, hide the overlay layer and reveal the original content of the homepage.
        if (string.IsNullOrEmpty(query))
        {
            SearchSuggestionsOverlay.IsVisible = false;
            MainContentScrollView.IsVisible = true;
            SearchSuggestions.Clear();
            return;
        }

        // Real-time fuzzy query of the database
        var allProducts = await _databaseService.GetAllProductsAsync();
        SearchSuggestions.Clear();

        // Remove the punctuation marks and spaces on both sides of the product name,
        // and then perform the "initial letter matching" operation.
        var matches = allProducts.Where(p =>
            p.Name != null &&
            p.Name.Trim(' ', '"', '\'').ToLower().StartsWith(query.ToLower())
        ).ToList();

        foreach (var match in matches)
        {
            SearchSuggestions.Add(match);
        }

        // Hide the home page
        SearchSuggestionsOverlay.IsVisible = true;
        MainContentScrollView.IsVisible = false;
    }

    // When the user clicks on a certain product name that is displayed below as a suggestion
    private async void OnSuggestionSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Product selectedProduct)
        {
            // Remove the remaining gray selected state
            SuggestionsCollectionView.SelectedItem = null;

            // Clear the search box and restore the home page layout
            MainSearchBar.Text = string.Empty;

            // Directly navigate to the product details page with the data in hand.
            await Navigation.PushAsync(new ProductDetailPage(selectedProduct));
        }
    }

    // Compatible users with obsessive-compulsive disorder pressed the enter/search key
    private async void OnSearchButtonPressed(object sender, EventArgs e)
    {
        string query = MainSearchBar.Text?.Trim();
        if (!string.IsNullOrEmpty(query))
        {
            MainSearchBar.Text = string.Empty;
            await Navigation.PushAsync(new SearchPage(query));
        }
    }


    // The logic for unfolding/closing the folding panel
    private void OnSpringTapped(object sender, EventArgs e) { SpringGrid.IsVisible = !SpringGrid.IsVisible; SpringArrow.Text = SpringGrid.IsVisible ? "▲" : "▼"; }
    private void OnSummerTapped(object sender, EventArgs e) { SummerGrid.IsVisible = !SummerGrid.IsVisible; SummerArrow.Text = SummerGrid.IsVisible ? "▲" : "▼"; }
    private void OnAutumnTapped(object sender, EventArgs e) { AutumnGrid.IsVisible = !AutumnGrid.IsVisible; AutumnArrow.Text = AutumnGrid.IsVisible ? "▲" : "▼"; }
    private void OnWinterTapped(object sender, EventArgs e) { WinterGrid.IsVisible = !WinterGrid.IsVisible; WinterArrow.Text = WinterGrid.IsVisible ? "▲" : "▼"; }

    // No matter which product card the user clicks, this method will be triggered.
    private async void OnProductTapped(object sender, TappedEventArgs e)
    {
        // Obtain the CommandParameter passed from the card in XAML
        if (e.Parameter is string productId)
        {
            // Use this ID to query the complete and accurate information of the actual product in the SQLite database.
            var realProduct = await _databaseService.GetProductByIdAsync(productId);

            // If the data is found, pass this data to the ProductDetailPage.
            if (realProduct != null)
            {
                await Navigation.PushAsync(new ProductDetailPage(realProduct));
            }
        }
    }

    //What information should a carousel project include?
    public class CarouselItem
    {
        public string Id { get; set; }
        public string ImageUrl { get; set; }
        public string Title { get; set; }
    }



    // API Key
    private const string OpenWeatherApiKey = "87aca45ad531775814b285a98598b2a9";

    // City Switch Click Events (Upgraded to async methods)

    private async void OnSingaporeClicked(object sender, EventArgs e)
    {
        // Toggle UI styling
        BtnSingapore.BackgroundColor = Color.FromArgb("#1A2980");
        BtnSingapore.TextColor = Colors.White;
        BtnQingdao.BackgroundColor = Color.FromArgb("#F5F5F5");
        BtnQingdao.TextColor = Colors.Gray;

        // Call Weather API
        await FetchWeatherAndRecommendAsync("Singapore");
    }

    private async void OnQingdaoClicked(object sender, EventArgs e)
    {
        // Toggle UI styling
        BtnQingdao.BackgroundColor = Color.FromArgb("#1A2980");
        BtnQingdao.TextColor = Colors.White;
        BtnSingapore.BackgroundColor = Color.FromArgb("#F5F5F5");
        BtnSingapore.TextColor = Colors.Gray;

        // Call Weather API
        await FetchWeatherAndRecommendAsync("Qingdao");
    }

    //Core Logic: Multi-dimensional Smart Weather Recommendations（Gemini)

    private async Task FetchWeatherAndRecommendAsync(string cityName)
    {
        try
        {
            using HttpClient client = new HttpClient();
            // Build request URL (units=metric for Celsius)
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={cityName}&units=metric&appid={OpenWeatherApiKey}";

            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                // Get the returned JSON text
                string json = await response.Content.ReadAsStringAsync();

                // Parse JSON to extract temperature, humidity, and weather conditions
                using JsonDocument doc = JsonDocument.Parse(json);
                JsonElement root = doc.RootElement;

                // 1. Extract temperature (temp) and humidity
                JsonElement mainNode = root.GetProperty("main");
                double temp = mainNode.GetProperty("temp").GetDouble();
                int humidity = mainNode.GetProperty("humidity").GetInt32();

                // 2. Extract weather condition (weather is an array, we take the 'main' property of the first element)
                string weatherCondition = root.GetProperty("weather")[0].GetProperty("main").GetString();

                // Execute smart recommendation UI update
                UpdateUIBasedOnWeather(cityName, temp, humidity, weatherCondition);
            }
            else
            {
                // 401 error usually means the Key is not yet activated(Gemini)
                await DisplayAlert("Notice", $"Failed to fetch weather. If you just applied for the Key, please wait 15 minutes and try again!\nStatus Code: {response.StatusCode}", "Got it");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Network Error", $"Unable to connect to weather server: {ex.Message}", "OK");
        }
    }


    // Created by Gemini
    private void UpdateUIBasedOnWeather(string cityName, double temp, int humidity, string weatherCondition)
    {
        // 1. Collapse all seasonal panels to reset the state
        SpringGrid.IsVisible = false; SpringArrow.Text = "▼";
        SummerGrid.IsVisible = false; SummerArrow.Text = "▼";
        AutumnGrid.IsVisible = false; AutumnArrow.Text = "▼";
        WinterGrid.IsVisible = false; WinterArrow.Text = "▼";

        // 2. Core recommendation logic: Tailored city and climate scenarios
        if (cityName == "Singapore" && (weatherCondition == "Rain" || weatherCondition == "Thunderstorm" || weatherCondition == "Drizzle"))
        {
            // 🇸🇬 Singapore rainy day logic: Slightly cool, recommend [Autumn Products]
            AutumnGrid.IsVisible = true;
            AutumnArrow.Text = "▲";
            DisplayAlert("Smart Assistant", $"It's raining in {cityName} 🌧️\nIt might get chilly in the AC room. We've recommended the cozy [Autumn Printed Set] and [Carbon Fiber Sleep Pad] for you!", "Explore Now");
        }
        else if (cityName == "Singapore")
        {
            // 🇸🇬 Singapore non-rainy logic: Hot and humid, recommend [Summer Products]
            SummerGrid.IsVisible = true;
            SummerArrow.Text = "▲";
            DisplayAlert("Smart Assistant", $"It's currently hot and humid in {cityName} (Humidity: {humidity}%) 💦\nYou need something breathable. We've automatically switched to the [Antibacterial Silk Summer Quilt] section!", "Stay Cool");
        }
        else if (cityName == "Qingdao" && temp >= 10 && temp <= 25)
        {
            // 🇨🇳 Qingdao spring logic: Temperature falls in the spring range (10℃~25℃)
            SpringGrid.IsVisible = true;
            SpringArrow.Text = "▲";
            DisplayAlert("Smart Assistant", $"Current temperature in {cityName} is {temp}°C 🌸\nThe spring breeze is gentle. We've automatically switched to the [Spring Pure Cotton Set] section!", "View Recommendations");
        }
        else if (temp < 10)
        {
            // General cold fallback logic: Below 10 degrees, auto-recommend [Winter Products]
            WinterGrid.IsVisible = true;
            WinterArrow.Text = "▲";
            DisplayAlert("Smart Assistant", $"Current temperature in {cityName} is only {temp}°C ❄️\nIt's freezing! We've automatically switched to the [White Goose Down Winter Quilt] recommendation section!", "Stay Warm");
        }
        else
        {
            // Default fallback logic
            SpringGrid.IsVisible = true;
            SpringArrow.Text = "▲";
            DisplayAlert("Smart Assistant", $"The weather is pleasant in {cityName} ({temp}°C) 🌸\nHere are our top picks for today!", "View");
        }
    }
    // Stop the timer when the user leaves this page.
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _isTimerRunning = false;
    }
}