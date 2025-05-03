using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using PeterO.Numbers;

namespace Lab8_ZPO
{
    /// <summary>
    /// Logika interakcji dla klasy ProgressBarWindow.xaml
    /// </summary>
    public partial class ProgressBarWindow : Window
    {
        private CancellationTokenSource _cts;
        private bool _isPaused = false;

        private string _lastSavedFilePath;
        public string LastSavedFilePath => _lastSavedFilePath;

        public int DigitsToCalculate { get; set; }
        public TimeSpan TotalElapsedTime { get; private set; }

        public ProgressBarWindow(int digitsToCalculate)
        {
            InitializeComponent();
            DigitsToCalculate = digitsToCalculate;
            Loaded += ProgressBarWindow_Loaded;
        }

        private async void ProgressBarWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _cts = new CancellationTokenSource();
            await CalculatePiAsync(DigitsToCalculate, _cts.Token);
        }

        private async Task CalculatePiAsync(int digits, CancellationToken cancellationToken)
        {
            int terms = digits * 10;
            EDecimal sum = EDecimal.Zero; //0.0
            object lockObj = new object();
            int calculatedDigits = 0;
            Stopwatch stopwatch = Stopwatch.StartNew();

            int lastReported = 0; // zmienna do przechowywania ostatnio zgłoszonej wartości

            await Task.Run(() =>
            {
                EContext context = EContext.ForPrecision(digits + 5).WithRounding(ERounding.HalfEven); // context ustawia precyzje + kilka cyfr; WithRounding ustawia zaokrąglanie

                Parallel.For(0, terms, new ParallelOptions { CancellationToken = cancellationToken }, () => EDecimal.Zero, //0.0
                    (i, loopState, local) =>
                    {
                        if (Volatile.Read(ref _isPaused))
                        {
                            stopwatch.Stop();
                            while (Volatile.Read(ref _isPaused))
                            {
                                Thread.Sleep(50);
                                if (cancellationToken.IsCancellationRequested)
                                {
                                    loopState.Stop();
                                    return local;
                                }
                            }
                            stopwatch.Start();
                        }

                        // PRZED ZAMIANĄ NA EDECIMAL: double term = 1.0 / (2 * i + 1);
                        EDecimal denominator = EDecimal.FromInt32(2 * i + 1); // konwersja int -> edecimal
                        EDecimal term = EDecimal.One.Divide(denominator, context);
                        if (i % 2 != 0)
                            term = term.Negate();

                        local = local.Add(term);
                        Interlocked.Increment(ref calculatedDigits);

                        // UI UPDATE
                        int current = Interlocked.Increment(ref calculatedDigits);
                        if (current - lastReported >= terms/100)
                        {
                            lastReported = current;

                            Dispatcher.Invoke(() =>
                            {
                                double effectiveProgress = Math.Min(1.0, (double)calculatedDigits / terms); // terms = digits * 10;
                                amountOfNumbersCalculated.Text = Math.Min(digits, calculatedDigits / 10).ToString();
                                piCalculatrionProgressBar.Value = effectiveProgress * 100;

                                var elapsed = stopwatch.Elapsed.TotalSeconds;
                                var estTotal = elapsed / effectiveProgress;
                                var estRemaining = Math.Max(0, estTotal - elapsed);
                                remainingTimeTextBlock.Text = $"{Math.Round(estRemaining, 2)} s";
                            });
                        }

                        return local;
                    },
                    local =>
                    {
                        lock (lockObj)
                        {
                            // sum += local;
                            sum = sum.Add(local);
                        }
                    });
            });

            // PRZED ZAMIANĄ NA EDECIMAL: double pi = 4 * sum;
            EDecimal pi = sum.Multiply(EDecimal.FromInt32(4));
            stopwatch.Stop();
            // wartość przekazywana do okna PiWindow
            TotalElapsedTime = stopwatch.Elapsed;

            /*###########################*/
            /* ZAPISYWANIE PLIKU DO .txt */
            /*###########################*/

            // PRZED ZMIANĄ NA EDECIMAL: var piDigits = pi.ToString("F" + digits);
            // zapisywanie dokładnej wartości pi
            EContext context = EContext.ForPrecision(digits).WithRounding(ERounding.HalfEven);
            EDecimal roundedPi = pi.RoundToPrecision(context);
            string piDigits = pi.ToString();

            // dzielenie obliczonej wartości na "chunki" po 150 znaków
            string[] parts = piDigits.Split('.');
            string integerPart = parts[0];
            string fractionalPart = parts.Length > 1 ? parts[1] : string.Empty;

            StringBuilder formattedPi = new StringBuilder();
            formattedPi.AppendLine($"Obliczona liczba Pi z dokładnością {digits} cyfr dziesiętnych:");
            formattedPi.AppendLine(integerPart + ".");

            for (int i = 0; i < fractionalPart.Length; i += 150)
            {
                int chunkLenght = Math.Min(150, fractionalPart.Length - i);
                formattedPi.AppendLine(fractionalPart.Substring(i, chunkLenght));
            }

            string folder = AppDomain.CurrentDomain.BaseDirectory;
            string baseName = "obliczanie_pi_";
            int fileIndex = 1;
            string filePath;

            do
            {
                filePath = System.IO.Path.Combine(folder, $"{baseName}{fileIndex}.txt");
                fileIndex++;
            }
            while (System.IO.File.Exists(filePath));

            try
            {
                System.IO.File.WriteAllText(filePath, formattedPi.ToString());
                _lastSavedFilePath = filePath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas zapisu pliku: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            MessageBox.Show("Obliczanie zakończone pomyślnie", "Gotowe", MessageBoxButton.OK, MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }

        private void stopCalculatingPiButton_Click(object sender, RoutedEventArgs e)
        {
            _isPaused = true;

            stopCalculatingPiButton.Visibility = Visibility.Collapsed;
            continueCalculatingPiButton.Visibility = Visibility.Visible;
            endCalculatingPiNowButton.Visibility = Visibility.Visible;
        }

        private void continueCalculatingPiButton_Click(object sender, RoutedEventArgs e)
        {
            _isPaused = false;

            continueCalculatingPiButton.Visibility = Visibility.Collapsed;
            endCalculatingPiNowButton.Visibility = Visibility.Collapsed;
            stopCalculatingPiButton.Visibility = Visibility.Visible;
        }

        private void endCalculatingPiNowButton_Click(object sender, RoutedEventArgs e)
        {
            _cts.Cancel();
        }

        private void cancelCalculatingPiButton_Click(object sender, RoutedEventArgs e)
        {
            _cts.Cancel();
            DialogResult = false;
            this.Close();
        }
    }
}