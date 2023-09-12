using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace FifthTask
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

        private void reportFileNameBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Save Report",
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                reportFileNameTextBox.Text = saveFileDialog.FileName;
            }
        }
        private void sourceDirectoryBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Source Directory",
                CheckFileExists = false,
                FileName = "Folder Selection",
                Filter = "Folders|*.thisdoesnotexist",
                ValidateNames = false,
                DereferenceLinks = true,
            };

            if (dialog.ShowDialog() == true)
            {
                sourceDirectoryTextBox.Text = System.IO.Path.GetDirectoryName(dialog.FileName);
            }
        }

        private void destinationDirectoryBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Destination Directory",
                CheckFileExists = false,
                FileName = "Folder Selection",
                Filter = "Folders|*.thisdoesnotexist",
                ValidateNames = false,
                DereferenceLinks = true,
            };

            if (dialog.ShowDialog() == true)
            {
                destinationDirectoryTextBox.Text = System.IO.Path.GetDirectoryName(dialog.FileName);
            }
        }
        private async void generateReportButton_Click(object sender, RoutedEventArgs e)
        {
            string sourceDirectory = sourceDirectoryTextBox.Text;
            string destinationDirectory = destinationDirectoryTextBox.Text;
            string reportFileName = reportFileNameTextBox.Text;

            if (string.IsNullOrEmpty(sourceDirectory) || string.IsNullOrEmpty(destinationDirectory) || string.IsNullOrEmpty(reportFileName))
            {
                MessageBox.Show("Please enter source directory, destination directory, and report file name.");
                return;
            }

            try
            {
                List<string> duplicateFiles = await FindDuplicateFilesAsync(sourceDirectory);

                await MoveOriginalFilesAndGenerateReportAsync(sourceDirectory, destinationDirectory, duplicateFiles, reportFileName);

                MessageBox.Show("Duplicate files have been moved to the destination directory, and the report has been generated.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private async Task MoveOriginalFilesAndGenerateReportAsync(string sourceDirectory, string destinationDirectory, List<string> duplicateFiles, string reportFileName)
        {
            using (StreamWriter writer = new StreamWriter(reportFileName))
            {
                foreach (string filePath in duplicateFiles)
                {
                    string fileName = System.IO.Path.GetFileName(filePath);
                    string destinationPath = System.IO.Path.Combine(destinationDirectory, fileName);

                    File.Move(filePath, destinationPath);

                    await writer.WriteLineAsync($"Moved: {filePath} -> {destinationPath}");
                }
            }
        }
        private async void findDuplicatesButton_Click(object sender, RoutedEventArgs e)
        {
            string sourceDirectory = sourceDirectoryTextBox.Text;
            string destinationDirectory = destinationDirectoryTextBox.Text;

            if (string.IsNullOrEmpty(sourceDirectory) || string.IsNullOrEmpty(destinationDirectory))
            {
                MessageBox.Show("Please enter source and destination directories.");
                return;
            }

            try
            {
                List<string> duplicateFiles = await FindDuplicateFilesAsync(sourceDirectory);

                await MoveOriginalFilesAsync(sourceDirectory, destinationDirectory, duplicateFiles);

                MessageBox.Show("Duplicate files have been moved to the destination directory.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private Task<List<string>> FindDuplicateFilesAsync(string sourceDirectory)
        {
            return Task.Run(() =>
            {
                var duplicateFiles = new List<string>();
                var fileHashes = new Dictionary<string, string>();

                string[] allFiles = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);

                foreach (string filePath in allFiles)
                {
                    string fileHash = CalculateFileHash(filePath);

                    if (fileHashes.ContainsKey(fileHash))
                    {
                        duplicateFiles.Add(filePath);
                    }
                    else
                    {
                        fileHashes[fileHash] = filePath;
                    }
                }

                return duplicateFiles;
            });
        }
        private Task MoveOriginalFilesAsync(string sourceDirectory, string destinationDirectory, List<string> duplicateFiles)
        {
            return Task.Run(() =>
            {
                foreach (string filePath in duplicateFiles)
                {
                    string fileName = System.IO.Path.GetFileName(filePath);
                    string destinationPath = System.IO.Path.Combine(destinationDirectory, fileName);

                    File.Move(filePath, destinationPath);
                }
            });
        }

        private string CalculateFileHash(string filePath)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

    }
}
