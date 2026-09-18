using Microsoft.Extensions.DependencyInjection;

namespace SleepHaven;

public partial class App : Application
{
    private readonly LoadingPage _loadingPage;

    public App(LoadingPage loadingPage)
    {
        InitializeComponent();
        _loadingPage = loadingPage;
    }

    // Rewrite the CreateWindow method to define the startup window
    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Create a new window containing the LoadingPage and return it to the system
        return new Window(_loadingPage);
    }
}
