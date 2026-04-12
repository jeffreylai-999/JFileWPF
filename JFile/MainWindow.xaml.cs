using JFile.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace JFile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static int tabSpace = 3;
        public MainWindow()
        {
            InitializeComponent();
            InitLayout();
        }

        private void InitLayout()
        {
            try
            {
                var setup = XmlExtension.ReadFromXmlFile<Setup>("Setup.xml").ElementAt(0);
                Cb_Trim.IsChecked = setup.Trim ?? false;
                Cb_Tab2Spaces.IsChecked = setup.ConvertTabToSpace ?? false;
                Cb_Keyword2Upper.IsChecked = setup.ConvertKeywordToUppercase ?? false;
                Cb_Override.IsChecked = setup.Override ?? false;

                if (setup.Space > 0)
                {
                    Cb_Tab2Spaces.Content = $"Convert TAB to {setup.Space} spaces";
                    tabSpace = setup.Space ?? 3;
                }
            }
            catch (FileNotFoundException)
            {
                var setup = new List<Setup>
                {
                    new Setup()
                };
                XmlExtension.WriteToXmlFile("Setup.xml", setup);
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "Access Denial", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFile = new OpenFileDialog()
                {
                    Filter = "SQL files (*.sql)|*.sql|Text files (*.txt)|*.txt"
                };

                if (openFile.ShowDialog() == true)
                {
                    var filePath = openFile.FileName;
                    var allText = File.ReadAllText(filePath);
                    IEnumerable<string> lines;

                    if (Cb_Keyword2Upper.IsChecked == true)
                    {
                        var keywords = XmlExtension.ReadFromXmlFile<Keyword>("Keywords.xml");
                        foreach (var keyword in keywords)
                        {
                            allText = Regex.Replace(allText, keyword.Key, keyword.ConvertTo, RegexOptions.IgnoreCase);
                        }
                    }

                    if (Cb_Tab2Spaces.IsChecked == true)
                    {
                        allText = allText.Replace("\t", new string(' ', tabSpace));
                    }

                    lines = allText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                    if (Cb_Trim.IsChecked == true)
                    {
                        lines = lines.Select(txt => txt.TrimEnd());
                    }

                    if (Cb_Override.IsChecked == true)
                    {
                        filePath = Path.GetDirectoryName(filePath) + @"\" + Path.GetFileNameWithoutExtension(filePath) + "_JFile" + Path.GetExtension(filePath);
                    }

                    File.WriteAllLines(filePath, lines);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void KeywordList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("notepad.exe", "Keywords.xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Setup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //System.Diagnostics.Process.Start("notepad.exe", "Setup.xml");
                var window = new SettingWindow();
                window.ShowDialog();
                InitLayout();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
