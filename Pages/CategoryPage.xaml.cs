using System.Collections.ObjectModel;

namespace SleepHaven;

public partial class CategoryPage : ContentPage
{
    private static readonly IReadOnlyDictionary<string, string> CategoryTitles =
        new Dictionary<string, string>
        {
            ["Pillows"] = "Cozy pillows",
            ["Quilts"] = "Warm quilts",
            ["BeddingSets"] = "Bedding sets",
            ["Mattresses"] = "Supportive mattresses"
        };

    private readonly DatabaseService _databaseService = new();
    private string _currentCategory = "Pillows";
    private int _currentSpan = 2;

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
        ResultCountLabel.Text = FilteredProducts.Count.ToString("00");

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
        var resources = Application.Current?.Resources;
        var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;
        var selectedBackground = (Color?)resources?["Accent"] ?? Color.FromArgb("#0878F9");
        var selectedText = Colors.White;
        var idleBackground = (Color?)resources?[isDark ? "DarkSurfaceMuted" : "AccentMist"] ?? Color.FromArgb("#DDF2FF");
        var idleText = (Color?)resources?[isDark ? "AccentDark" : "AccentDeep"] ?? Color.FromArgb("#064A9B");

        foreach (var (category, button, label) in GetTabs())
        {
            var selected = category == selectedCategory;
            button.BackgroundColor = selected ? selectedBackground : idleBackground;
            label.TextColor = selected ? selectedText : idleText;
        }
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        var span = width >= 1100 ? 3 : width >= 620 ? 2 : 1;
        if (span != _currentSpan)
        {
            _currentSpan = span;
            CategoryGridLayout.Span = span;
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
