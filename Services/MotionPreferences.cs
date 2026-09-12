namespace SleepHaven;

public static class MotionPreferences
{
    public static bool AreAnimationsEnabled
    {
        get
        {
#if WINDOWS
            try
            {
                return new Windows.UI.ViewManagement.UISettings().AnimationsEnabled;
            }
            catch
            {
                return false;
            }
#else
            return false;
#endif
        }
    }
}
