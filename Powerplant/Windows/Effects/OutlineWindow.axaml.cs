using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Powerplant.Controls;
using Powerplant.Core;
using Powerplant.Core.Effects;

namespace Powerplant.Windows.Effects;

public partial class OutlineWindow : Window
{
    private EffectRunner<OutlineEffect> _runner;
    private bool _keepEffect;
    private ViewportControl _viewport;

    public OutlineWindow()
    {
        InitializeComponent();
    }
    
    public OutlineWindow(ViewportControl viewport) : this()
    {
        _viewport = viewport;
        _runner = new EffectRunner<OutlineEffect>(viewport);
        _runner.Apply();

        StyleFourNeighbors.IsChecked = true;
        StyleEightNeighbors.IsChecked = false;
        SmoothSwitch.IsChecked = false;
        
        Closed += OnClosed;
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        if (_keepEffect) return;
        
        _runner.Reset();
    }

    private void ConfirmButtonClicked(object? sender, RoutedEventArgs e)
    {
        _keepEffect = true;
        
        _runner.Reset();
        _viewport.RunCommand(_runner.RunEffectCommand);
        
        Close();
    }

    private void CancelButtonClicked(object? sender, RoutedEventArgs e)
    {
        _keepEffect = false;
        Close();
    }

    private void StyleEightNeighbors_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (StyleEightNeighbors.IsChecked!.Value)
            StyleFourNeighbors.IsChecked = false;
        
        UpdateEffectPreview();
    }

    private void UpdateEffectPreview()
    {
        if (_runner == null) return;
        
        _runner.Effect.IsEightNeighbor = StyleEightNeighbors.IsChecked!.Value;
        _runner.Effect.IsSmooth = SmoothSwitch.IsChecked!.Value;
        _runner.Effect.Size = (int)OutlineSizeBox.Value!.Value;
        _runner.Effect.Color = new PwColor(ColorTextBox.Text ?? "000000");
        
        _runner.Apply();
    }

    private void StyleFourNeighbors_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (StyleFourNeighbors.IsChecked!.Value)
            StyleEightNeighbors.IsChecked = false;
        
        UpdateEffectPreview();
    }

    private void SmoothSwitch_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        UpdateEffectPreview();
    }

    private void OutlineSizeBox_OnValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        UpdateEffectPreview();
    }

    private void ColorTextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        UpdateEffectPreview();
    }

    private async void SelectColorButtonClicked(object? sender, RoutedEventArgs e)
    {
        PwColor? color = await new ColorSelectWindow(_runner.Effect.Color).ShowDialog<PwColor?>(this);
        if (!color.HasValue) return;

        ColorTextBox.Text = color.Value.ToHexString();
        UpdateEffectPreview();
    }
}