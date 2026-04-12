namespace com.jl.jfilewpf.Services;

public record ProcessingOptions(
    bool ConvertKeywords = false,
    bool ConvertTabsToSpaces = false,
    int TabSpaceCount = 3,
    bool TrimTrailingWhitespace = false,
    bool CreateSeparateOutputFile = false
);
