using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lab8_ZPO
{
    public partial class PiWindow : Window, INotifyPropertyChanged
    {
        // binding
        private string _inputText;

        public string UserInput
        {
            get => _inputText;
            set
            {
                _inputText = value;
                OnPropertyChanged(nameof(UserInput));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public PiWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        // ten regex pozwala na wpisanie tylko cyfr od 0 do 9
        private static readonly Regex _regex = new Regex("^[0-9]{0,9}$");

        // możliwe wpisanie tylko cyfry
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
            if (string.IsNullOrEmpty(UserInput))
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
            // wyciąganie wartości z textboxa i przekazywanie wartości do okna ProgressBarWindow
            if (int.TryParse(UserInput, out int digits) && digits > 0)
            {
                var progressBarWindow = new ProgressBarWindow(digits)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                
                bool? result = progressBarWindow.ShowDialog();

                // enable przycisku otwierania pliku oraz wyświetlenie upłyniętego czasu w tym oknie
                if (result == true)
                {
                    openTextFileButton.IsEnabled = true;

                    var elapsed = progressBarWindow.TotalElapsedTime;
                    elapsedTimeTextBlock.Text = $"Czas obliczeń: {Math.Round(elapsed.TotalSeconds, 2)} s"; // z dokładnością do 2 miejsc po przecinku
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