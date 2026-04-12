using System.Text.Json.Serialization;

namespace com.jl.jfilewpf.Models;

public class Keyword
{
    [JsonPropertyName("key")] public string Key { get; set; } = string.Empty;

    [JsonPropertyName("convertTo")] public string ConvertTo { get; set; } = string.Empty;

    public static List<Keyword> CreateDefaults() =>
    [
        new() { Key = @"\bSELECT\b", ConvertTo = "SELECT" },
        new() { Key = @"\bFROM\b", ConvertTo = "FROM" },
        new() { Key = @"\bWHERE\b", ConvertTo = "WHERE" },
        new() { Key = @"\bAND\b", ConvertTo = "AND" },
        new() { Key = @"\bOR\b", ConvertTo = "OR" },
        new() { Key = @"\bNOT\b", ConvertTo = "NOT" },
        new() { Key = @"\bIN\b", ConvertTo = "IN" },
        new() { Key = @"\bBETWEEN\b", ConvertTo = "BETWEEN" },
        new() { Key = @"\bLIKE\b", ConvertTo = "LIKE" },
        new() { Key = @"\bDISTINCT\b", ConvertTo = "DISTINCT" },
        new() { Key = @"\bORDER\s+BY\b", ConvertTo = "ORDER BY" },
        new() { Key = @"\bGROUP\s+BY\b", ConvertTo = "GROUP BY" },
        new() { Key = @"\bHAVING\b", ConvertTo = "HAVING" },
    ];
}
