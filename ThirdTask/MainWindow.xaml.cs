using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
namespace ThirdTask
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CancellationTokenSource cancellationTokenSource;
        public SynchronizationContext uiContext;
        public MainWindow()
        {
            InitializeComponent();
            uiContext = SynchronizationContext.Current;
        }
        private bool CheckSymbols()
        {
            bool symbolsChecked = false;
            uiContext.Send(state =>
            {
                symbolsChecked = SymbolsCB.IsChecked == true;
            }, null);
            return symbolsChecked;
        }

        private bool CheckWords()
        {
            bool symbolsChecked = false;
            uiContext.Send(state =>
            {
                symbolsChecked = WordsCB.IsChecked == true;
            }, null);
            return symbolsChecked;
        }

        private bool CheckOffers()
        {
            bool symbolsChecked = false;
            uiContext.Send(state =>
            {
                symbolsChecked = OffersCB.IsChecked == true;
            }, null);
            return symbolsChecked;
        }

        private bool CheckExclamation()
        {
            bool symbolsChecked = false;
            uiContext.Send(state =>
            {
                symbolsChecked = ExclamationCB.IsChecked == true;
            }, null);
            return symbolsChecked;
        }

        private bool CheckInterrogative()
        {
            bool symbolsChecked = false;
            uiContext.Send(state =>
            {
                symbolsChecked = InterrogativeCB.IsChecked == true;
            }, null);
            return symbolsChecked;
        }
        private void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {

            cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() =>
            {
                try
                {
                    if (CheckOffers())
                    {
                        Task.Delay(2000).Wait();
                        uiContext.Send(d => MessageBox.Show($"Number Of Offers: {CheckNumbersOfOffers(TBInput.Text).Result}"), null);
                    }

                    if (CheckSymbols())
                    {
                        Task.Delay(2000).Wait();
                        uiContext.Send(d => MessageBox.Show($"Number Of Symbols: {CheckNumbersOfSymbols(TBInput.Text).Result}"), null);
                    }

                    if (CheckWords())
                    {
                        Task.Delay(2000).Wait();
                        uiContext.Send(d => MessageBox.Show($"Number Of Words: {CheckNumbersOfWords(TBInput.Text).Result}"), null);
                    }

                    if (CheckInterrogative())
                    {
                        Task.Delay(2000).Wait();
                        uiContext.Send(d => MessageBox.Show($"Interrogative Sentences: {CountInterrogativeSentences(TBInput.Text).Result}"), null);
                    }

                    if (CheckExclamation())
                    {
                        Task.Delay(2000).Wait();
                        uiContext.Send(d => MessageBox.Show($"Exclamation Sentences: {CountExclamationSentences(TBInput.Text).Result}"), null);
                    }

                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
                }
                catch (OperationCanceledException)
                {
                    // Обработка отмены задачи
                    MessageBox.Show("Canceled Operation.");
                }
            });

            /*
            Task.Run(() =>
            {
                Task.Delay(2000).Wait();
                // MessageBox.Show($"Number Of Offers: {CheckNumbersOfOffers(TBInput.Text).Result}");
                uiContext.Send(d => MessageBox.Show($"Number Of Offers: {CheckNumbersOfOffers(TBInput.Text).Result}"), null);
                // MessageBox.Show($"Number Of Symbols: {CheckNumbersOfSymbols(TBInput.Text).Result}");

                // MessageBox.Show($"Number Of Words: {CheckNumbersOfWords(TBInput.Text).Result}");

                // MessageBox.Show($"Interrogative Sentences: {CountInterrogativeSentences(TBInput.Text).Result}");

                // MessageBox.Show($"Exclamation Sentences: {CountExclamationSentences(TBInput.Text).Result}");
            });
            */
            // Call Methods
        }

        // Save Information Click
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (StreamWriter writer = new StreamWriter("File.txt"))
            {
                writer.WriteLine($"Number Of Offers: {CheckNumbersOfOffers(TBInput.Text).Result} ");
                writer.WriteLine($"Number Of Symbols: {CheckNumbersOfSymbols(TBInput.Text).Result} ");
                writer.WriteLine($"Number Of Words: {CheckNumbersOfWords(TBInput.Text).Result} ");
                writer.WriteLine($"Interrogative Sentences: {CountInterrogativeSentences(TBInput.Text).Result} ");
                writer.WriteLine($"Exclamation Sentences: {CountExclamationSentences(TBInput.Text).Result} ");
            }

        }

        // Number Of Offers
        private Task<int> CheckNumbersOfOffers(string inputText)
        {
            return Task.Run(() =>
            {
                // Split the input text by a separator (e.g., comma)
                string[] offers = inputText.Split('.');
                // Get the number of offers
                int numberOfOffers = offers.Length;

                // Return Result
                return numberOfOffers - 1;
            });
        }

        // Number Of Symbols
        private Task<int> CheckNumbersOfSymbols(string inputText)
        {

            return Task.Run(() =>
            {
                // Get the number of offers
                int numberOfOffers = inputText.Length;

                // Return Result
                return numberOfOffers;
            });
        }

        // Number Of Words
        private Task<int> CheckNumbersOfWords(string inputText)
        {
            return Task.Run(() =>
            {
                string[] words = inputText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Get the number of offers
                int numberOfOffers = words.Length;

                // Return Result
                return numberOfOffers;
            });
        }

        // Number of Interrogative Sentences
        private Task<int> CountInterrogativeSentences(string inputText)
        {
            return Task.Run(() =>
            {
                // Use a regular expression to match interrogative sentences
                // This pattern matches sentences that end with a question mark, regardless of punctuation inside
                string pattern = @"[^.!?]*[?]+[^.!?]*";
                MatchCollection matches = Regex.Matches(inputText, pattern);

                // Count the number of interrogative sentences
                int numberOfInterrogativeSentences = matches.Count;

                // Return Result
                return numberOfInterrogativeSentences;
            });
        }

        // Count Exclamation Sentences
        private Task<int> CountExclamationSentences(string inputText)
        {
            return Task.Run(() =>
            {
                // Use a regular expression to match sentences ending with exclamation marks
                string pattern = @"[^.!?]*[!]+[^.!?]*";
                MatchCollection matches = Regex.Matches(inputText, pattern);

                // Count the number of exclamation sentences
                int numberOfExclamationSentences = matches.Count;

                // Return Result
                return numberOfExclamationSentences;
            });
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource.Cancel();
        }
    }
}
