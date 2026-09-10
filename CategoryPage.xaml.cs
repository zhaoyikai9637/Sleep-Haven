using System.Collections.ObjectModel;

namespace SleepHaven;

public partial class CategoryPage : ContentPage
{
    DatabaseService _databaseService = new DatabaseService();
    public ObservableCollection<Product> FilteredProducts { get; set; } = new ObservableCollection<Product>();

    // The default entry is Pillows, and it will change automatically as the user clicks
    private string _currentCategoryKeyword = "Pillows";

    public CategoryPage()
    {
        InitializeComponent();
        CategoryCollectionView.ItemsSource = FilteredProducts;
    }

    // Restore memory automatically when returning to the page.
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Make sure that the red heart status is up-to-date.
        await LoadCategoryDataAsync(_currentCategoryKeyword);

        // Force synchronization of UI highlights and titles to prevent misalignment of data and UI
        UpdateTabStyles(_currentCategoryKeyword);
        CurrentCategoryTitle.Text = _currentCategoryKeyword switch
        {
            "Pillows" => "Cozy Pillows",
            "Quilts" => "Warm Quilts",
            "BeddingSets" => "Premium Bedding Sets",
            "Mattresses" => "Supportive Mattresses",
            _ => "Our Collection"
        };
    }

    private async void OnCategoryTapped(object sender, TappedEventArgs e)
    {
        string category = e.Parameter as string;

        if (string.IsNullOrEmpty(category))
        {
            if (sender == BtnPillows) category = "Pillows";
            else if (sender == BtnQuilts) category = "Quilts";
            else if (sender == BtnBeddingSets) category = "BeddingSets";
            else if (sender == BtnMattresses) category = "Mattresses";
        }

        if (!string.IsNullOrEmpty(category))
        {
            // The memory status is updated every time it is clicked.
            _currentCategoryKeyword = category;

            UpdateTabStyles(category);

            // Update the text of the title on the right side.
            CurrentCategoryTitle.Text = category switch
            {
                "Pillows" => "Cozy Pillows",
                "Quilts" => "Warm Quilts",
                "BeddingSets" => "Premium Bedding Sets",
                "Mattresses" => "Supportive Mattresses",
                _ => "Our Collection"
            };

            await LoadCategoryDataAsync(category);
        }
    }

    private async Task LoadCategoryDataAsync(string keyword)
    {
        var allProducts = await _databaseService.GetAllProductsAsync();
        FilteredProducts.Clear();

        foreach (var product in allProducts)
        {
            if (product.Category != null && product.Category.Contains(keyword))
            {
                FilteredProducts.Add(product);
            }
        }

        EmptyStateLabel.IsVisible = FilteredProducts.Count == 0;
        CategoryCollectionView.IsVisible = FilteredProducts.Count > 0;
    }

    private async void OnProductSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Product selectedProduct)
        {
            await Navigation.PushAsync(new ProductDetailPage(selectedProduct));
            // Clear the selected state to prevent a grayish residual background from appearing when returning.
            CategoryCollectionView.SelectedItem = null;
        }
    }

    private void UpdateTabStyles(string selectedCategory)
    {
        // Reset the status of all buttons
        BtnPillows.BackgroundColor = Colors.Transparent; LblPillows.TextColor = Colors.Gray; LblPillows.FontAttributes = FontAttributes.None;
        BtnQuilts.BackgroundColor = Colors.Transparent; LblQuilts.TextColor = Colors.Gray; LblQuilts.FontAttributes = FontAttributes.None;
        BtnBeddingSets.BackgroundColor = Colors.Transparent; LblBeddingSets.TextColor = Colors.Gray; LblBeddingSets.FontAttributes = FontAttributes.None;
        BtnMattresses.BackgroundColor = Colors.Transparent; LblMattresses.TextColor = Colors.Gray; LblMattresses.FontAttributes = FontAttributes.None;

        // Light up the currently selected button
        switch (selectedCategory)
        {
            case "Pillows":
                BtnPillows.BackgroundColor = Colors.White; LblPillows.TextColor = Color.FromArgb("#1A2980"); LblPillows.FontAttributes = FontAttributes.Bold;
                break;
            case "Quilts":
                BtnQuilts.BackgroundColor = Colors.White; LblQuilts.TextColor = Color.FromArgb("#1A2980"); LblQuilts.FontAttributes = FontAttributes.Bold;
                break;
            case "BeddingSets":
                BtnBeddingSets.BackgroundColor = Colors.White; LblBeddingSets.TextColor = Color.FromArgb("#1A2980"); LblBeddingSets.FontAttributes = FontAttributes.Bold;
                break;
            case "Mattresses":
                BtnMattresses.BackgroundColor = Colors.White; LblMattresses.TextColor = Color.FromArgb("#1A2980"); LblMattresses.FontAttributes = FontAttributes.Bold;
                break;
        }
    }
}