using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WpfApp25
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

        // Display Information Click
        private void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            // Call Methods

            MessageBox.Show($"Number Of Offers: { CheckNumbersOfOffers(TBInput.Text).Result }");

            MessageBox.Show($"Number Of Symbols: { CheckNumbersOfSymbols(TBInput.Text).Result }");

            MessageBox.Show($"Number Of Words: { CheckNumbersOfWords(TBInput.Text).Result }");

            MessageBox.Show($"Interrogative Sentences: { CountInterrogativeSentences(TBInput.Text).Result }");

            MessageBox.Show($"Exclamation Sentences: { CountExclamationSentences(TBInput.Text).Result }");
        }

        // Save Information Click
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (StreamWriter writer = new StreamWriter("File.txt"))
            {
                writer.WriteLine($"Number Of Offers: {CheckNumbersOfOffers(TBInput.Text).Result } ");
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
    }
}
