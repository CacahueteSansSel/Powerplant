using System.Numerics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Powerplant.Windows;

public partial class NewTextureWindow : Window
{
    public NewTextureWindow()
    {
        InitializeComponent();

        DataContext = new Data(16, 16);
    }

    private void CancelButtonClicked(object? sender, RoutedEventArgs e)
    {
        Close(new Vector2(0, 0));
    }

    private void CreateTextureButtonClicked(object? sender, RoutedEventArgs e)
    {
        Data data = (Data)DataContext!;
        
        Close(new Vector2(data.Width, data.Height));
    }

    public record Data(int Width, int Height);

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        WidthBox.Focus();
    }
}