using com.jl.jfilewpf.Models;
using com.jl.jfilewpf.Services;
using Shouldly;
using Xunit;

namespace com.jl.jfilewpf.Tests;

public class ModelDefaultTests
{
    [Fact]
    public void Setup_DefaultTrim_IsFalse() =>
        new Setup().Trim.ShouldBeFalse();

    [Fact]
    public void Setup_DefaultConvertTabToSpace_IsFalse() =>
        new Setup().ConvertTabToSpace.ShouldBeFalse();

    [Fact]
    public void Setup_DefaultSpace_IsThree() =>
        new Setup().Space.ShouldBe(3);

    [Fact]
    public void Setup_DefaultConvertKeywordToUppercase_IsFalse() =>
        new Setup().ConvertKeywordToUppercase.ShouldBeFalse();

    [Fact]
    public void Setup_DefaultOverride_IsFalse() =>
        new Setup().Override.ShouldBeFalse();

    [Fact]
    public void Keyword_DefaultKey_IsEmpty() =>
        new Keyword().Key.ShouldBe(string.Empty);

    [Fact]
    public void Keyword_DefaultConvertTo_IsEmpty() =>
        new Keyword().ConvertTo.ShouldBe(string.Empty);

    [Fact]
    public void ProcessingOptions_Defaults_AllFalseAndThreeSpaces()
    {
        var options = new ProcessingOptions();

        options.ConvertKeywords.ShouldBeFalse();
        options.ConvertTabsToSpaces.ShouldBeFalse();
        options.TabSpaceCount.ShouldBe(3);
        options.TrimTrailingWhitespace.ShouldBeFalse();
        options.CreateSeparateOutputFile.ShouldBeFalse();
    }
}
