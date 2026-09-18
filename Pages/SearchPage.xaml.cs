using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;

namespace SleepHaven;

public partial class SearchPage : ContentPage
{
    private readonly string _query;
    private readonly DatabaseService _databaseService;
    private readonly ProductCatalogService _catalogService;
    private readonly AsyncNavigationGuard _navigationGuard;
    private readonly ILogger _logger;
    private bool _hasLoaded;

    public ObservableCollection<Product> SearchResults { get; } = [];

    public SearchPage(
        string query,
        DatabaseService databaseService,
        ProductCatalogService catalogService,
        AsyncNavigationGuard navigationGuard,
        ILogger logger)
    {
        InitializeComponent();
        _query = query;
        _databaseService = databaseService;
        _catalogService = catalogService;
        _navigationGuard = navigationGuard;
        _logger = logger;
        SearchResultTitle.Text = $"Results for \"{query}\"";
        ResultsCollectionView.ItemsSource = SearchResults;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_hasLoaded) return;
        _hasLoaded = true;

        try
        {
            foreach (var product in await _catalogService.SearchAsync(_query)) SearchResults.Add(product);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Search results could not be loaded.");
        }

        EmptyStateContainer.IsVisible = SearchResults.Count == 0;
    }

    private async void OnProductSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Product product) return;
        ResultsCollectionView.SelectedItem = null;
        await _navigationGuard.TryRunAsync(() =>
            Navigation.PushAsync(new ProductDetailPage(product, _databaseService, _catalogService)));
    }
}
