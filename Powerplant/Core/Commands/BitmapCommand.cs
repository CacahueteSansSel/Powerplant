using System;
using System.Numerics;
using Avalonia;
using Avalonia.Media.Imaging;

namespace Powerplant.Core.Commands;

public class BitmapCommand : Command
{
    private ViewportBitmap _oldBitmap;
    private Bitmap _bitmap;
    private Vector2 _offset;

    public BitmapCommand(int x, int y, Bitmap bitmap)
    {
        _offset = new Vector2(x, y);
        _bitmap = bitmap;
    }

    public override void Init()
    {
        _oldBitmap = Bitmap.Copy();
    }

    public override unsafe void Run()
    {
        byte[] pixels = new byte[_bitmap.PixelSize.Width * _bitmap.PixelSize.Height * 4];
        _bitmap.CopyPixels(new PixelRect(0, 0, _bitmap.PixelSize.Width, _bitmap.PixelSize.Height), 
            (IntPtr)pixels.AsMemory().Pin().Pointer, pixels.Length, _bitmap.PixelSize.Width * 4);

        for (int y = 0; y < _bitmap.PixelSize.Height; y++)
        {
            for (int x = 0; x < _bitmap.PixelSize.Width; x++)
            {
                PwColor color = new(pixels[y * _bitmap.PixelSize.Width * 4 + x * 4],
                    pixels[y * _bitmap.PixelSize.Width * 4 + x * 4 + 1],
                    pixels[y * _bitmap.PixelSize.Width * 4 + x * 4 + 2],
                    pixels[y * _bitmap.PixelSize.Width * 4 + x * 4 + 3]);
                
                Bitmap.Set((int)(x + _offset.X), (int)(y + _offset.Y), color, true);
            }
        }
    }

    public override void Undo()
    {
        Viewport.SetBitmap(_oldBitmap);
    }
}