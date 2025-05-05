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
            // im większy mnożnik digits tym większa dokładność wyniku
            int terms = digits * 10;
            // zmienna do przechowywania obliczonej wartości pi
            EDecimal sum = EDecimal.Zero; //0.0
            object lockObj = new object();
            int calculatedDigits = 0;
            Stopwatch stopwatch = Stopwatch.StartNew();

            int lastReported = 0; // zmienna do przechowywania ostatnio zgłoszonej wartości

            // używanie asynchronicznie dostępnych wątków bez blokowania głównego wątku
            await Task.Run(() =>
            {
                // context ForPrecision wymaga pondanej ilość liczb + dowolny zapas aby uniknąć błędów w zaokrąglaniu
                // HalfEven oznacza że jeśli liczba do zaokrąglenia jest równa 5 to zaokrągla w górę lub w doł w zależności od tego czy ostatnia liczba była parzysta czy nie
                // parzyta = w dół, nieparzysta = w górę
                EContext context = EContext.ForPrecision(digits + 5).WithRounding(ERounding.HalfEven);

                Parallel.For(0, terms, new ParallelOptions { CancellationToken = cancellationToken }, () => EDecimal.Zero, //0.0
                    (i, loopState, local) =>
                    {
                        if (Volatile.Read(ref _isPaused))
                        {
                            // pauzowanie stopwatcha kiedy wątek jest wstrzymany aby nie zawyżał czasu
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

                        /* Algorytm korzysta z szeregu Gregory'ego Leibniza */
                        /*                                                  */
                        /*            /      1     1     1     1        \   */
                        /*    π = 4 * | 1 - --- + --- - --- + --- - ... |   */
                        /*            \      3     5     7     9        /   */
                        /*                                                  */
                        /*  Dokładniejszy wzór znajduje się w specyfikacji  */

                        // PRZED ZAMIANĄ NA EDECIMAL: double term = 1.0 / (2 * i + 1);
                        EDecimal denominator = EDecimal.FromInt32(2 * i + 1); // 1, 3, 5, 7...
                        EDecimal term = EDecimal.One.Divide(denominator, context); // 1 / (2 * n + 1)
                        if (i % 2 != 0)
                            term = term.Negate(); // -1, +1, -1, +1...

                        // dodawnie do sumy lokalnej
                        local = local.Add(term);
                        Interlocked.Increment(ref calculatedDigits);

                        // update ui: upłynięty czas oraz progressbar
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

                        // zwraca obliczoną liczbę pi
                        return local;
                    },
                    local =>
                    {
                        lock (lockObj)
                        {
                            // dodanie obliczonej wartości do zmiennej sum
                            sum = sum.Add(local);
                        }
                    });
            });

            // aby było zgodnie ze wzorem z szeregu Leibniza wynik należy pomnożyć razy 4
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
                int chunkLength = Math.Min(150, fractionalPart.Length - i);
                formattedPi.AppendLine(fractionalPart.Substring(i, chunkLength));
            }

            // utworzony plik będzie się znajdował w folderze projektu /bin/Debug/net8.0-windows
            string folder = AppDomain.CurrentDomain.BaseDirectory;
            string baseName = "obliczanie_pi_";
            int fileIndex = 1;
            string filePath;

            do
            {
                // ścierzka + nazwa + index który jest generowany o jeden index wyżej jeśli plik istnieje
                // np. obliczanie_pi_1.txt, obliczanie_pi_2.txt itd.
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

        // zapauzowanie obliczeń sprawia że przycisk "wtrzymaj" znika i pojawiają się dla użytkownika dostępne inne przyciski
        private void stopCalculatingPiButton_Click(object sender, RoutedEventArgs e)
        {
            _isPaused = true;

            stopCalculatingPiButton.Visibility = Visibility.Collapsed;
            continueCalculatingPiButton.Visibility = Visibility.Visible;
            endCalculatingPiNowButton.Visibility = Visibility.Visible;
        }

        // i wicewersa
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