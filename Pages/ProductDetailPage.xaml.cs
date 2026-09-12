namespace SleepHaven;

public partial class ProductDetailPage : ContentPage
{
    private readonly Product _currentProduct;
    private readonly DatabaseService _databaseService = new();
    private bool? _isLandscape;

    public ProductDetailPage(Product product)
    {
        InitializeComponent();

        _currentProduct = product;
        ProductImage.Source = product.ThumbnailUrl;
        SemanticProperties.SetDescription(ProductImage, product.Name);
        NameLabel.Text = product.Name;
        PriceLabel.Text = product.Price;
        CategoryLabel.Text = product.Category;
        DescLabel.Text = product.Description;

        UpdateFavoriteIcon();

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var freshProduct = await _databaseService.GetProductByIdAsync(_currentProduct.Id);
        if (freshProduct != null)
        {
            _currentProduct.IsFavorite = freshProduct.IsFavorite;
            UpdateFavoriteIcon();
        }
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        var isLandscape = width > height;
        if (_isLandscape == isLandscape)
        {
            return;
        }

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
        await _databaseService.UpdateProductAsync(_currentProduct);
        UpdateFavoriteIcon();

    }

    private void UpdateFavoriteIcon()
    {
        FavoriteToolbarItem.IconImageSource = _currentProduct.IsFavorite ? "collect_on.png" : "collect_off.png";
        FavoriteToolbarItem.Text = _currentProduct.IsFavorite ? "Saved" : "Save";
    }
}
