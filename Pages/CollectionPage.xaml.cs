using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;

namespace SleepHaven;

public partial class CollectionPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private readonly ProductCatalogService _catalogService;
    private readonly AsyncNavigationGuard _navigationGuard;
    private readonly ILogger<CollectionPage> _logger;

    public ObservableCollection<Product> FavoriteProducts { get; } = [];

    public CollectionPage(
        DatabaseService databaseService,
        ProductCatalogService catalogService,
        AsyncNavigationGuard navigationGuard,
        ILogger<CollectionPage> logger)
    {
        InitializeComponent();
        _databaseService = databaseService;
        _catalogService = catalogService;
        _navigationGuard = navigationGuard;
        _logger = logger;
        FavoritesCollectionView.ItemsSource = FavoriteProducts;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadFavoritesAsync();
    }

    private async Task LoadFavoritesAsync()
    {
        try
        {
            ReplaceItems(FavoriteProducts, await _databaseService.GetFavoriteProductsAsync());
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Saved products could not be loaded.");
            FavoriteProducts.Clear();
        }

        var hasFavorites = FavoriteProducts.Count > 0;
        EmptyStateLabel.IsVisible = !hasFavorites;
        FavoritesCollectionView.IsVisible = hasFavorites;
    }

    private async void OnProductSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Product product) return;
        FavoritesCollectionView.SelectedItem = null;
        await _navigationGuard.TryRunAsync(() =>
            Navigation.PushAsync(new ProductDetailPage(product, _databaseService, _catalogService)));
    }

    private async void OnRemoveClicked(object sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: Product product }) return;

        try
        {
            await _databaseService.SetFavoriteAsync(product.Id, false);
            _catalogService.Invalidate();
            await LoadFavoritesAsync();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "A saved product could not be removed.");
        }
    }

    private static void ReplaceItems<T>(ObservableCollection<T> target, IEnumerable<T> items)
    {
        target.Clear();
        foreach (var item in items) target.Add(item);
    }
}
