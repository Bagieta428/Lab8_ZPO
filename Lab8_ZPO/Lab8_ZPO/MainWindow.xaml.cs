using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.ComponentModel;

namespace Lab8_ZPO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
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
        }

        private void calculatePiButton_Click(object sender, RoutedEventArgs e)
        {
            var piWindow = new PiWindow();
            piWindow.Owner = this;
            piWindow.Show();
        }

        private void sinButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "sin(";
        }

        private void cosButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "cos(";
        }

        private void tanButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "tan(";
        }

        private void rootButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "sqrt(";
        }

        private void divideXXButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void piButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "π";
        }

        private void exponentiationButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "^";
        }

        private void logarithmButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "log(";
        }

        private void zeroButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "0";
        }

        private void oneButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "1";
        }

        private void twoButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "2";
        }

        private void threeButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "3";
        }

        private void fourButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "4";
        }

        private void fiveButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "5";
        }

        private void sixButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "6";
        }

        private void sevenButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "7";
        }

        private void eightButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "8";
        }

        private void nineButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "9";
        }

        private void commaButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += ",";
        }

        private void addMinusButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "(-";
        }

        private void parenthesisLeftButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "(";
        }

        private void parenthesisRightButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += ")";
        }

        private void divideButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "/";
        }

        private void multiplicationButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "*";
        }

        private void minusButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "-";
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            InputText += "+";
        }

        private void clearAllButton_Click(object sender, RoutedEventArgs e)
        {
            InputText = string.Empty;
            OutputText = string.Empty;
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            InputText = InputText.Length > 0 ? InputText.Substring(0, InputText.Length - 1) : string.Empty;
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
            catch (Exception ex)
            {
                InputText = ("Błąd składni");
            }
        }
    }
}