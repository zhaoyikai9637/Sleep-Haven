namespace SleepHaven;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
    }

    protected override void OnNavigated(ShellNavigatedEventArgs args)
    {
        base.OnNavigated(args);

        // reset the icon to "Not Selected"
        TabHome.Icon = "home_off.png";
        TabCategory.Icon = "category_off.png";
        TabCollection.Icon = "collect_off.png";

        // Obtain the content of the currently displayed page
        // Logical chain: Current Shell -> TabBar (CurrentItem) -> Current Tab (CurrentItem) -> Current Content (CurrentItem)
        var currentContent = CurrentItem?.CurrentItem?.CurrentItem;

        // Determine and highlight
        if (currentContent == TabHome)
        {
            TabHome.Icon = "home_on.png";
        }
        else if (currentContent == TabCategory)
        {
            TabCategory.Icon = "category_on.png";
        }
        else if (currentContent == TabCollection)
        {
            TabCollection.Icon = "collect_on.png";
        }
    }
}