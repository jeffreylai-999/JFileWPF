# JFileWPF

A Windows desktop application for formatting `.sql` and `.txt` files. Built with WPF on .NET 10.

## Features

- **SQL keyword uppercasing** — converts keywords like `select`, `from`, `where` to uppercase; comment-aware (`--` and `/* */` blocks are left untouched)
- **Tab to spaces** — replaces tabs with a configurable number of spaces (default: 3)
- **Trim trailing whitespace** — removes trailing spaces/tabs from every line
- **Output control** — writes to a new `{name}_JFileWPF{ext}` file by default, or overwrites the original

## Requirements

- Windows
- [.NET 10 Runtime](https://dotnet.microsoft.com/download/dotnet/10.0)

## Usage

1. Launch `JFileWPF.exe`
2. Check the transformations you want applied
3. Click **SELECT** and pick a `.sql` or `.txt` file
4. The processed file is saved alongside the original as `{name}_JFileWPF{ext}`

### Settings

Click the **Settings** icon (top right) to access:

- **Keywords List** — opens `keywords.json` in Notepad; edit or add entries to customise which keywords get uppercased. Each entry has a `key` (regex pattern) and a `convertTo` value.
- **Configuration** — opens the settings dialog to change defaults (tab size, which transformations are on by default, output behaviour)

### keywords.json

Created automatically on first use with 13 core SQL keywords. The `key` field is a regex pattern — word boundaries (`\b`) ensure partial matches like `INSERT` are not affected by the `IN` rule.

Example entry:

```json
{ "key": "\\bSELECT\\b", "convertTo": "SELECT" }
```

## Building from Source

```bash
dotnet build JFileWPF.sln -c Release
dotnet test src/JFileWPF.Tests/
dotnet publish src/JFileWPF/JFileWPF.csproj -c Release -r win-x64 --self-contained false
```
