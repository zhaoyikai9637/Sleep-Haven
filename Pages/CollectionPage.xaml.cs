using System.Collections.ObjectModel;

namespace SleepHaven;

public partial class CollectionPage : ContentPage
{
    private readonly DatabaseService _databaseService = new();

    public ObservableCollection<Product> FavoriteProducts { get; } = [];

    public CollectionPage()
    {
        InitializeComponent();
        FavoritesCollectionView.ItemsSource = FavoriteProducts;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadFavoritesAsync();
    }

    private async Task LoadFavoritesAsync()
    {
        var favorites = await _databaseService.GetFavoriteProductsAsync();
        FavoriteProducts.Clear();
        foreach (var product in favorites)
        {
            FavoriteProducts.Add(product);
        }

        var hasFavorites = FavoriteProducts.Count > 0;
        EmptyStateLabel.IsVisible = !hasFavorites;
        FavoritesCollectionView.IsVisible = hasFavorites;
    }

    private async void OnProductSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Product product)
        {
            return;
        }

        FavoritesCollectionView.SelectedItem = null;
        await Navigation.PushAsync(new ProductDetailPage(product));
    }

    private async void OnRemoveClicked(object sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: Product product })
        {
            return;
        }

        product.IsFavorite = false;
        await _databaseService.UpdateProductAsync(product);
        await LoadFavoritesAsync();
    }
}
