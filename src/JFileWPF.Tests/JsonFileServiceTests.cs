using System.IO;
using com.jl.jfilewpf.Models;
using com.jl.jfilewpf.Services;
using Shouldly;
using Xunit;

namespace com.jl.jfilewpf.Tests;

public class JsonFileServiceTests : IDisposable
{
    private readonly string _tempDir =
        Path.Combine(Path.GetTempPath(), $"JFileWPFTests_{Guid.NewGuid():N}");

    public JsonFileServiceTests() => Directory.CreateDirectory(_tempDir);

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    private string TempFile(string name) => Path.Combine(_tempDir, name);

    [Fact]
    public void WriteToFile_Setup_RoundTripsCorrectly()
    {
        var setup = new Setup { Trim = true, ConvertTabToSpace = true, Space = 4, Override = true };

        JsonFileService.WriteToFile(TempFile("setup.json"), setup);

        JsonFileService.ReadFromFile<Setup>(TempFile("setup.json")).ShouldBeEquivalentTo(setup);
    }

    [Fact]
    public void WriteToFile_DefaultSetup_PreservesDefaults()
    {
        var setup = new Setup();

        JsonFileService.WriteToFile(TempFile("setup_defaults.json"), setup);

        JsonFileService.ReadFromFile<Setup>(TempFile("setup_defaults.json")).ShouldBeEquivalentTo(setup);
    }

    [Fact]
    public void WriteToFile_KeywordList_RoundTripsCorrectly()
    {
        var keywords = new List<Keyword>
        {
            new() { Key = "SELECT ", ConvertTo = "SELECT " },
            new() { Key = "FROM ",   ConvertTo = "FROM "   }
        };

        JsonFileService.WriteToFile(TempFile("keywords.json"), keywords);

        JsonFileService.ReadFromFile<List<Keyword>>(TempFile("keywords.json")).ShouldBeEquivalentTo(keywords);
    }

    [Fact]
    public void WriteToFile_EmptyList_RoundTripsCorrectly()
    {
        JsonFileService.WriteToFile(TempFile("empty.json"), new List<Keyword>());

        JsonFileService.ReadFromFile<List<Keyword>>(TempFile("empty.json")).ShouldBeEmpty();
    }

    [Fact]
    public void WriteToFile_OrderedList_PreservesOrder()
    {
        var keywords = new List<Keyword>
        {
            new() { Key = "A", ConvertTo = "1" },
            new() { Key = "B", ConvertTo = "2" },
            new() { Key = "C", ConvertTo = "3" }
        };

        JsonFileService.WriteToFile(TempFile("ordered.json"), keywords);

        JsonFileService.ReadFromFile<List<Keyword>>(TempFile("ordered.json")).ShouldBeEquivalentTo(keywords);
    }

    [Fact]
    public void ReadFromFile_NonexistentFile_ThrowsFileNotFoundException()
    {
        Should.Throw<FileNotFoundException>(() =>
            JsonFileService.ReadFromFile<Setup>(TempFile("nonexistent.json")));
    }

    [Fact]
    public void WriteToFile_NewPath_CreatesIndentedFile()
    {
        var path = TempFile("new.json");

        JsonFileService.WriteToFile(path, new Setup());
        var content = File.ReadAllText(path);

        File.Exists(path).ShouldBeTrue();
        content.ShouldContain("\n");
        content.ShouldContain("  ");
    }
}
