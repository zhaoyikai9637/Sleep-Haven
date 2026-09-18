namespace SleepHaven;

public partial class ProductDetailPage : ContentPage
{
    private readonly Product _currentProduct;
    private readonly DatabaseService _databaseService;
    private readonly ProductCatalogService _catalogService;
    private bool? _isLandscape;

    public ProductDetailPage(
        Product product,
        DatabaseService databaseService,
        ProductCatalogService catalogService)
    {
        InitializeComponent();
        _currentProduct = product;
        _databaseService = databaseService;
        _catalogService = catalogService;
        ProductImage.Source = product.ThumbnailUrl;
        SemanticProperties.SetDescription(ProductImage, product.Name);
        NameLabel.Text = product.Name;
        PriceLabel.Text = product.FormattedPrice;
        CategoryLabel.Text = product.CategoryDisplay;
        DescLabel.Text = product.Description;
        MaterialValueLabel.Text = product.Material.ToDisplayName();
        SeasonValueLabel.Text = product.Season.ToString();
        ProfileValueLabel.Text = InferProfile(product);
        UpdateFavoriteIcon();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var freshProduct = await _databaseService.GetProductByIdAsync(_currentProduct.Id);
        if (freshProduct is not null)
        {
            _currentProduct.IsFavorite = freshProduct.IsFavorite;
            UpdateFavoriteIcon();
        }
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        var isLandscape = width > height;
        if (_isLandscape == isLandscape) return;

        _isLandscape = isLandscape;
        if (isLandscape)
        {
            ContentGrid.RowDefinitions.Clear();
            ContentGrid.ColumnDefinitions.Clear();
            ContentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(6, GridUnitType.Star) });
            ContentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) });
            Grid.SetColumn(ImageFrame, 0);
            Grid.SetRow(ImageFrame, 0);
            Grid.SetColumn(TextScrollView, 1);
            Grid.SetRow(TextScrollView, 0);
            ImageFrame.HeightRequest = -1;
            ProductImage.Source = _currentProduct.LandscapeUrl;
            ProductImage.Aspect = Aspect.AspectFill;
        }
        else
        {
            ContentGrid.ColumnDefinitions.Clear();
            ContentGrid.RowDefinitions.Clear();
            ContentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            ContentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            Grid.SetColumn(ImageFrame, 0);
            Grid.SetRow(ImageFrame, 0);
            Grid.SetColumn(TextScrollView, 0);
            Grid.SetRow(TextScrollView, 1);
            ImageFrame.HeightRequest = 320;
            ProductImage.Source = _currentProduct.ThumbnailUrl;
            ProductImage.Aspect = Aspect.AspectFill;
        }
    }

    private async void OnFavoriteClicked(object sender, EventArgs e)
    {
        _currentProduct.IsFavorite = !_currentProduct.IsFavorite;
        await _databaseService.SetFavoriteAsync(_currentProduct.Id, _currentProduct.IsFavorite);
        _catalogService.Invalidate();
        UpdateFavoriteIcon();
    }

    private void UpdateFavoriteIcon() =>
        SaveButton.Text = _currentProduct.IsFavorite ? "SAVED TO YOUR SLEEP EDIT" : "SAVE TO YOUR SLEEP EDIT";

    private static string InferProfile(Product product) => product.ProductType switch
    {
        ProductType.Pillow or ProductType.Mattress => "Supportive",
        _ when product.Season == ProductSeason.Summer || product.Material == ProductMaterial.Silk => "Cooling",
        _ when product.Season == ProductSeason.Winter => "Warm",
        _ => "Balanced"
    };
}
