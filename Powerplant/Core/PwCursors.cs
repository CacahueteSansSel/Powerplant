using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Powerplant.Core;

public static class PwCursors
{
    public static Cursor Default { get; private set; }
    public static Cursor RectDraw { get; private set; }
    public static Cursor ColorPicker { get; private set; }

    public static void Init()
    {
        Default = new Cursor(StandardCursorType.Arrow);
        RectDraw = new Cursor(StandardCursorType.Cross);
        
        ColorPicker = new Cursor(new Bitmap(AssetLoader.Open(
            new Uri("avares://Powerplant/Resources/cursors/color_picker.png"))), new PixelPoint(6, 26));
    }
}