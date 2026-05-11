using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Powerplant.Core;

public class ViewportBitmap
{
    private WriteableBitmap _bitmap;
    private PwColor[] _buffer;

    public IImage Image => _bitmap;
    public int Width { get; }
    public int Height { get; }

    public ViewportBitmap(int width, int height)
    {
        _bitmap = new WriteableBitmap(new PixelSize(width, height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Premul);
        _buffer = new PwColor[width * height];

        Width = width;
        Height = height;
        
        Fill(PwColor.Transparent);
    }
    
    public void Fill(PwColor color)
    {
        Array.Fill(_buffer, color);
    }

    public PwColor Get(int x, int y)
        => _buffer[y * Width + x];

    public void Set(int x, int y, PwColor color)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
            return;
        
        _buffer[y * Width + x] = color;
    }

    public unsafe void Sync()
    {
        using var fb = _bitmap.Lock();
        uint* buffer = (uint*)fb.Address;
        
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                buffer[y * fb.RowBytes / 4 + x] = Get(x, y).ToBgra();
            }
        }
    }
}