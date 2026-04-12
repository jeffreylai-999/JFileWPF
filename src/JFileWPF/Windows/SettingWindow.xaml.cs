using System.IO;
using com.jl.jfilewpf.Models;
using com.jl.jfilewpf.Services;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace com.jl.jfilewpf.Windows;

public partial class SettingWindow : Window
{
    private const int DefaultTabSpace = 3;

    [GeneratedRegex("[^0-9]+")]
    private static partial Regex NumericValidationRegex();

    public SettingWindow()
    {
        InitializeComponent();
        InitLayout();
    }

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
                Tb_Space.Text = setup.Space.ToString();
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

    private void Close_Click(object sender, RoutedEventArgs e) => Close();

    private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
            DragMove();
    }

    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) =>
        e.Handled = NumericValidationRegex().IsMatch(e.Text);

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var space = int.TryParse(Tb_Space.Text, out var parsed) ? parsed : DefaultTabSpace;

            var setup = new Setup
            {
                Trim = Cb_Trim.IsChecked == true,
                ConvertTabToSpace = Cb_Tab2Spaces.IsChecked == true,
                ConvertKeywordToUppercase = Cb_Keyword2Upper.IsChecked == true,
                Override = Cb_Override.IsChecked == true,
                Space = space
            };
            JsonFileService.WriteToFile("setup.json", setup);
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
