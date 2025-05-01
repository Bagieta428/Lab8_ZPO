using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Lab8_ZPO
{
    /// <summary>
    /// Logika interakcji dla klasy PiWindow.xaml
    /// </summary>
    public partial class PiWindow : Window
    {
        public PiWindow()
        {
            InitializeComponent();
        }

        private static readonly Regex _regex = new Regex("^[0-9]{0,9}$");

        // tylko cyfry
        private void UserInputTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !_regex.IsMatch(e.Text);
        }

        // nie można wpisać spacji
        private void UserInputTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        // nie można wkleić blędnych symboli
        private void UserInputTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                var text = (String)e.DataObject.GetData(typeof(String));
                if (!_regex.IsMatch(text))
                {
                    e.CancelCommand();
                }
            }
        }

        // znikanie placeholder/hint tekstu jeśli w textbox jest wpisana wartość
        private void UserInputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(userInputTextBox.Text))
            {
                placeholderTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                placeholderTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void calculatePiButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(userInputTextBox.Text, out int digits) && digits > 0)
            {
                var progressBarWindow = new ProgressBarWindow(digits)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                
                bool? result = progressBarWindow.ShowDialog();

                if (result == true)
                {
                    openTextFileButton.IsEnabled = true;

                    var elapsed = progressBarWindow.TotalElapsedTime;
                    elapsedTimeTextBlock.Text = $"Czas obliczeń: {Math.Round(elapsed.TotalSeconds, 2)} s";
                    elapsedTimeTextBlock.Visibility = Visibility.Visible;

                    _lastSavedFilePath = progressBarWindow.LastSavedFilePath;
                }
            }
            else
            {
                MessageBox.Show("Wprowadź poprawną liczbę całkowitą większą od zera.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string _lastSavedFilePath;
        private void openTextFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(_lastSavedFilePath) && System.IO.File.Exists(_lastSavedFilePath))
                {
                    System.Diagnostics.Process.Start("notepad.exe", _lastSavedFilePath);
                }
                else
                {
                    MessageBox.Show("Nie znaleziono zapisanego pliku", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas otwierania pliku: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}