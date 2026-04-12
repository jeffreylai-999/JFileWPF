using JFile.Models;
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
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        private const string tabSpace = "3";
        public SettingWindow()
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
                    Tb_Space.Text = setup.Space.ToString() ?? tabSpace;
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

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int space = int.TryParse(Tb_Space.Text, out space) ? space : int.Parse(tabSpace);

                var setup = new List<Setup>
                {
                    new Setup() 
                    {
                        Trim = Cb_Trim.IsChecked, 
                        ConvertTabToSpace = Cb_Tab2Spaces.IsChecked, 
                        ConvertKeywordToUppercase = Cb_Keyword2Upper.IsChecked, 
                        Override = Cb_Override.IsChecked, 
                        Space = space
                    }
                };
                XmlExtension.WriteToXmlFile("Setup.xml", setup);

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
