namespace SleepHaven;

public partial class LoadingPage : ContentPage
{
    public LoadingPage() => InitializeComponent();

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (MotionPreferences.AreAnimationsEnabled)
        {
            LoadingStatus.Opacity = 0.35;
            await LoadingStatus.FadeToAsync(1, 420, Easing.CubicOut);
        }

        await Task.Delay(700);

        if (Application.Current?.Windows.FirstOrDefault() is { } window)
        {
            window.Page = new AppShell();
        }
    }
}
