using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Powerplant.Windows;

public partial class ResizeViewportWindow : Window
{
    private ResizeAnchor _anchor = ResizeAnchor.TopLeft;

    public ResizeViewportWindow()
    {
        InitializeComponent();
        
        DataContext = new Data(16, 16);
        UpdateAnchorButtons();
    }
    
    public ResizeViewportWindow(int width, int height)
    {
        InitializeComponent();

        DataContext = new Data(width, height);
        UpdateAnchorButtons();
    }

    private void CancelButtonClicked(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }

    private void CreateTextureButtonClicked(object? sender, RoutedEventArgs e)
    {
        Data data = (Data)DataContext!;
        
        Close(new ResizeViewportResult(_anchor, data.Width, data.Height));
    }

    private void PlacementButtonClicked(object? sender, RoutedEventArgs e)
    {
        if (e.Source is not ToggleButton tb) return;
        
        switch (tb.Name)
        {
            case "BtnTL":
                _anchor = ResizeAnchor.TopLeft;
                break;
            case "BtnTM":
                _anchor = ResizeAnchor.TopMiddle;
                break;
            case "BtnTR":
                _anchor = ResizeAnchor.TopRight;
                break;
            case "BtnML":
                _anchor = ResizeAnchor.MiddleLeft;
                break;
            case "BtnMM":
                _anchor = ResizeAnchor.Center;
                break;
            case "BtnMR":
                _anchor = ResizeAnchor.MiddleRight;
                break;
            case "BtnBL":
                _anchor = ResizeAnchor.BottomLeft;
                break;
            case "BtnBM":
                _anchor = ResizeAnchor.BottomMiddle;
                break;
            case "BtnBR":
                _anchor = ResizeAnchor.BottomRight;
                break;
        }

        UpdateAnchorButtons();
    }

    private void UpdateAnchorButtons()
    {
        BtnTL.IsChecked = _anchor == ResizeAnchor.TopLeft;
        BtnTM.IsChecked = _anchor == ResizeAnchor.TopMiddle;
        BtnTR.IsChecked = _anchor == ResizeAnchor.TopRight;
        BtnML.IsChecked = _anchor == ResizeAnchor.MiddleLeft;
        BtnMM.IsChecked = _anchor == ResizeAnchor.Center;
        BtnMR.IsChecked = _anchor == ResizeAnchor.MiddleRight;
        BtnBL.IsChecked = _anchor == ResizeAnchor.BottomLeft;
        BtnBM.IsChecked = _anchor == ResizeAnchor.BottomMiddle;
        BtnBR.IsChecked = _anchor == ResizeAnchor.BottomRight;
    }
    
    public record Data(int Width, int Height);
}

public record ResizeViewportResult(ResizeAnchor Anchor, int Width, int Height);

public enum ResizeAnchor
{
    TopLeft,
    TopMiddle,
    TopRight,
    MiddleLeft,
    Center,
    MiddleRight,
    BottomLeft,
    BottomMiddle,
    BottomRight
}