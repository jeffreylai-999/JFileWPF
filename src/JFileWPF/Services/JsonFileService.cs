using System.IO;
using System.Text.Json;

namespace com.jl.jfilewpf.Services;

internal static class JsonFileService
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    public static void WriteToFile<T>(string filePath, T data)
    {
        var json = JsonSerializer.Serialize(data, Options);
        File.WriteAllText(filePath, json);
    }

    public static T ReadFromFile<T>(string filePath)
    {
        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<T>(json, Options)
            ?? throw new InvalidOperationException($"Failed to deserialize {filePath}");
    }
}
