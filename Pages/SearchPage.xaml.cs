using System.Collections.ObjectModel;

namespace SleepHaven;

public partial class SearchPage : ContentPage
{
    private readonly DatabaseService _databaseService = new();
    private readonly string _query;
    private bool _hasLoaded;

    public ObservableCollection<Product> SearchResults { get; } = [];

    public SearchPage(string query)
    {
        InitializeComponent();
        _query = query;
        SearchResultTitle.Text = $"Results for \"{query}\"";
        ResultsCollectionView.ItemsSource = SearchResults;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_hasLoaded)
        {
            return;
        }

        _hasLoaded = true;
        var products = await _databaseService.GetAllProductsAsync();
        var matches = products.Where(product =>
            product.Name.Contains(_query, StringComparison.OrdinalIgnoreCase) ||
            product.Description.Contains(_query, StringComparison.OrdinalIgnoreCase));

        foreach (var product in matches)
        {
            SearchResults.Add(product);
        }

        EmptyStateContainer.IsVisible = SearchResults.Count == 0;
    }

    private async void OnProductSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Product product)
        {
            return;
        }

        ResultsCollectionView.SelectedItem = null;
        await Navigation.PushAsync(new ProductDetailPage(product));
    }
}
