using Microsoft.Extensions.Logging;

namespace SleepHaven;

public sealed class MotionPreferences(ILogger<MotionPreferences> logger)
{
    public bool AreAnimationsEnabled
    {
        get
        {
#if WINDOWS
            try
            {
                return new Windows.UI.ViewManagement.UISettings().AnimationsEnabled;
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "Windows animation preferences could not be read.");
                return false;
            }
#else
            return false;
#endif
        }
    }
}
