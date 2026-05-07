namespace CollectorApp.Helpers;

public static class AppLogger
{
    private static readonly string LogPath = Path.Combine(
        FileSystem.Current.AppDataDirectory,
        "app.log");

    public static void Log(string message)
    {
        var line = $"{DateTime.Now:HH:mm:ss.fff} | {message}";
        System.Diagnostics.Debug.WriteLine(line);

#if DEBUG
        try
        {
            File.AppendAllText(LogPath, line + Environment.NewLine);
        }
        catch { }
#endif
    }

    public static string GetLogPath() => LogPath;
}