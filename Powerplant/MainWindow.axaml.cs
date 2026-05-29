using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Powerplant.Core;
using Powerplant.Core.Commands;
using Powerplant.Core.Effects;
using Powerplant.Core.Platforms;
using Powerplant.Core.Tools;
using Powerplant.FileFormats;
using Powerplant.Windows;
using Powerplant.Windows.Effects;
using ReactiveUI;
using Path = System.IO.Path;

namespace Powerplant;

public partial class MainWindow : Window
{
    private bool _disableEvents = false;
    private string? _currentFilename;
    private bool _isFileModified = false;

    private ViewportTool[] _tools;

    public MainWindow()
    {
        InitializeComponent();

        _tools =
        [
            new ColorPickerTool(),
            new EllipseTool(),
            new EraserTool(),
            new FloodFillTool(),
            new MagicWandTool(),
            new MoveSelectionTool(),
            new PixelTool(),
            new RectangleTool(),
            new SelectionRectangleTool()
        ];

        DataContext = new MainWindowCommands(this);

        Viewport.OnPrimaryColorChanged += ViewportOnPrimaryColorChanged;
        Viewport.OnSecondaryColorChanged += ViewportOnSecondaryColorChanged;
        Viewport.OnBitmapChanged += ViewportOnBitmapChanged;
        Viewport.OnCursorPositionChanged += ViewportOnCursorPositionChanged;
        Viewport.OnSelectionChanged += ViewportOnSelectionChanged;
        Viewport.OnToolDescriptionTextChanged += ViewportOnToolDescriptionTextChanged;
        Viewport.OnModification += ViewportOnModification;
        Viewport.OnToolChanged += ViewportOnToolChanged;

        SetupTitleBarOffsets();
        BuildWindowMenu();

        KeyDown += OnKeyDown;

        Viewport.SetTool(new PixelTool());
        Viewport.SetPrimaryColor(PwColor.White);
        Viewport.SetSecondaryColor(PwColor.Black);

        UpdateTextureDetails();

        Focus();
    }

    private void ViewportOnToolChanged(object? sender, ViewportTool? tool)
    {
        ToolOptionsBar.IsVisible = tool != null;
        ToolNameText.Text = tool?.Name;

        if (ToolOptionsBar.IsVisible && tool != null)
        {
            Control? toolControl = tool.ToolSettingsControl;

            ToolSettingsControlPanel.Children.Clear();

            if (toolControl != null)
                ToolSettingsControlPanel.Children.Add(toolControl);
        }

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

    private void SetupTitleBarOffsets()
    {
        if (OperatingSystem.IsMacOS())
        {
            LeftTitlebarOffsetPanel.Width = 80;
        }

        if (OperatingSystem.IsWindows())
        {
            RightTitlebarOffsetPanel.Width = 200;
        }
    }

    private void BuildWindowMenu()
    {
        NativeMenu? nativeMenu = NativeMenu.GetMenu(this);
        WindowMenu.Items.Clear();

        if (nativeMenu == null) return;

        foreach (Control control in BuildItemList(nativeMenu.Items))
            WindowMenu.Items.Add(control);
    }

    List<Control> BuildItemList(IList<NativeMenuItemBase> nativeItems)
    {
        List<Control> targetList = new();

        foreach (NativeMenuItemBase item in nativeItems)
        {
            Control windowItem = null;

            switch (item)
            {
                case NativeMenuItemSeparator:
                    windowItem = new Separator();

                    break;
                case NativeMenuItem stdNativeItem:
                    MenuItem mi = new();
                    mi.Header = stdNativeItem.Header;
                    mi.Tag = stdNativeItem;
                    mi.Click += GeneratedWindowItemOnClick;

                    if (stdNativeItem.Menu != null)
                    {
                        foreach (Control control in BuildItemList(stdNativeItem.Menu.Items))
                            mi.Items.Add(control);
                    }

                    windowItem = mi;

                    break;
            }

            if (windowItem != null)
                targetList.Add(windowItem);
        }

        return targetList;
    }

    private void GeneratedWindowItemOnClick(object? sender, RoutedEventArgs e)
    {
        MenuItem item = (MenuItem)sender!;
        NativeMenuItem nativeItem = (NativeMenuItem)item.Tag!;

        if (nativeItem.Command != null)
            nativeItem.Command.Execute(nativeItem.CommandParameter);
    }

    private void ViewportOnModification()
    {
        SetModified(true);
    }

    private void ViewportOnToolDescriptionTextChanged(object? sender, string text)
    {
        ToolDetailsText.Text = text;
    }

    private void ViewportOnSelectionChanged(object? sender, PixelSelection e)
    {
        if (e.IsEmpty)
        {
            SelectionDetailsText.Text = "x: 0; y: 0; w: 0; h: 0";
            return;
        }

        SelectionDetailsText.Text =
            $"x: {(int)e.Bounds.X}; y: {(int)e.Bounds.Y}; w: {(int)e.Bounds.Width}; h: {(int)e.Bounds.Height}";
    }

    private void ViewportOnCursorPositionChanged(int x, int y)
    {
        CoordsDetailsText.Text = $"x: {x}; y: {y}";
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        foreach (ViewportTool tool in _tools)
        {
            if (tool.Key != e.Key) continue;

            Viewport.SetTool(tool);
            return;
        }
    }

    private void ViewportOnBitmapChanged(object? sender, ViewportBitmap e)
    {
        UpdateTextureDetails();
    }

    private void UpdateTextureDetails()
    {
        Title = "";

        if (_currentFilename != null)
        {
            Title = Path.GetFileName(_currentFilename);
        }
        else Title = $"New Texture";

        if (_isFileModified) Title += "*";

        Title += $" ({Viewport.Bitmap.Width}x{Viewport.Bitmap.Height}) - Powerplant";
        TitleText.Text = Title;

        TextureSizeDetailsText.Text = $"w: {Viewport.Bitmap.Width}; y: {Viewport.Bitmap.Height}";
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

    private void SetModified(bool modified)
    {
        _isFileModified = modified;
        PlatformManager.Current.SetModifiedFlagOnWindow(this, modified);

        UpdateTextureDetails();
    }

    private async void OpenFile(string path)
    {
        if (_isFileModified)
        {
            if (!await PlatformManager.Current.ShowConfirmDialog("Unsaved changes",
                    "You have unsaved changes. If you continue, you will loose progress ! Continue anyway ?",
                    "Continue and loose progress", "Cancel"))
            {
                return;
            }
        }

        Viewport.LoadTexture(path);
        RecentFilesManager.Add(path);

        _currentFilename = path;

        SetModified(false);
    }

    private void SaveFile(string path)
    {
        FileFormatBase? ff = FileFormatManager.GetByExtension(Path.GetExtension(path).TrimStart('.'));
        if (ff == null) return;

        ff.Save(Viewport.Bitmap, path);
        _currentFilename = path;
        RecentFilesManager.Add(path);

        SetModified(false);
    }

    private void ColorSpectrum_OnColorChanged(object? sender, ColorChangedEventArgs e)
    {
        Viewport.SetPrimaryColor(new PwColor(e.NewColor));
    }

    private void ColorSpinR_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (Viewport == null || _disableEvents) return;

        byte r = (byte)e.NewValue;
        Viewport.SetPrimaryColor(Viewport.PrimaryColor with { R = r });
    }

    private void ColorSpinG_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (Viewport == null || _disableEvents) return;

        byte g = (byte)e.NewValue;
        Viewport.SetPrimaryColor(Viewport.PrimaryColor with { G = g });
    }

    private void ColorSpinB_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (Viewport == null || _disableEvents) return;

        byte b = (byte)e.NewValue;
        Viewport.SetPrimaryColor(Viewport.PrimaryColor with { B = b });
    }

    private void ColorSpinA_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (Viewport == null || _disableEvents) return;

        byte a = (byte)e.NewValue;
        Viewport.SetPrimaryColor(Viewport.PrimaryColor with { A = a });
    }

    private void ColorTextR_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!byte.TryParse(ColorTextR.Text, out byte r)) return;

        Viewport.SetPrimaryColor(Viewport.PrimaryColor with { R = r });
    }

    private void ColorTextG_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!byte.TryParse(ColorTextG.Text, out byte g)) return;

        Viewport.SetPrimaryColor(Viewport.PrimaryColor with { G = g });
    }

    private void ColorTextB_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!byte.TryParse(ColorTextB.Text, out byte b)) return;

        Viewport.SetPrimaryColor(Viewport.PrimaryColor with { B = b });
    }

    private void ColorTextA_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!byte.TryParse(ColorTextA.Text, out byte a)) return;

        Viewport.SetPrimaryColor(Viewport.PrimaryColor with { A = a });
    }

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        Viewport.Center();
    }

    private void PixelToolOptionClicked(object? sender, RoutedEventArgs e)
    {
        Viewport.SetTool(new PixelTool());
    }

    private void EraserToolButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Viewport.SetTool(new EraserTool());
    }

    private void ColorPickerTool_OnClick(object? sender, RoutedEventArgs e)
    {
        Viewport.SetTool(new ColorPickerTool());
    }

    private void FloodFillTool_OnClick(object? sender, RoutedEventArgs e)
    {
        Viewport.SetTool(new FloodFillTool());
    }

    private void RectangleTool_OnClick(object? sender, RoutedEventArgs e)
    {
        Viewport.SetTool(new RectangleTool());
    }

    private void EllipseTool_OnClick(object? sender, RoutedEventArgs e)
    {
        Viewport.SetTool(new EllipseTool());
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
        Viewport.SetTool(new SelectionRectangleTool());
    }

    private void MoveSelectionTool_OnClick(object? sender, RoutedEventArgs e)
    {
        Viewport.SetTool(new MoveSelectionTool());
    }

    private void MagicWandTool_OnClick(object? sender, RoutedEventArgs e)
    {
        Viewport.SetTool(new MagicWandTool());
    }

    public void ColorSwitchButtonClicked(object? sender, PointerPressedEventArgs e)
    {
        PwColor primaryColor = Viewport.PrimaryColor;
        PwColor secondaryColor = Viewport.SecondaryColor;

        Viewport.SetPrimaryColor(secondaryColor);
        Viewport.SetSecondaryColor(primaryColor);
    }

    private async void Window_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (!_isFileModified) return;
        e.Cancel = true;
        
        await ShowFileModifiedDialogAsync();

        _isFileModified = false;
        Close();
    }

    async Task<bool> ShowFileModifiedDialogAsync()
    {
        bool result = await PlatformManager.Current.ShowConfirmDialog("Save changes ?",
            "You have unsaved changes. Save before exiting ?", "Save", "Discard changes");

        if (result && DataContext is MainWindowCommands cmds)
        {
            if (_currentFilename != null) cmds.MenuSaveTextureOptionClicked();
            else cmds.MenuSaveTextureAsOptionClicked();
        }

        return result;
    }

    public class MainWindowCommands
    {
        private MainWindow _win;

        public MainWindowCommands(MainWindow window)
        {
            _win = window;
        }

        public async void MenuNewTextureOptionClicked()
        {
            Vector2 size = await new NewTextureWindow().ShowDialog<Vector2>(_win);
            if (size.X == 0 || size.Y == 0) return;
            
            if (_win._isFileModified)
                await _win.ShowFileModifiedDialogAsync();

            _win.Viewport.CreateTexture((int)size.X, (int)size.Y);
        }

        public async void MenuOpenTextureOptionClicked()
        {
            List<FilePickerFileType> fileTypes = FileFormatManager.BuildFilePickerFileList();

            IReadOnlyList<IStorageFile> fileList = await _win.StorageProvider.OpenFilePickerAsync(
                new FilePickerOpenOptions()
                {
                    FileTypeFilter = fileTypes
                });

            if (fileList.Count != 1) return;
            IStorageFile file = fileList.First();

            _win.OpenFile(HttpUtility.UrlDecode(file.Path.AbsolutePath));
        }

        public async void MenuSaveTextureAsOptionClicked()
        {
            List<FilePickerFileType> fileTypes = FileFormatManager.BuildFilePickerFileList();

            IStorageFile? file = await _win.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
            {
                FileTypeChoices = fileTypes
            });
            if (file == null) return;

            _win.SaveFile(HttpUtility.UrlDecode(file.Path.AbsolutePath));
        }

        public void UndoOptionClicked()
        {
            _win.Viewport.UndoRedoStack.Undo();
        }

        public void RedoOptionClicked()
        {
            _win.Viewport.UndoRedoStack.Redo();
        }

        public async void ResizeImageOptionClicked()
        {
            ResizeImageResult? result =
                await new ResizeImageWindow(_win.Viewport.Bitmap.Width, _win.Viewport.Bitmap.Height)
                    .ShowDialog<ResizeImageResult?>(_win);
            if (result == null) return;

            _win.Viewport.RunCommand(new ResizeImageCommand(result.Width, result.Height, result.InterpolationMode));
        }

        public async void ResizeViewportOptionClicked()
        {
            ResizeViewportResult? result =
                await new ResizeViewportWindow(_win.Viewport.Bitmap.Width, _win.Viewport.Bitmap.Height)
                    .ShowDialog<ResizeViewportResult?>(_win);
            if (result == null) return;

            _win.Viewport.RunCommand(new ResizeViewportCommand(result.Width, result.Height, result.Anchor));
        }

        public void SelectAllOptionClicked()
        {
            _win.Viewport.SetSelection(PixelSelection.Rectangle(0, 0, _win.Viewport.Bitmap.Width,
                _win.Viewport.Bitmap.Height));
        }

        public async void OpenRecentMenuOptionClicked()
        {
            string? filename = await new OpenRecentWindow().ShowDialog<string>(_win);

            if (!string.IsNullOrWhiteSpace(filename))
                _win.OpenFile(filename);
        }

        public void MenuSaveTextureOptionClicked()
        {
            if (_win._currentFilename == null) return;

            _win.SaveFile(_win._currentFilename);
        }

        public void VerticalFlipOptionClicked()
        {
            _win.Viewport.RunCommand(new FlipCommand(false));
        }

        public void HorizontalFlipOptionClicked()
        {
            _win.Viewport.RunCommand(new FlipCommand(true));
        }

        public void PureBlackEffectOptionClicked()
        {
            _win.Viewport.RunCommand(new EffectRunner<PureBlackEffect>(_win.Viewport).RunEffectCommand);
        }

        public void OutlineEffectOptionClicked()
        {
            new OutlineWindow(_win.Viewport).Show(_win);
        }

        public void CenterViewOptionClicked()
        {
            _win.Viewport.Center();
        }

        public void ZoomPlusOptionClicked()
        {
            _win.Viewport.IncrementZoom();
        }

        public void ZoomMinusOptionClicked()
        {
            _win.Viewport.DecrementZoom();
        }

        public void ResizeToSelectionOptionClicked()
        {
            ViewportBitmap? bitmap = _win.Viewport.GenerateBitmapFromSelection();
            if (bitmap == null) return;

            _win.Viewport.ClearSelection();
            _win.Viewport.RunCommand(new SetBitmapCommand(bitmap));
        }

        public void CopyOptionClicked()
        {
            if (_win.Viewport.Selection.IsEmpty)
                return;

            ViewportBitmap? bitmap = _win.Viewport.GenerateBitmapFromSelection();
            if (bitmap == null) return;

            _win.Clipboard?.SetBitmapAsync(bitmap.Bitmap);
        }

        public void CutOptionClicked()
        {
            if (_win.Viewport.Selection.IsEmpty)
                return;

            ViewportBitmap? bitmap = _win.Viewport.GenerateBitmapFromSelection();
            if (bitmap == null) return;

            _win.Clipboard?.SetBitmapAsync(bitmap.Bitmap);
            _win.Viewport.RunCommand(new PixelsCommand(_win.Viewport.Selection.Pixels, PwColor.Transparent));
            _win.Viewport.ClearSelection();
        }

        public async void PasteOptionClicked()
        {
            Bitmap? bitmap = await _win.Clipboard!.TryGetBitmapAsync();
            if (bitmap == null) return;

            _win.Viewport.SetTool(new PasteImageTool(bitmap, true));
        }

        public void HideHelperCheckerboardOptionClicked()
        {
            _win.Viewport.SetFixedCheckerboardTileSize(null);
        }

        public void SetHelperCheckerboardOptionClicked(int size)
        {
            _win.Viewport.SetFixedCheckerboardTileSize(new Vector2(size, size));
        }
    }
}