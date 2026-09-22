using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;

namespace SleepHaven;

public partial class CategoryPage : ContentPage
{
    private static readonly IReadOnlyDictionary<string, (string Title, ProductType Type)> Categories =
        new Dictionary<string, (string, ProductType)>
        {
            ["Pillows"] = ("Cozy pillows", ProductType.Pillow),
            ["Quilts"] = ("Warm quilts", ProductType.Quilt),
            ["BeddingSets"] = ("Bedding sets", ProductType.BeddingSet),
            ["Mattresses"] = ("Supportive mattresses", ProductType.Mattress)
        };

    private readonly IProductStore _productStore;
    private readonly ProductCatalogService _catalogService;
    private readonly AsyncNavigationGuard _navigationGuard;
    private readonly ILogger<CategoryPage> _logger;
    private string _currentCategory = "Pillows";
    private int _currentSpan = 2;

    public ObservableCollection<Product> FilteredProducts { get; } = [];

    public CategoryPage(
        IProductStore productStore,
        ProductCatalogService catalogService,
        AsyncNavigationGuard navigationGuard,
        ILogger<CategoryPage> logger)
    {
        InitializeComponent();
        _productStore = productStore;
        _catalogService = catalogService;
        _navigationGuard = navigationGuard;
        _logger = logger;
        CategoryCollectionView.ItemsSource = FilteredProducts;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await SelectCategoryAsync(_currentCategory);
    }

    private async void OnCategoryTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is string category && Categories.ContainsKey(category))
        {
            await SelectCategoryAsync(category);
        }
    }

    private async Task SelectCategoryAsync(string category)
    {
        _currentCategory = category;
        CurrentCategoryTitle.Text = Categories[category].Title;
        UpdateTabStyles(category);

        try
        {
            ReplaceItems(FilteredProducts, await _catalogService.GetByTypeAsync(Categories[category].Type));
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "The product category could not be loaded.");
            FilteredProducts.Clear();
        }

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
        await _navigationGuard.TryRunAsync(() =>
            Navigation.PushAsync(new ProductDetailPage(product, _productStore, _catalogService)));
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
        foreach (var item in items) target.Add(item);
    }
}
