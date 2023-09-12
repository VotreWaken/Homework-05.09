using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;

namespace FourthTask
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
                sourceDirectoryTextBox.Text = Path.GetDirectoryName(dialog.FileName);
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
                destinationDirectoryTextBox.Text = Path.GetDirectoryName(dialog.FileName);
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
                    string fileName = Path.GetFileName(filePath);
                    string destinationPath = Path.Combine(destinationDirectory, fileName);

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
