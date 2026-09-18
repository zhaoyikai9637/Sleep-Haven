namespace SleepHaven;

public partial class LoadingPage : ContentPage
{
    private readonly AppShell _appShell;

    public LoadingPage(AppShell appShell)
    {
        InitializeComponent();
        _appShell = appShell;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(3000);

        if (Application.Current?.Windows.FirstOrDefault() is { } window)
        {
            window.Page = _appShell;
        }
    }
}
