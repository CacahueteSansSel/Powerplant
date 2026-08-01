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

    private void TextureSizePresetButton16_OnClick(object? sender, RoutedEventArgs e)
    {
        Close(new Vector2(16, 16));
    }

    private void TextureSizePresetButton32_OnClick(object? sender, RoutedEventArgs e)
    {
        Close(new Vector2(32, 32));
    }

    private void TextureSizePresetButton64_OnClick(object? sender, RoutedEventArgs e)
    {
        Close(new Vector2(64, 64));
    }

    private void TextureSizePresetButton128_OnClick_OnClick(object? sender, RoutedEventArgs e)
    {
        Close(new Vector2(128, 128));
    }

    private void TextureSizePresetButton512x340_OnClick(object? sender, RoutedEventArgs e)
    {
        Close(new Vector2(512, 340));
    }

    private void TextureSizePresetButton256x170_OnClick(object? sender, RoutedEventArgs e)
    {
        Close(new Vector2(256, 170));
    }
}