namespace SleepHaven;

public partial class ProductDetailPage : ContentPage
{
    Product _currentProduct;
    DatabaseService _databaseService;

    public ProductDetailPage(Product product)
    {
        InitializeComponent();

        _currentProduct = product;
        _databaseService = new DatabaseService();

        ProductImage.Source = product.ThumbnailUrl;
        NameLabel.Text = product.Name;
        PriceLabel.Text = product.Price;
        CategoryLabel.Text = product.Category;
        DescLabel.Text = product.Description;

        UpdateFavoriteIcon();

        Shell.Current.Navigated += async (sender, args) =>
        {
            var freshProduct = await _databaseService.GetProductByIdAsync(_currentProduct.Id);
            if (freshProduct != null)
            {
                _currentProduct.IsFavorite = freshProduct.IsFavorite;
                UpdateFavoriteIcon();
            }
        };
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

        if (width > height) // Horizontal screen logic
        {
            ContentGrid.RowDefinitions.Clear();
            ContentGrid.ColumnDefinitions.Clear();
            ContentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(6, GridUnitType.Star) });
            ContentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) });

            Grid.SetColumn(ProductImage, 0);
            Grid.SetRow(ProductImage, 0);

            Grid.SetColumn(TextScrollView, 1);
            Grid.SetRow(TextScrollView, 0);

            ProductImage.HeightRequest = -1;
            ProductImage.Source = _currentProduct.LandscapeUrl;
            ProductImage.Aspect = Aspect.AspectFill;
        }
        else // Vertical screen logic (created by Gemini)
        {
            ContentGrid.ColumnDefinitions.Clear();
            ContentGrid.RowDefinitions.Clear();
            ContentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            ContentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

            Grid.SetColumn(ProductImage, 0);
            Grid.SetRow(ProductImage, 0);

            Grid.SetColumn(TextScrollView, 0);
            Grid.SetRow(TextScrollView, 1);

            ProductImage.HeightRequest = 300;
            ProductImage.Source = _currentProduct.ThumbnailUrl;
            ProductImage.Aspect = Aspect.AspectFit;
        }
    }

    private async void OnFavoriteClicked(object sender, EventArgs e)
    {
        _currentProduct.IsFavorite = !_currentProduct.IsFavorite;
        await _databaseService.UpdateProductAsync(_currentProduct);
        UpdateFavoriteIcon();

        if (_currentProduct.IsFavorite)
        {
            await DisplayAlertAsync("Success", "Added to your collection!", "OK");
        }
    }

    //Red Heart UI Status Update Function
    private void UpdateFavoriteIcon()
    {
        FavoriteToolbarItem.IconImageSource = _currentProduct.IsFavorite ? "collect_on.png" : "collect_off.png";
    }
}