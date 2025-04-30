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

namespace Lab8_ZPO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void calculatePiButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void sinButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text += "sin(";
        }

        private void cosButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text += "cos(";
        }

        private void tanButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text += "tan(";
        }

        private void rootButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text += "sqrt(";
        }

        private void divideXXButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void piButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text += "π";
        }

        private void exponentiationButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text += "^";
        }

        private void logarithmButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text += "log(";
        }

        private void zeroButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text += "0";
        }

        private void oneButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text += "1";
        }

        private void twoButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text += "2";
        }

        private void threeButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text += "3";
        }

        private void fourButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text += "4";
        }

        private void fiveButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text += "5";
        }

        private void sixButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text += "6";
        }

        private void sevenButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text += "7";
        }

        private void eightButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text += "8";
        }

        private void nineButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text += "9";
        }

        private void commaButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text += ",";
        }

        private void addMinusButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text += "(-";
        }

        private void parenthesisLeftButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text += "(";
        }

        private void parenthesisRightButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text += ")";
        }

        private void divideButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text += "%";
        }

        private void multiplicationButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text += "*";
        }

        private void minusButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text += "-";
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text += "+";
        }

        private void clearAllButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text = string.Empty;
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBlock.Text = inputTextBlock.Text.Length > 0 ? inputTextBlock.Text.Substring(0, inputTextBlock.Text.Length - 1) : string.Empty;
        }

        private void calculateButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}