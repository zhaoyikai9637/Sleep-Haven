namespace SleepHaven;

public partial class LoadingPage : ContentPage
{
    public LoadingPage() => InitializeComponent();

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(3000);

        if (Application.Current?.Windows.FirstOrDefault() is { } window)
        {
            window.Page = new AppShell();
        }
    }
}
