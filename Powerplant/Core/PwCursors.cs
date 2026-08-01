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
    public static Cursor Eraser { get; private set; }
    public static Cursor Pencil { get; private set; }
    public static Cursor Bucket { get; private set; }
    public static Cursor MagicWand { get; private set; }

    public static void Init()
    {
        Default = new Cursor(StandardCursorType.Arrow);
        RectDraw = new Cursor(StandardCursorType.Cross);
        
        ColorPicker = new Cursor(new Bitmap(AssetLoader.Open(
            new Uri("avares://Powerplant/Resources/cursors/color_picker.png"))), new PixelPoint(6, 26));
        Eraser = new Cursor(new Bitmap(AssetLoader.Open(
            new Uri("avares://Powerplant/Resources/cursors/eraser.png"))), new PixelPoint(8, 24));
        Pencil = new Cursor(new Bitmap(AssetLoader.Open(
            new Uri("avares://Powerplant/Resources/cursors/pencil.png"))), new PixelPoint(6, 26));
        Bucket = new Cursor(new Bitmap(AssetLoader.Open(
            new Uri("avares://Powerplant/Resources/cursors/bucket.png"))), new PixelPoint(8, 25));
        MagicWand = new Cursor(new Bitmap(AssetLoader.Open(
            new Uri("avares://Powerplant/Resources/cursors/wand.png"))), new PixelPoint(8, 8));
    }
}