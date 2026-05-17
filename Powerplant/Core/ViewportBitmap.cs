using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SkiaSharp;

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

    public unsafe ViewportBitmap(SKBitmap bitmap)
    {
        _bitmap = new WriteableBitmap(new PixelSize(bitmap.Width, bitmap.Height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Premul);
        _buffer = new PwColor[(int)_bitmap.Size.Width * (int)_bitmap.Size.Height];

        Width = (int)_bitmap.Size.Width;
        Height = (int)_bitmap.Size.Height;

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                _buffer[y * Width + x] = new PwColor(bitmap.GetPixel(x, y));
            }
        }
        
        Sync();
    }
    
    public void ApplyBitmap(ViewportBitmap source, int destX, int destY)
    {
        for (int y = 0; y < source.Height; y++)
        {
            for (int x = 0; x < source.Width; x++)
            {
                Set(destX + x, destY + y, source.Get(x, y));
            }
        }
    }

    public void ApplyBitmap(ViewportBitmap source)
    {
        if (source.Width != Width || source.Height != Height)
            throw new ArgumentException("Source bitmap must have the same dimensions as the target bitmap.");
        
        Array.Copy(source._buffer, _buffer, source._buffer.Length);
    }
    
    public ViewportBitmap Copy()
    {
        ViewportBitmap copy = new ViewportBitmap(Width, Height);
        Array.Copy(_buffer, copy._buffer, _buffer.Length);
        
        return copy;
    }
    
    public ViewportBitmap Copy(int x, int y, int width, int height)
    {
        ViewportBitmap cutBitmap = new ViewportBitmap(width, height);

        for (int py = 0; py < height; py++)
        {
            for (int px = 0; px < width; px++)
            {
                cutBitmap.Set(px, py, Get(x + px, y + py));
            }
        }

        return cutBitmap;
    }
    
    public void Fill(PwColor color)
    {
        Array.Fill(_buffer, color);
    }

    public PwColor Get(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
            return PwColor.Transparent;
        
        return _buffer[y * Width + x];
    }

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

    public ViewportBitmap Scale(int width, int height, BitmapInterpolationMode interpolationMode)
    {
        using ILockedFramebuffer fb = _bitmap.Lock();

        SKImageInfo info = new SKImageInfo(
            Width,
            Height,
            SKColorType.Bgra8888,
            SKAlphaType.Premul);

        SKBitmap srcBitmap = new SKBitmap(info, fb.RowBytes);
        srcBitmap.SetPixels(fb.Address);
        
        SKBitmap resized = new SKBitmap(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul));
        using SKCanvas canvas = new SKCanvas(resized);

        if (!srcBitmap.ScalePixels(resized, new SKSamplingOptions(SKFilterMode.Nearest)))
        {
            throw new Exception("Failed to scale bitmap");
        }

        return new ViewportBitmap(resized);
    }
}