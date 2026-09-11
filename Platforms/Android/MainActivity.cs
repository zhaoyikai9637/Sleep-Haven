using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.View;

namespace SleepHaven;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Set the background of the status bar to pure white forcibly 
        if (Window != null)
        {
#pragma warning disable CA1422
            Window.SetStatusBarColor(Android.Graphics.Color.White);
#pragma warning restore CA1422

            if (Window.DecorView != null)
            {
                // obtain the Controller
                var controller = WindowCompat.GetInsetsController(Window, Window.DecorView);

                // Recheck to see if it is empty and ensure 100% security
                if (controller != null)
                {
                    controller.AppearanceLightStatusBars = true;
                }
            }
        }
    }
}