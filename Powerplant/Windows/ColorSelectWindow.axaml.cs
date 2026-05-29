using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Powerplant.Core;

namespace Powerplant.Windows;

public partial class ColorSelectWindow : Window
{
    private PwColor _color;
    private bool _disableEvents = true;
    public PwColor Color => _color;
    
    public ColorSelectWindow()
    {
        InitializeComponent();
        SetColor(PwColor.White);
    }

    public ColorSelectWindow(PwColor selectedColor) : this()
    {
        SetColor(selectedColor);
    }

    private void SetColor(PwColor color)
    {
        _color = color;
        Color avColor = color.ToColor();

        ColorCell.Background = new SolidColorBrush(avColor);

        _disableEvents = true;

        ColorSpinR.Value = (float)color.R / byte.MaxValue;
        ColorSpinR.Color = avColor;
        ColorTextR.Text = color.R.ToString();
        ColorSpinG.Value = (float)color.G / byte.MaxValue;
        ColorSpinG.Color = avColor;
        ColorTextG.Text = color.G.ToString();
        ColorSpinB.Value = (float)color.B / byte.MaxValue;
        ColorSpinB.Color = avColor;
        ColorTextB.Text = color.B.ToString();
        ColorSpinA.Value = (float)color.A / byte.MaxValue;
        ColorSpinA.Color = avColor;
        ColorTextA.Text = color.A.ToString();

        ColorSpinH.Color = avColor;
        ColorTextH.Text = ColorSpinH.Value.ToString("0");
        ColorSpinS.Color = avColor;
        ColorTextS.Text = ColorSpinS.Value.ToString("0");
        ColorSpinV.Color = avColor;
        ColorTextV.Text = ColorSpinV.Value.ToString("0");

        HexText.Text = (avColor.A < 255 ? avColor.A.ToString("X2") : "") + avColor.R.ToString("X2")
                                                                         + avColor.G.ToString("X2")
                                                                         + avColor.B.ToString("X2");

        _disableEvents = false;

        ColorWheel.Color = avColor;
    }

    private void ColorSpectrum_OnColorChanged(object? sender, ColorChangedEventArgs e)
    {
        if (_disableEvents) return;
        
        SetColor(new PwColor(e.NewColor));
    }

    private void ColorTextR_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_disableEvents) return;
        
        byte.TryParse(ColorTextR.Text, out _color.R);
        SetColor(_color);
    }

    private void ColorSpinR_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (_disableEvents) return;

        _color.R = (byte)e.NewValue;
        SetColor(_color);
    }

    private void ColorTextG_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_disableEvents) return;
        
        byte.TryParse(ColorTextG.Text, out _color.G);
        SetColor(_color);
    }

    private void ColorSpinG_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (_disableEvents) return;

        _color.G = (byte)e.NewValue;
        SetColor(_color);
    }

    private void ColorTextB_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_disableEvents) return;
        
        byte.TryParse(ColorTextB.Text, out _color.B);
        SetColor(_color);
    }

    private void ColorSpinB_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (_disableEvents) return;

        _color.B = (byte)e.NewValue;
        SetColor(_color);
    }

    private void ColorTextA_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_disableEvents) return;
        
        byte.TryParse(ColorTextA.Text, out _color.A);
        SetColor(_color);
    }

    private void ColorSpinA_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (_disableEvents) return;

        _color.A = (byte)e.NewValue;
        SetColor(_color);
    }

    private void ColorTextH_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        
    }

    private void ColorSpinH_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (_disableEvents) return;
        
        SetColor(new PwColor(ColorSpinH.Color));
    }

    private void ColorTextS_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        
    }

    private void ColorSpinS_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (_disableEvents) return;
        
        SetColor(new PwColor(ColorSpinS.Color));
    }

    private void ColorTextV_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        
    }

    private void ColorSpinV_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (_disableEvents) return;
        
        SetColor(new PwColor(ColorSpinV.Color));
    }

    private void HexText_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_disableEvents) return;

        string hex = HexText.Text!.TrimStart('#');
        if (hex.Length != 6 && hex.Length != 8) return;

        SetColor(new PwColor(hex));
    }

    private void SelectButtonClicked(object? sender, RoutedEventArgs e)
    {
        Close(_color);
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }
}