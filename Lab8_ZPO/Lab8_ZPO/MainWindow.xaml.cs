using System;
using System.Windows;

using System.ComponentModel;

namespace Lab8_ZPO
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // bidning
        private string _inputText;
        private string _outputText;
        public string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value;
                OnPropertyChanged(nameof(InputText));
            }
        }

        public string OutputText
        {
            get => _outputText;
            set
            {
                _outputText = value;
                OnPropertyChanged(nameof(OutputText));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            InputText = string.Empty;
            OutputText = string.Empty;
        }

        // okno służące do obliczania liczby pi
        // nie modalne ponieważ nie chcemy blokować głównego okna
        private void calculatePiButton_Click(object sender, RoutedEventArgs e)
        {
            var piWindow = new PiWindow();
            piWindow.Owner = this;
            piWindow.Show();
        }

        private void sinButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "sin(";
            OutputText = string.Empty;
        }

        private void cosButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "cos(";
            OutputText = string.Empty;
        }

        private void tanButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "tan(";
            OutputText = string.Empty;
        }

        private void rootButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "sqrt(";
            OutputText = string.Empty;
        }

        private void divideXXButton_Click(object sender, RoutedEventArgs e)
        {
            // no clue how to implement this
            OutputText = string.Empty;
        }

        private void piButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "π";
            OutputText = string.Empty;
        }

        private void exponentiationButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "^";
            OutputText = string.Empty;
        }

        private void logarithmButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "log(";
            OutputText = string.Empty;
        }

        private void zeroButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "0";
            OutputText = string.Empty;
        }

        private void oneButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "1";
            OutputText = string.Empty;
        }

        private void twoButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "2";
            OutputText = string.Empty;
        }

        private void threeButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "3";
            OutputText = string.Empty;
        }

        private void fourButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "4";
            OutputText = string.Empty;
        }

        private void fiveButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "5";
            OutputText = string.Empty;
        }

        private void sixButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "6";
            OutputText = string.Empty;
        }

        private void sevenButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "7";
            OutputText = string.Empty;
        }

        private void eightButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "8";
            OutputText = string.Empty;
        }

        private void nineButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "9";
            OutputText = string.Empty;
        }

        private void commaButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += ",";
            OutputText = string.Empty;
        }

        private void addMinusButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "(-";
            OutputText = string.Empty;
        }

        private void parenthesisLeftButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "(";
            OutputText = string.Empty;
        }

        private void parenthesisRightButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += ")";
            OutputText = string.Empty;
        }

        private void divideButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "/";
            OutputText = string.Empty;
        }

        private void multiplicationButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "*";
            OutputText = string.Empty;
        }

        private void minusButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "-";
            OutputText = string.Empty;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "+";
            OutputText = string.Empty;
        }

        private void clearAllButton_Click(object sender, RoutedEventArgs e)
        {
            InputText = string.Empty;
            OutputText = string.Empty;
        }

        // usuwanie jednego elementu na raz z końca
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            InputText = InputText.Length > 0 ? InputText.Substring(0, InputText.Length - 1) : string.Empty;
            OutputText = string.Empty;
        }

        private void calculateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tokenizer = new Tokenizer();
                var tokens = tokenizer.Tokenize(InputText);

                var parser = new Parser(tokens);
                var expression = parser.ParseExpression();

                var result = expression.Evaluate();
                OutputText = result.ToString();
            }
            catch (Exception)
            {
                OutputText = ("Błąd składni");
            }
        }
    }
}