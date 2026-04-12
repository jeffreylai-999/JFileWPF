using System.Text.Json.Serialization;

namespace com.jl.jfilewpf.Models;

public class Setup
{
    [JsonPropertyName("trim")]
    public bool Trim { get; set; }

    [JsonPropertyName("convertTabToSpace")]
    public bool ConvertTabToSpace { get; set; }

    [JsonPropertyName("space")]
    public int Space { get; set; } = 3;

    [JsonPropertyName("convertKeywordToUppercase")]
    public bool ConvertKeywordToUppercase { get; set; }

    [JsonPropertyName("override")]
    public bool Override { get; set; }
}
