namespace CollectorApp.Helpers;

public static class AppLogger
{
    private static readonly string LogPath = Path.Combine(
        FileSystem.Current.AppDataDirectory,
        "app.log");

    private const string Tag = "COLLECTOR";

    public static void Debug(string message) => Log("DBG", message);
    public static void Info(string message) => Log("INF", message);
    public static void Warning(string message) => Log("WRN", message);
    public static void Error(string message, Exception? ex = null)
    {
        Log("ERR", message);
        if (ex is not null)
        {
            Log("ERR", $"Exception: {ex.GetType().Name}: {ex.Message}");
            if (ex.InnerException is not null)
                Log("ERR", $"Inner: {ex.InnerException.Message}");
            Log("ERR", $"Stack: {ex.StackTrace}");
        }
    }

    public static void Log(string message) => Info(message);

    private static void Log(string level, string message)
    {
        var line = $"[{level}] {DateTime.Now:HH:mm:ss.fff} | {message}";

#if ANDROID
        switch (level)
        {
            case "DBG": Android.Util.Log.Debug(Tag, message); break;
            case "INF": Android.Util.Log.Info(Tag, message); break;
            case "WRN": Android.Util.Log.Warn(Tag, message); break;
            case "ERR": Android.Util.Log.Error(Tag, message); break;
            default: Android.Util.Log.Info(Tag, message); break;
        }
#endif

#if DEBUG
        System.Diagnostics.Debug.WriteLine(line);
        try
        {
            File.AppendAllText(LogPath, line + Environment.NewLine);
        }
        catch { }
#endif
    }

    public static string GetLogPath() => LogPath;
}