using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Powerplant.Core;
using Powerplant.Core.Commands;
using Powerplant.Core.Tools;
using Powerplant.FileFormats;
using Powerplant.Windows;
using ReactiveUI;

namespace Powerplant;

public partial class MainWindow : Window
{
    private bool _disableEvents = false;
    
    public MainWindow()
    {
        InitializeComponent();
        
        Viewport.OnPrimaryColorChanged += ViewportOnPrimaryColorChanged;
        Viewport.OnSecondaryColorChanged += ViewportOnSecondaryColorChanged;
        Viewport.OnBitmapChanged += ViewportOnBitmapChanged;
        
        SetTool(new PixelTool());
    }

    private void ViewportOnBitmapChanged(object? sender, ViewportBitmap e)
    {
        Title = $"{e.Width}x{e.Height} Texture - Powerplant";
    }

    private void ViewportOnSecondaryColorChanged(object? sender, PwColor e)
    {
        Color avColor = e.ToColor();
        
        SecondaryColorCell.Background = new SolidColorBrush(avColor);
    }

    private void ViewportOnPrimaryColorChanged(object? sender, PwColor e)
    {
        Color avColor = e.ToColor();
        
        PrimaryColorCell.Background = new SolidColorBrush(avColor);

        _disableEvents = true;
        
        ColorSpinR.Value = (float)e.R / byte.MaxValue;
        ColorSpinR.Color = avColor;
        ColorTextR.Text = e.R.ToString();
        ColorSpinG.Value = (float)e.G / byte.MaxValue;
        ColorSpinG.Color = avColor;
        ColorTextG.Text = e.G.ToString();
        ColorSpinB.Value = (float)e.B / byte.MaxValue;
        ColorSpinB.Color = avColor;
        ColorTextB.Text = e.B.ToString();
        ColorSpinA.Value = (float)e.A / byte.MaxValue;
        ColorSpinA.Color = avColor;
        ColorTextA.Text = e.A.ToString();

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

    private async void MenuNewTextureOptionClicked(object? sender, EventArgs e)
    {
        Vector2 size = await new NewTextureWindow().ShowDialog<Vector2>(this);
        if (size.X == 0 || size.Y == 0) return;
        
        Viewport.CreateTexture((int)size.X, (int)size.Y);
    }

    private async void MenuOpenTextureOptionClicked(object? sender, EventArgs e)
    {
        List<FilePickerFileType> fileTypes = FileFormatManager.BuildFilePickerFileList();

        IReadOnlyList<IStorageFile> fileList = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            FileTypeFilter = fileTypes
        });
        
        if (fileList.Count != 1) return;
        IStorageFile file = fileList.First();
        
        Viewport.LoadTexture(file.Path.AbsolutePath);
    }

    private async void MenuSaveTextureOptionClicked(object? sender, EventArgs e)
    {
        List<FilePickerFileType> fileTypes = FileFormatManager.BuildFilePickerFileList();

        IStorageFile? file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
        {
            FileTypeChoices = fileTypes
        });
        if (file == null) return;
        
        FileFormatBase? ff = FileFormatManager.GetByExtension(System.IO.Path.GetExtension(file.Path.AbsolutePath).TrimStart('.'));
        if (ff == null) return;
        
        ff.Save(Viewport.Bitmap, file.Path.AbsolutePath);
    }

    private void ColorSpectrum_OnColorChanged(object? sender, ColorChangedEventArgs e)
    {
        Viewport.SetPrimaryColor(new PwColor(e.NewColor));
    }

    private void ColorSpinR_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (Viewport == null || _disableEvents) return;
        
        byte r = (byte)e.NewValue;
        Viewport.SetPrimaryColor(Viewport.PrimaryColor with {R = r});
    }

    private void ColorSpinG_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (Viewport == null || _disableEvents) return;
        
        byte g = (byte)e.NewValue;
        Viewport.SetPrimaryColor(Viewport.PrimaryColor with {G = g});
    }

    private void ColorSpinB_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (Viewport == null || _disableEvents) return;
        
        byte b = (byte)e.NewValue;
        Viewport.SetPrimaryColor(Viewport.PrimaryColor with {B = b});
    }

    private void ColorSpinA_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (Viewport == null || _disableEvents) return;
        
        byte a = (byte)e.NewValue;
        Viewport.SetPrimaryColor(Viewport.PrimaryColor with {A = a});
    }

    private void ColorTextR_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!byte.TryParse(ColorTextR.Text, out byte r)) return;
        
        Viewport.SetPrimaryColor(Viewport.PrimaryColor with {R = r});
    }

    private void ColorTextG_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!byte.TryParse(ColorTextG.Text, out byte g)) return;
        
        Viewport.SetPrimaryColor(Viewport.PrimaryColor with {G = g});
    }

    private void ColorTextB_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!byte.TryParse(ColorTextB.Text, out byte b)) return;
        
        Viewport.SetPrimaryColor(Viewport.PrimaryColor with {B = b});
    }

    private void ColorTextA_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!byte.TryParse(ColorTextA.Text, out byte a)) return;
        
        Viewport.SetPrimaryColor(Viewport.PrimaryColor with {A = a});
    }

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        Viewport.Center();
    }

    private void PixelToolOptionClicked(object? sender, RoutedEventArgs e)
    {
        SetTool(new PixelTool());
    }

    private void SetTool(ViewportTool? tool)
    {
        Viewport.SetTool(tool);

        // Update buttons here
        PixelToolButton.IsChecked = tool is PixelTool;
        EraserToolButton.IsChecked = tool is EraserTool;
        ColorPickerTool.IsChecked = tool is ColorPickerTool;
        FloodFillTool.IsChecked = tool is FloodFillTool;
        RectangleTool.IsChecked = tool is RectangleTool;
        EllipseTool.IsChecked = tool is EllipseTool;
        RectSelectTool.IsChecked = tool is SelectionRectangleTool;
        MoveSelectionTool.IsChecked = tool is MoveSelectionTool;
        MagicWandTool.IsChecked = tool is MagicWandTool;
    }

    private void EraserToolButton_OnClick(object? sender, RoutedEventArgs e)
    {
        SetTool(new EraserTool());
    }

    private void ColorPickerTool_OnClick(object? sender, RoutedEventArgs e)
    {
        SetTool(new ColorPickerTool());
    }

    private void FloodFillTool_OnClick(object? sender, RoutedEventArgs e)
    {
        SetTool(new FloodFillTool());
    }

    private void RectangleTool_OnClick(object? sender, RoutedEventArgs e)
    {
        SetTool(new RectangleTool());
    }

    private void UndoOptionClicked(object? sender, EventArgs e)
    {
        Viewport.UndoRedoStack.Undo();
    }

    private void RedoOptionClicked(object? sender, EventArgs e)
    {
        Viewport.UndoRedoStack.Redo();
    }

    private void EllipseTool_OnClick(object? sender, RoutedEventArgs e)
    {
        SetTool(new EllipseTool());
    }

    private async void ResizeImageOptionClicked(object? sender, EventArgs e)
    {
        ResizeImageResult? result = await new ResizeImageWindow(Viewport.Bitmap.Width, Viewport.Bitmap.Height)
            .ShowDialog<ResizeImageResult?>(this);
        if (result == null) return;
        
        Viewport.RunCommand(new ResizeImageCommand(result.Width, result.Height, result.InterpolationMode));
    }

    private async void ResizeViewportOptionClicked(object? sender, EventArgs e)
    {
        ResizeViewportResult? result = await new ResizeViewportWindow(Viewport.Bitmap.Width, Viewport.Bitmap.Height)
            .ShowDialog<ResizeViewportResult?>(this);
        if (result == null) return;
        
        Viewport.RunCommand(new ResizeViewportCommand(result.Width, result.Height, result.Anchor));
    }

    private void ColorTextH_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        
    }

    private void ColorTextS_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        
    }

    private void ColorTextV_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        
    }

    private void ColorSpinH_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (Viewport == null || _disableEvents) return;
        
        Viewport.SetPrimaryColor(new PwColor(ColorSpinH.Color));
    }

    private void ColorSpinS_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (Viewport == null || _disableEvents) return;
        
        Viewport.SetPrimaryColor(new PwColor(ColorSpinS.Color));
    }

    private void ColorSpinV_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (Viewport == null || _disableEvents) return;
        
        Viewport.SetPrimaryColor(new PwColor(ColorSpinV.Color));
    }

    private void HexText_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (Viewport == null || _disableEvents) return;

        string hex = HexText.Text.TrimStart('#');
        
        if (hex.Length != 6 && hex.Length != 8) return;
         
        Viewport.SetPrimaryColor(new PwColor(hex));
    }

    private void RectSelectTool_OnClick(object? sender, RoutedEventArgs e)
    {
        SetTool(new SelectionRectangleTool());
    }

    private void AboutMenuOptionClicked(object? sender, EventArgs e)
    {
        new AboutWindow().ShowDialog(this);
    }

    private void MoveSelectionTool_OnClick(object? sender, RoutedEventArgs e)
    {
        SetTool(new MoveSelectionTool());
    }

    private void MagicWandTool_OnClick(object? sender, RoutedEventArgs e)
    {
        SetTool(new MagicWandTool());
    }
}