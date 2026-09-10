using SleepHaven;
using System;
using System.Collections.Generic;
using System.Text;

namespace SleepHaven;

public partial class LoadingPage : ContentPage
{
    public LoadingPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(3000);

        if (Application.Current != null)
        {
            // Attempt to obtain the main window of the current application
            // Usually, there is only one window on mobile devices, so accessing Windows[0] is safe
            // Robustness: First, check if there is anything in the Windows list
            if (Application.Current.Windows.Count > 0)
            {
                var currentWindow = Application.Current.Windows[0];

                // Modify the page of the window
                currentWindow.Page = new AppShell();
            }
        }
    }
}