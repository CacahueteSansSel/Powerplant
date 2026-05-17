using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Powerplant.Core.Tools;

namespace Powerplant.Controls.ToolsSettings;

public partial class DrawRectangleToolSettings : UserControl
{
    public RectangleTool Tool { get; set; }

    public DrawRectangleToolSettings()
    {
        InitializeComponent();
    }

    public DrawRectangleToolSettings(RectangleTool tool) : this()
    {
        Tool = tool;
        UpdateSettings(tool.Settings);
    }

    private void UpdateSettings(RectangleTool.ToolSettings toolSettings)
    {
        OutlineRectOptionButton.IsChecked = !toolSettings.IsFilled;
        FillRectOptionButton.IsChecked = toolSettings.IsFilled;
        StrokeSizeSpin.Value = toolSettings.Thickness;
    }

    private void OutlineRectOptionButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Tool.Settings.IsFilled = false;
        UpdateSettings(Tool.Settings);
    }

    private void FillRectOptionButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Tool.Settings.IsFilled = true;
        UpdateSettings(Tool.Settings);
    }

    private void StrokeSizeSpin_OnValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        Tool.Settings.Thickness = (int)(e.NewValue ?? 1);
        UpdateSettings(Tool.Settings);
    }
}