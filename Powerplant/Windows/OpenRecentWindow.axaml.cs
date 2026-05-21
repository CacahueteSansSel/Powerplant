using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Powerplant.Core;

namespace Powerplant.Windows;

public partial class OpenRecentWindow : Window
{
    public OpenRecentWindow()
    {
        InitializeComponent();

        foreach (string file in RecentFilesManager.Files)
            RecentFilesListBox.Items.Add(file);
    }

    private void OpenButtonClicked(object? sender, RoutedEventArgs e)
    {
        
    }

    private void CancelButtonClicked(object? sender, RoutedEventArgs e)
    {
        
    }

    private void RecentFilesListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0) return;

        string item = (string)e.AddedItems[0];
        Close(item);
    }
}