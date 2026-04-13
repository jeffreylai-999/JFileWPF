# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

JFileWPF is a Windows desktop WPF application (.NET 10, C# latest) that processes `.sql` and `.txt` files with text transformation features: SQL keyword uppercasing, tab-to-space conversion, trailing whitespace trimming, and optional output file override.

**Namespace:** `com.jl.jfilewpf`

## Build Commands

```bash
# Restore + Debug build
dotnet build JFileWPF.sln -c Debug

# Release build
dotnet build JFileWPF.sln -c Release

# Run the app
dotnet run --project src/JFileWPF/JFileWPF.csproj

# Run all tests
dotnet test src/JFileWPF.Tests/

# Run a single test by name
dotnet test src/JFileWPF.Tests/ --filter "FullyQualifiedName~MethodName"

# Single-file publish
dotnet publish src/JFileWPF/JFileWPF.csproj -c Release -r win-x64 --self-contained false
```

Output artifacts:

- Debug: `src/JFileWPF/bin/Debug/net10.0-windows/JFileWPF.exe`
- Release: `src/JFileWPF/bin/Release/net10.0-windows/JFileWPF.exe`

## Architecture

### Project Structure

```text
src/
  JFileWPF/          — Main WPF application
  JFileWPF.Tests/    — xUnit + Shouldly test project
```

### Layers

- **UI** — `Windows/MainWindow.xaml/.cs`, `Windows/SettingWindow.xaml/.cs`: file selection, option display, user interaction
- **Services** — `Services/FileProcessingService.cs`, `Services/JsonFileService.cs`: text transformation pipeline, JSON persistence
- **Models** — `Models/Setup.cs`, `Models/Keyword.cs`, `Services/ProcessingOptions.cs`: configuration state, keyword mappings, transformation options

`FileProcessingService` contains the extracted text processing pipeline. Window code-behind delegates to this service for all transformations. `JsonFileService` handles all JSON file persistence for both `Setup` and `List<Keyword>`.

### Runtime Data Files

Created in the working directory on first run:

- `setup.json` — serialized `Setup` object
- `keywords.json` — seeded from `Keyword.CreateDefaults()` (13 core SQL keywords) on first use

### Processing Pipeline

When a file is selected, transformations are applied in this order:

1. **Keyword conversion** — `CommentPattern()` (source-generated regex) splits text into alternating code/comment segments; keywords are replaced only in code segments, leaving `--` and `/* */` comments untouched
2. **Tab to spaces** — replaces `\t` with configurable number of spaces (default: 3)
3. **Trim** — removes trailing whitespace per line
4. **Output** — writes to `{name}_JFileWPF{ext}` unless Override=true (replaces original)

### Key Implementation Details

- `Setup.Override` is inverted relative to the UI label: the "Do not override" checkbox maps to `CreateSeparateOutputFile=true` in `ProcessingOptions`
- `ProcessingOptions` is a positional record with all-`false` defaults — constructed explicitly in `MainWindow.Select_Click`
- `SettingWindow` uses `[GeneratedRegex]` for numeric input validation; `FileProcessingService` uses `[GeneratedRegex]` for the comment splitter — both classes must be declared `partial`
- `Keyword.CreateDefaults()` is the single source of truth for the default keyword list; called both in `Select_Click` (silent seed) and `KeywordList_Click` (seed then open in Notepad)
- Material Design v5 is bootstrapped via `BundledTheme` in `App.xaml` (theme: Light, Primary=Blue, Secondary=Indigo); use `ElevationAssist.Elevation` not the removed `ShadowAssist.ShadowDepth`
- The title bar uses a custom `TitleBarButton` style (defined in `App.xaml` `Application.Resources`, shared by both windows) — a plain `ControlTemplate` with `#33FFFFFF` hover and `#55FFFFFF` pressed states; the Settings button uses `Button` + `Button.ContextMenu`, not `PopupBox`
- File content is fully read into memory — not suitable for very large files

## Testing Conventions

### Test Naming

All test methods must follow the pattern:

```text
[MethodUnderTest]_[Scenario]_[ExpectedResult]
```

Examples:

- `ApplyKeywordConversions_CommentedText_IsNotConverted`
- `GetOutputFilePath_SeparateFile_AppendsJFileWPFSuffix`
- `ProcessText_NullKeywords_SkipsConversion`

Use `[Theory]` + `[InlineData]` to collapse multiple cases that share the same method, scenario type, and assertion shape into a single test. Keep `[Fact]` for cases that require multiline strings or complex setup that cannot be expressed as inline literals.

## Workflow Conventions

### Git & GitHub

- **Never add Claude as a co-author** in commit messages.
- Prefer `gh` (GitHub CLI) for all GitHub operations (PRs, comments, reviews). Fall back to `git` only when no `gh` command covers the task.

### PR Reviews

When asked to review a PR:

1. Fetch **only unresolved comments** — skip anything already resolved.
2. For each unresolved comment, state whether it needs fixing and why.
3. **Wait for user confirmation** on which suggestions to apply before making any changes.
4. After applying the confirmed fixes:
   - Commit and push the changes.
   - Post a PR comment listing what was fixed and why each dismissed comment was not actioned.
   - Resolve each conversation thread via `gh api`.
