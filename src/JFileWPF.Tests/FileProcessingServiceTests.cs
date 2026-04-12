using com.jl.jfilewpf.Models;
using com.jl.jfilewpf.Services;
using Shouldly;
using Xunit;

namespace com.jl.jfilewpf.Tests;

public class FileProcessingServiceTests
{
    private readonly FileProcessingService _sut = new();

    // ── ApplyKeywordConversions ──────────────────────────────────────────────

    [Theory]
    [InlineData("select * from users")]
    [InlineData("sElEcT * from users")]
    [InlineData("SELECT * from users")]
    public void ApplyKeywordConversions_CaseInsensitiveInput_ConvertsToUppercase(string input)
    {
        var keywords = new List<Keyword> { new() { Key = "SELECT ", ConvertTo = "SELECT " } };

        _sut.ApplyKeywordConversions(input, keywords).ShouldStartWith("SELECT ");
    }

    [Fact]
    public void ApplyKeywordConversions_WithEmptyList_ReturnsOriginal()
    {
        _sut.ApplyKeywordConversions("select * from users", []).ShouldBe("select * from users");
    }

    [Fact]
    public void ApplyKeywordConversions_NonKeywordText_IsUnchanged()
    {
        var keywords = new List<Keyword> { new() { Key = "SELECT ", ConvertTo = "SELECT " } };

        _sut.ApplyKeywordConversions("hello world", keywords).ShouldBe("hello world");
    }

    [Fact]
    public void ApplyKeywordConversions_MultipleKeywords_AllAreConverted()
    {
        var keywords = new List<Keyword>
        {
            new() { Key = "SELECT ", ConvertTo = "SELECT " },
            new() { Key = "FROM ",   ConvertTo = "FROM "   }
        };

        _sut.ApplyKeywordConversions("select name from users", keywords).ShouldBe("SELECT name FROM users");
    }

    // ── Comment protection ───────────────────────────────────────────────────

    [Theory]
    [InlineData("-- select all users",      "-- select all users")]
    [InlineData("/* select from backup */", "/* select from backup */")]
    [InlineData("select * -- select all",   "SELECT * -- select all")]
    public void ApplyKeywordConversions_CommentedText_IsNotConverted(string input, string expected)
    {
        var keywords = new List<Keyword> { new() { Key = @"\bSELECT\b", ConvertTo = "SELECT" } };

        _sut.ApplyKeywordConversions(input, keywords).ShouldBe(expected);
    }

    [Fact]
    public void ApplyKeywordConversions_MultilineBlockComment_IsNotConverted()
    {
        var keywords = new List<Keyword> { new() { Key = @"\bSELECT\b", ConvertTo = "SELECT" } };
        var text = $"/*{Environment.NewLine}  select all{Environment.NewLine}*/";

        _sut.ApplyKeywordConversions(text, keywords).ShouldBe(text);
    }

    [Fact]
    public void ApplyKeywordConversions_MixedCodeAndComments_OnlyCodeIsConverted()
    {
        var keywords = new List<Keyword>
        {
            new() { Key = @"\bSELECT\b", ConvertTo = "SELECT" },
            new() { Key = @"\bFROM\b",   ConvertTo = "FROM"   }
        };
        var text = $"-- get users{Environment.NewLine}select * from users";

        _sut.ApplyKeywordConversions(text, keywords).ShouldBe($"-- get users{Environment.NewLine}SELECT * FROM users");
    }

    // ── ConvertTabsToSpaces ──────────────────────────────────────────────────

    [Theory]
    [InlineData("\tHello",    3, "   Hello")]
    [InlineData("\t\tHello",  4, "        Hello")]
    [InlineData("Hello World",3, "Hello World")]
    [InlineData("\tHello",    0, "Hello")]
    public void ConvertTabsToSpaces_TabCharacters_AreReplacedWithSpaces(string input, int spaces, string expected)
    {
        _sut.ConvertTabsToSpaces(input, spaces).ShouldBe(expected);
    }

    // ── TrimLines ────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("Hello   ",   "Hello")]
    [InlineData("   Hello   ","   Hello")]
    [InlineData("   ",        "")]
    [InlineData("Hello",      "Hello")]
    public void TrimLines_TrailingWhitespace_IsRemoved(string input, string expected)
    {
        _sut.TrimLines([input]).ShouldHaveSingleItem().ShouldBe(expected);
    }

    // ── GetOutputFilePath ────────────────────────────────────────────────────

    [Fact]
    public void GetOutputFilePath_NotSeparateFile_ReturnsOriginal()
    {
        _sut.GetOutputFilePath(@"C:\dir\query.sql", false).ShouldBe(@"C:\dir\query.sql");
    }

    [Theory]
    [InlineData(@"C:\dir\query.sql",     @"C:\dir\query_JFileWPF.sql")]
    [InlineData(@"C:\dir\data.txt",      @"C:\dir\data_JFileWPF.txt")]
    [InlineData(@"C:\dir\README",        @"C:\dir\README_JFileWPF")]
    [InlineData(@"C:\a\b\c\file.sql",   @"C:\a\b\c\file_JFileWPF.sql")]
    public void GetOutputFilePath_SeparateFile_AppendsJFileWPFSuffix(string input, string expected)
    {
        _sut.GetOutputFilePath(input, true).ShouldBe(expected);
    }

    // ── ProcessText (integration) ────────────────────────────────────────────

    [Fact]
    public void ProcessText_AllOptionsDisabled_ReturnsOriginalLines()
    {
        var text = $"line1{Environment.NewLine}line2";

        var result = _sut.ProcessText(text, new ProcessingOptions()).ToList();

        result.Count.ShouldBe(2);
        result[0].ShouldBe("line1");
        result[1].ShouldBe("line2");
    }

    [Fact]
    public void ProcessText_AllEnabled_AppliesAllTransformations()
    {
        var keywords = new List<Keyword> { new() { Key = "SELECT ", ConvertTo = "SELECT " } };
        var options = new ProcessingOptions(
            ConvertKeywords: true,
            ConvertTabsToSpaces: true,
            TabSpaceCount: 2,
            TrimTrailingWhitespace: true);
        var text = $"\tselect * from users   {Environment.NewLine}\tline2   ";

        var result = _sut.ProcessText(text, options, keywords).ToList();

        result[0].ShouldBe("  SELECT * from users");
        result[1].ShouldBe("  line2");
    }

    [Fact]
    public void ProcessText_NullKeywords_SkipsConversion()
    {
        var options = new ProcessingOptions(ConvertKeywords: true);

        _sut.ProcessText("select * from users", options, keywords: null)
            .ShouldHaveSingleItem().ShouldBe("select * from users");
    }
}
