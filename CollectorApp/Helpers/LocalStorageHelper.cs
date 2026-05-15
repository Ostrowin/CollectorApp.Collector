using System.Text.Json;

namespace CollectorApp.Helpers;

public static class LocalStorageHelper
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public static async Task SaveAsync<T>(string fileName, T data, string directory)
    {
        var filePath = Path.Combine(directory, fileName);
        var json = JsonSerializer.Serialize(data, Options);
        await File.WriteAllTextAsync(filePath, json);
    }

    public static async Task<T?> LoadAsync<T>(string fileName, string directory)
    {
        var filePath = Path.Combine(directory, fileName);
        if (!File.Exists(filePath))
            return default;
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<T>(json, Options);
    }

    public static bool Exists(string fileName, string directory)
    {
        var filePath = Path.Combine(directory, fileName);
        return File.Exists(filePath);
    }
}
