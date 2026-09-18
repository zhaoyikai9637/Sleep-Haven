namespace SleepHaven;

public partial class AppShell : Shell
{
    public AppShell(HomePage homePage, CategoryPage categoryPage, CollectionPage collectionPage)
    {
        InitializeComponent();
        TabHome.Content = homePage;
        TabCategory.Content = categoryPage;
        TabCollection.Content = collectionPage;
    }
}
