using System.IO;
using System.Text.RegularExpressions;
using com.jl.jfilewpf.Models;

namespace com.jl.jfilewpf.Services;

public partial class FileProcessingService
{
    // Captures both line comments (-- ...) and block comments (/* ... */)
    [GeneratedRegex(@"(--[^\r\n]*|/\*.*?\*/)", RegexOptions.Singleline)]
    private static partial Regex CommentPattern();

    public IEnumerable<string> ProcessText(string text, ProcessingOptions options, IReadOnlyList<Keyword>? keywords = null)
    {
        if (options.ConvertKeywords && keywords is not null)
            text = ApplyKeywordConversions(text, keywords);

        if (options.ConvertTabsToSpaces)
            text = ConvertTabsToSpaces(text, options.TabSpaceCount);

        IEnumerable<string> lines = text.Split(Environment.NewLine);

        if (options.TrimTrailingWhitespace)
            lines = TrimLines(lines);

        return lines;
    }

    public string ApplyKeywordConversions(string text, IReadOnlyList<Keyword> keywords)
    {
        // Split into alternating segments: even = code, odd = comment (preserved as-is)
        var segments = CommentPattern().Split(text);
        for (int i = 0; i < segments.Length; i += 2)
        {
            foreach (var keyword in keywords)
                segments[i] = Regex.Replace(segments[i], keyword.Key, keyword.ConvertTo, RegexOptions.IgnoreCase);
        }
        return string.Concat(segments);
    }

    public string ConvertTabsToSpaces(string text, int spaceCount) =>
        text.Replace("\t", new string(' ', spaceCount));

    public IEnumerable<string> TrimLines(IEnumerable<string> lines) =>
        lines.Select(line => line.TrimEnd());

    public string GetOutputFilePath(string originalFilePath, bool createSeparateFile)
    {
        if (!createSeparateFile)
            return originalFilePath;

        var directory = Path.GetDirectoryName(originalFilePath) ?? string.Empty;
        var name = Path.GetFileNameWithoutExtension(originalFilePath);
        var ext = Path.GetExtension(originalFilePath);
        return Path.Combine(directory, $"{name}_JFileWPF{ext}");
    }
}
