using com.jl.jfilewpf.Models;
using com.jl.jfilewpf.Services;
using Microsoft.Win32;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace com.jl.jfilewpf.Windows;

public partial class MainWindow : Window
{
    private const int DefaultTabSpace = 3;
    private readonly FileProcessingService _processor = new();
    private int _tabSpace = DefaultTabSpace;

    public MainWindow()
    {
        InitializeComponent();
        VersionText.Text = $"v{GetVersion()}";
        InitLayout();
    }

    private static string GetVersion() =>
        Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion?.Split('+')[0] ?? "dev";

    private void InitLayout()
    {
        try
        {
            var setup = JsonFileService.ReadFromFile<Setup>("setup.json");
            Cb_Trim.IsChecked = setup.Trim;
            Cb_Tab2Spaces.IsChecked = setup.ConvertTabToSpace;
            Cb_Keyword2Upper.IsChecked = setup.ConvertKeywordToUppercase;
            Cb_Override.IsChecked = setup.Override;

            if (setup.Space > 0)
            {
                Cb_Tab2Spaces.Content = $"Convert TAB to {setup.Space} spaces";
                _tabSpace = setup.Space;
            }
        }
        catch (FileNotFoundException)
        {
            JsonFileService.WriteToFile("setup.json", new Setup());
        }
        catch (IOException ex)
        {
            MessageBox.Show(ex.Message, "Access Denial", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
            DragMove();
    }

    private static void ShowError(string message) =>
        MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var menu = button.ContextMenu;
        if (menu is null) return;
        menu.PlacementTarget = button;
        menu.Placement = PlacementMode.Bottom;
        menu.IsOpen = true;
    }

    private void Close_Click(object sender, RoutedEventArgs e) =>
        Application.Current.Shutdown();

    private void Select_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var openFile = new OpenFileDialog
            {
                Filter = "SQL files (*.sql)|*.sql|Text files (*.txt)|*.txt"
            };

            if (openFile.ShowDialog() == true)
            {
                var filePath = openFile.FileName;
                var allText = File.ReadAllText(filePath);

                var options = new ProcessingOptions(
                    ConvertKeywords: Cb_Keyword2Upper.IsChecked == true,
                    ConvertTabsToSpaces: Cb_Tab2Spaces.IsChecked == true,
                    TabSpaceCount: _tabSpace,
                    TrimTrailingWhitespace: Cb_Trim.IsChecked == true,
                    CreateSeparateOutputFile: Cb_Override.IsChecked == true
                );

                List<Keyword>? keywords = null;
                if (options.ConvertKeywords)
                {
                    try
                    {
                        keywords = JsonFileService.ReadFromFile<List<Keyword>>("keywords.json");
                    }
                    catch (FileNotFoundException)
                    {
                        keywords = Keyword.CreateDefaults();
                        JsonFileService.WriteToFile("keywords.json", keywords);
                    }
                }

                var processedLines = _processor.ProcessText(allText, options, keywords);
                var outputPath = _processor.GetOutputFilePath(filePath, options.CreateSeparateOutputFile);

                File.WriteAllLines(outputPath, processedLines);
            }
        }
        catch (Exception ex) { ShowError(ex.Message); }
    }

    private void KeywordList_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!File.Exists("keywords.json"))
                JsonFileService.WriteToFile("keywords.json", Keyword.CreateDefaults());

            System.Diagnostics.Process.Start("notepad.exe", "keywords.json");
        }
        catch (Exception ex) { ShowError(ex.Message); }
    }

    private void Setup_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            new SettingWindow().ShowDialog();
            InitLayout();
        }
        catch (Exception ex) { ShowError(ex.Message); }
    }
}
