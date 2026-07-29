using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Powerplant.Core;

namespace Powerplant.Windows;

public partial class OpenRecentWindow : Window
{
    public OpenRecentWindow()
    {
        InitializeComponent();

        RecentFilesListBox.ItemsSource = RecentFilesManager.Files
            .Where(File.Exists)
            .Select(file => new RecentFile(file));
        
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        SearchTextBox.Focus();
    }

    private void OpenButtonClicked(object? sender, RoutedEventArgs e)
    {
        
    }

    private void CancelButtonClicked(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }

    private void RecentFilesListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0) return;

        RecentFile item = (RecentFile?)e.AddedItems[0];
        Close(item.FullPath);
    }

    public class RecentFile
    {
        public string FullPath { get; }
        public string Filename => Path.GetFileName(FullPath);
        public Bitmap? Thumbnail => GetThumbnail();

        public RecentFile(string fullPath)
        {
            FullPath = fullPath;
        }

        private Bitmap? GetThumbnail()
        {
            try
            {
                return new Bitmap(FullPath);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }

    private void SearchTextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
        {
            RecentFilesListBox.ItemsSource = RecentFilesManager.Files.Select(file => new RecentFile(file));
            return;
        }
        
        RecentFilesListBox.ItemsSource = RecentFilesManager.Files
            .Where(file => file.Contains(SearchTextBox.Text.Trim(), StringComparison.InvariantCultureIgnoreCase))
            .Select(file => new RecentFile(file));
    }
}