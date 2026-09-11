using System.Collections.ObjectModel;

namespace SleepHaven;

public partial class CollectionPage : ContentPage
{
    // Introduce the database
    DatabaseService _databaseService;

    // Data source binding collection: Ensure that changes to the collection can automatically notify the UI for a refresh.
    public ObservableCollection<Product> FavoriteProducts { get; set; } = new ObservableCollection<Product>();

    public CollectionPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();

        // Specify the data source for the UI list control
        FavoritesCollectionView.ItemsSource = FavoriteProducts;
    }

    // Whenever a user switches to the "Favorites" tab, this method will be triggered.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Every time the page is accessed, the latest collection list is retrieved from the database.
        await LoadFavoritesFromDatabaseAsync();
    }

    private async Task LoadFavoritesFromDatabaseAsync()
    {
        // Retrieve all the products where the value of IsFavorite is true from the SQLite database.
        var favorites = await _databaseService.GetFavoriteProductsAsync();

        // Clear the old list and add the new data retrieved from the database.
        FavoriteProducts.Clear();
        foreach (var item in favorites)
        {
            FavoriteProducts.Add(item);
        }

        // If the list is empty, display the prompt text "Empty as a cup"; if there are data, display the list of products.
        EmptyStateLabel.IsVisible = FavoriteProducts.Count == 0;
        FavoritesCollectionView.IsVisible = FavoriteProducts.Count > 0;
    }

    // When the user clicks on a certain item in the product list, it triggers (redirecting back to the detail page)
    private async void OnProductSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Product selectedProduct)
        {
            await Navigation.PushAsync(new ProductDetailPage(selectedProduct));
            FavoritesCollectionView.SelectedItem = null; // Clear selection state
        }
    }

    // It is triggered when the user clicks the remove button on the right side of the product card.
    private async void OnRemoveClicked(object sender, EventArgs e)
    {
        // Obtain which product the user intends to remove
        if (sender is Button btn && btn.CommandParameter is Product productToRemove)
        {
            // Change its red heart state to false
            productToRemove.IsFavorite = false;

            // This modification has been saved in SQLite.
            await _databaseService.UpdateProductAsync(productToRemove);

            // Reload the list again
            await LoadFavoritesFromDatabaseAsync();
        }
    }
}