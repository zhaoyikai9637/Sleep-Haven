using System.Collections.ObjectModel;

namespace SleepHaven;

public partial class CategoryPage : ContentPage
{
    private static readonly IReadOnlyDictionary<string, string> CategoryTitles =
        new Dictionary<string, string>
        {
            ["Pillows"] = "Cozy Pillows",
            ["Quilts"] = "Warm Quilts",
            ["BeddingSets"] = "Premium Bedding Sets",
            ["Mattresses"] = "Supportive Mattresses"
        };

    private readonly DatabaseService _databaseService = new();
    private string _currentCategory = "Pillows";

    public ObservableCollection<Product> FilteredProducts { get; } = [];

    public CategoryPage()
    {
        InitializeComponent();
        CategoryCollectionView.ItemsSource = FilteredProducts;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await SelectCategoryAsync(_currentCategory);
    }

    private async void OnCategoryTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is string category && CategoryTitles.ContainsKey(category))
        {
            await SelectCategoryAsync(category);
        }
    }

    private async Task SelectCategoryAsync(string category)
    {
        _currentCategory = category;
        CurrentCategoryTitle.Text = CategoryTitles[category];
        UpdateTabStyles(category);

        var products = await _databaseService.GetAllProductsAsync();
        ReplaceItems(FilteredProducts, products.Where(product =>
            product.Category.Contains(category, StringComparison.OrdinalIgnoreCase)));

        var hasProducts = FilteredProducts.Count > 0;
        EmptyStateLabel.IsVisible = !hasProducts;
        CategoryCollectionView.IsVisible = hasProducts;
    }

    private async void OnProductSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Product product)
        {
            return;
        }

        CategoryCollectionView.SelectedItem = null;
        await Navigation.PushAsync(new ProductDetailPage(product));
    }

    private void UpdateTabStyles(string selectedCategory)
    {
        foreach (var (category, button, label) in GetTabs())
        {
            var selected = category == selectedCategory;
            button.BackgroundColor = selected ? Colors.White : Colors.Transparent;
            label.TextColor = selected ? Color.FromArgb("#1A2980") : Colors.Gray;
            label.FontAttributes = selected ? FontAttributes.Bold : FontAttributes.None;
        }
    }

    private IEnumerable<(string Category, Border Button, Label Label)> GetTabs()
    {
        yield return ("Pillows", BtnPillows, LblPillows);
        yield return ("Quilts", BtnQuilts, LblQuilts);
        yield return ("BeddingSets", BtnBeddingSets, LblBeddingSets);
        yield return ("Mattresses", BtnMattresses, LblMattresses);
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
