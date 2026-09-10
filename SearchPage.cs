using System.Collections.ObjectModel;

namespace SleepHaven;

public partial class SearchPage : ContentPage
{
    DatabaseService _databaseService = new DatabaseService();
    public ObservableCollection<Product> SearchResults { get; set; } = new ObservableCollection<Product>();

    public SearchPage(string query)
    {
        InitializeComponent();

        // Bind data source
        ResultsCollectionView.ItemsSource = SearchResults;

        // Update the title
        SearchResultTitle.Text = $"Results for \"{query}\"";

        // Start the bottom-level search immediately
        ExecuteSearchAsync(query);
    }

    // Created by ai 
    private async void ExecuteSearchAsync(string query)
    {
        // Convert the words entered by the user to lowercase to facilitate the comparison that ignores case differences.
        var lowerQuery = query.ToLower();

        // Take out all the goods from the database.
        var allProducts = await _databaseService.GetAllProductsAsync();

        SearchResults.Clear();

        // As long as the search term is included in the name or description, it will be retrieved.
        foreach (var product in allProducts)
        {
            if ((product.Name != null && product.Name.ToLower().Contains(lowerQuery)) ||
                (product.Description != null && product.Description.ToLower().Contains(lowerQuery)))
            {
                SearchResults.Add(product);
            }
        }

        // If no result is found, display an empty interface.
        EmptyStateContainer.IsVisible = SearchResults.Count == 0;
    }

    // The logic of clicking and jumping between the items in the search result list
    private async void OnProductSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Product selectedProduct)
        {
            await Navigation.PushAsync(new ProductDetailPage(selectedProduct));
            // Remove the remaining gray selected state
            ResultsCollectionView.SelectedItem = null;
        }
    }
}