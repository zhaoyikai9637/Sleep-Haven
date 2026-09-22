using Microsoft.Extensions.Logging;

namespace SleepHaven;

public sealed class MotionPreferences
{
#if WINDOWS
    private readonly ILogger<MotionPreferences> _logger;
#endif

    public MotionPreferences(ILogger<MotionPreferences> logger)
    {
#if WINDOWS
        _logger = logger;
#endif
    }

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
                _logger.LogWarning(exception, "Windows animation preferences could not be read.");
                return false;
            }
#else
            return false;
#endif
        }
    }
}
