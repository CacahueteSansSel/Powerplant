using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;

namespace Powerplant.Windows;

public partial class ResizeImageWindow : Window
{
    public ResizeImageWindow()
    {
        InitializeComponent();
        
        DataContext = new Data(16, 16);
    }
    
    public ResizeImageWindow(int width, int height)
    {
        InitializeComponent();

        DataContext = new Data(width, height);
    }

    private void CancelButtonClicked(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }

    private void CreateTextureButtonClicked(object? sender, RoutedEventArgs e)
    {
        Data data = (Data)DataContext!;
        
        Close(new ResizeImageResult(data.Width, data.Height, BitmapInterpolationMode.None));
    }
    
    public record Data(int Width, int Height);
}

public record ResizeImageResult(int Width, int Height, BitmapInterpolationMode InterpolationMode);